using System.Collections.Generic;
using UnityEngine;

namespace RandomMap
{
    #region 地形生成基底類別與初始化
    /// <summary>
    /// 地形生成基底類別
    /// </summary>
    public abstract class ITerrainGenerater : IGenerater
    {
        protected static Queue<Coordinate> generateTargets;
        protected static Coordinate target;

        public ITerrainGenerater(MapGenerateManager generaterManager)
            : base(generaterManager) { }
    }

    /// <summary>
    /// 地形生成階段初始化
    /// </summary>
    public class TerrainGenerateInitail : ITerrainGenerater
    {
        public TerrainGenerateInitail(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Update()
        {
            //Debug.Log("Terrain generate initail");

            generateTargets = mapBuilder.GetTargets();
            target = generateTargets.Dequeue();
            generaterManager.AddTicks();
            generaterManager.SetNextGenerater(
                new BassTerrainGenerater(generaterManager));
        }
    }
    #endregion

    #region 地形生成策略
    /// <summary>
    /// 基礎地形生成者。
    /// 建立絕對不可被堵住的區域。
    /// </summary>
    public class BassTerrainGenerater : ITerrainGenerater
    {
        public BassTerrainGenerater(MapGenerateManager generaterManager)
            : base(generaterManager) { }
        public override void Update()
        {
            if (mapBuilder.GetBlockType(target) != BlockType.Null)
            {
                mapBuilder.SetBassTerrain(target);

                for (int d = 0; d < Direction.DirectionCount; d++)
                {
                    if (mapBuilder.HasSetBoudary(target, d))
                        mapBuilder.SetBoundaryTerrain(target, d);
                    else
                        switch (mapBuilder.GetBoundaryType(target, d))
                        {
                            case BoundaryType.Entry:
                                mapBuilder.SetBoundaryTerrain(target, d, 3, 0);
                                break;
                            case BoundaryType.SmallPath:
                                mapBuilder.SetBoundaryTerrain(target, d, Random.Range(4, 7), Random.Range(-4, 4));
                                break;
                            case BoundaryType.LargePath:
                                mapBuilder.SetBoundaryTerrain(target, d, Random.Range(7, 10), Random.Range(-2, 2));
                                break;
                            case BoundaryType.OpenBoundary:
                                mapBuilder.SetBoundaryTerrain(target, d, Random.Range(10, 13), 0);
                                break;
                            case BoundaryType.Wall:
                                mapBuilder.SetBoundaryTerrain(target, d, 0, 0);
                                break;
                        }
                }

                /*Debug.Log(string.Format($"Bass terrain at Generate ({target.Column},{target.Row}):" +
                    $"{Time.time - generaterManager.ScaleStartTime} secends, Total " +
                    $"{Time.time - generaterManager.StartTime} secends"));
                generaterManager.ScaleStartTime = Time.time;*/
            }
    
            generaterManager.SetNextGenerater(new BasicTerrainGenerater(generaterManager));
        }
    }

    /// <summary>
    /// 簡單隨機地形生成者
    /// </summary>
    public class BasicTerrainGenerater : ITerrainGenerater
    {
        sbyte[,] terrainData;
        bool isEnd;

        public BasicTerrainGenerater(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Initail()
        {
            terrainData = mapBuilder.GetTerrainData(target);
        }

        public override void Update()
        {
            if (mapBuilder.GetBlockType(target) != BlockType.Null)
            {
                //隨機高度
                do
                {
                    isEnd = true;

                    for (int x = 0; x < terrainData.GetLength(0); x++)
                        for (int y = 0; y < terrainData.GetLength(1); y++)
                            if (terrainData[x, y] != -1)
                                FindGeneratePoint(x, y);
                            else
                                isEnd = false;
                } while (!isEnd);

                //處理被環繞的地形
                for (int x = 0; x < terrainData.GetLength(0); x++)
                    for (int y = 0; y < terrainData.GetLength(1); y++)
                        if (terrainData[x, y] < 10)
                            FillSerround(x, y);

                generaterManager.AddTicks();
                /*Debug.Log(string.Format($"Basic terrain generate at ({target.Column},{target.Row}):" +
                    $"{Time.time - generaterManager.ScaleStartTime} secends, Total " +
                    $"{Time.time - generaterManager.StartTime} secends"));
                generaterManager.ScaleStartTime = Time.time;*/
            }

            if (generateTargets.Count > 0)
            {
                target = generateTargets.Dequeue();
                generaterManager.SetNextGenerater(new BassTerrainGenerater(generaterManager));
            }
            else
            {
                generaterManager.SetNextGenerater(new MapPrintingInitail(generaterManager));
            }
        }

        private void FindGeneratePoint(int x, int y)
        {
            for (int xDisp = -1; xDisp < 2; xDisp++)
                for (int yDisp = -1; yDisp < 2; yDisp++)
                    if (!(xDisp == 0 && yDisp == 0)
                        && x + xDisp > -1 && x + xDisp < terrainData.GetLength(0)
                        && y + yDisp > -1 && y + yDisp < terrainData.GetLength(1)
                        && terrainData[x + xDisp, y + yDisp] == -1)
                        RandomHeight(x + xDisp, y + yDisp);
        }

        private void RandomHeight(int x, int y)
        {
            List<sbyte> hightList = new List<sbyte>(8);

            for (int xDisp = -1; xDisp < 2; xDisp++)
                for (int yDisp = -1; yDisp < 2; yDisp++)
                    if (!(xDisp == 0 && yDisp == 0)
                        && x + xDisp > -1 && x + xDisp < terrainData.GetLength(0)
                        && y + yDisp > -1 && y + yDisp < terrainData.GetLength(1)
                        && terrainData[x + xDisp, y + yDisp] != -1)
                        hightList.Add(terrainData[x + xDisp, y + yDisp]);

            sbyte pointHeight = 0;

            foreach (sbyte height in hightList)
                pointHeight += height;

            pointHeight /= (sbyte)hightList.Count;
            pointHeight += (sbyte)Random.Range(1, 5);

            terrainData[x, y] = pointHeight > 10 ? (sbyte)10 : pointHeight;
        }

        private void FillSerround(int x, int y)
        {
            int serroundCount = 0;

            for (int xDisp = -1; xDisp < 2; xDisp++)
                for (int yDisp = -1; yDisp < 2; yDisp++)
                    if (xDisp * yDisp == 0
                        && x + xDisp > -1 && x + xDisp < terrainData.GetLength(0)
                        && y + yDisp > -1 && y + yDisp < terrainData.GetLength(1)
                        && terrainData[x + xDisp, y + yDisp] == 10)
                        serroundCount++;

            if (serroundCount > 3)
                terrainData[x, y] = 10;
        }
    }
    #endregion
}
