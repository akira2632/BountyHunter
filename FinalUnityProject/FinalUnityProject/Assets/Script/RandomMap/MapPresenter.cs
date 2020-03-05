using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace RandomMap
{
    #region 地圖呈現者基底類別與輸出階段初始化
    public abstract class IMapPresenter : IGenerater
    {
        protected static Queue<Coordinate> printTargets;
        protected static Coordinate target;
        protected static TileMapBuilder tileMapBuilder;

        protected static int columnMax, rowMax, columnMin, rowMin;

        public IMapPresenter(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        protected void UpdateColumnRowMinMax()
        {
            columnMax = columnMax < target.Column ? target.Column : columnMax;
            columnMin = columnMin > target.Column ? target.Column : columnMin;
            rowMax = rowMax < target.Row ? target.Row : rowMax;
            rowMin = rowMin > target.Row ? target.Row : rowMin;
        }
    }

    public class MapPrintingInitail : IMapPresenter
    {
        public MapPrintingInitail(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Update()
        {
            tileMapBuilder = generaterManager.GetGeneraterFactry().GetMapPrinter();
            printTargets = mapBuilder.GetTargets();
            columnMax = columnMin = rowMax = rowMin = 0;

            generaterManager.SetNextGenerater(new MiniMapPresenter(generaterManager));
        }
    }
    #endregion

    #region 地圖呈現者
    public class MiniMapPresenter : IMapPresenter
    {
        bool isEntryBlock;

        public MiniMapPresenter(MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            isEntryBlock = false;
        }

        public override void Update()
        {
            if (printTargets.Count > 0)
            {
                target = printTargets.Dequeue();
                UpdateColumnRowMinMax();

                if (mapBuilder.GetBlockType(target) != BlockType.Null)
                {
                    for (int d = 0; d < Direction.DirectionCount; d++)
                    {
                        if (mapBuilder.HasWall(target, d))
                        {
                            tileMapBuilder.SetMiniMapWall(target, d, mapBuilder.GetBlockType(target));
                        }
                        else if (mapBuilder.GetBoundaryType(target, d) == BoundaryType.Entry)
                        {
                            tileMapBuilder.SetMiniMapEntry(target, d);
                            isEntryBlock = true;
                        }

                        if (!(mapBuilder.HasBlock(target + d + Direction.LeftSide(d))
                            && mapBuilder.HasOpenBoundary(target, d)
                            && mapBuilder.HasOpenBoundary(target, Direction.LeftSide(d))
                            && mapBuilder.HasOpenBoundary(target + d + Direction.LeftSide(d), Direction.RightSide(d))
                            && mapBuilder.HasOpenBoundary(target + d + Direction.LeftSide(d), Direction.Reverse(d))))
                        {
                            tileMapBuilder.SetMiniMapCorner(target, d, Direction.LeftSide(d), mapBuilder.GetBlockType(target));
                        }
                    }

                    generaterManager.AddTicks();
                    if (!isEntryBlock)
                        generaterManager.SetNextGenerater(new GameMapPresenter(generaterManager));
                    else
                        generaterManager.SetNextGenerater(new MapEntryPresenter(generaterManager));
                }
                else
                {
                    generaterManager.AddTicks();
                    generaterManager.SetNextGenerater(new NullBlockPresenter(generaterManager));
                }
            }
            else
                generaterManager.SetNextGenerater(new PresentMap(generaterManager));
        }
    }

    public class NullBlockPresenter : IMapPresenter
    {
        public NullBlockPresenter(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Update()
        {
            for (int column = 0; column < 15; column++)
                for (int row = 0; row < 15; row++)
                {
                    tileMapBuilder.SetGameMapWall(target.Column * 15 + column, target.Row * 15 + row);
                }

            generaterManager.AddTicks();
            generaterManager.SetNextGenerater(new MiniMapPresenter(generaterManager));
        }
    }

    public class MapEntryPresenter : IMapPresenter
    {
        sbyte[,] terrainData;
        public MapEntryPresenter(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Initail()
        {
            terrainData = mapBuilder.GetTerrainData(target);
        }

        public override void Update()
        {
            for (int column = 0; column < terrainData.GetLength(0); column++)
                for (int row = 0; row < terrainData.GetLength(1); row++)
                {
                    if (terrainData[column, row] < 10)
                    {
                        tileMapBuilder.SetGameMapGround(target.Column * 15 + column, target.Row * 15 + row);

                        int random = UnityEngine.Random.Range(0, 100);

                        if (random > 85 && terrainData[column, row] > 4)
                            tileMapBuilder.SetWallDecorates(random, target.Column * 15 + column, target.Row * 15 + row);
                        else if (random > 70 && terrainData[column, row] < 4)
                            tileMapBuilder.SetGroundDecorates(random, target.Column * 15 + column, target.Row * 15 + row);
                    }
                    else
                        tileMapBuilder.SetGameMapWall(target.Column * 15 + column, target.Row * 15 + row);
                }

            for (int d = 0; d < Direction.DirectionCount; d++)
                if (mapBuilder.GetBoundaryType(target, d) == BoundaryType.Entry)
                {
                    var direction = (Direction)d;
                    var columnDisp = direction.Column == 0 ? 7 : direction.Row > 0 ? 0 : 14;
                    var rowDisp = direction.Row == 0 ? 7 : direction.Column > 0 ? 0 : 14;

                    tileMapBuilder.SetGameMapEntry(
                        target.Column * 15 + columnDisp,
                        target.Row * 15 + rowDisp, direction);
                }

            generaterManager.AddTicks();
            generaterManager.SetNextGenerater(new MiniMapPresenter(generaterManager));
        }
    }

    public class GameMapPresenter : IMapPresenter
    {
        sbyte[,] terrainData;

        public GameMapPresenter(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Initail()
        {
            terrainData = mapBuilder.GetTerrainData(target);
        }

        public override void Update()
        {
            for (int column = 0; column < terrainData.GetLength(0); column++)
                for (int row = 0; row < terrainData.GetLength(1); row++)
                {
                    if (terrainData[column, row] < 10)
                    {
                        if (mapBuilder.GetBlockType(target) == BlockType.BossRoom)
                            tileMapBuilder.SetBossRoomGround(target.Column * 15 + column, target.Row * 15 + row);
                        else
                            tileMapBuilder.SetGameMapGround(target.Column * 15 + column, target.Row * 15 + row);

                        int random = UnityEngine.Random.Range(0, 100);

                        if (random > 85 && terrainData[column, row] > 4)
                            tileMapBuilder.SetWallDecorates(random, target.Column * 15 + column, target.Row * 15 + row);
                        else if (random > 70 && terrainData[column, row] < 4)
                            tileMapBuilder.SetGroundDecorates(random, target.Column * 15 + column, target.Row * 15 + row);
                        else if (random < 2 && terrainData[column, row] > 4)
                            tileMapBuilder.SetBoxDecorates(random, target.Column * 15 + column, target.Row * 15 + row);
                        else if (random >= 2 && random < 4 && terrainData[column, row] > 4)
                            tileMapBuilder.SetSkullDecorates(random, target.Column * 15 + column, target.Row * 15 + row);
                    }
                    else
                        tileMapBuilder.SetGameMapWall(target.Column * 15 + column, target.Row * 15 + row);
                }

            var spwanPoint = mapBuilder.GetSpwanPoint(target);
            if(spwanPoint != null)
                tileMapBuilder.SetSpwanPoint(target.Column * 15 + 8, target.Row * 15 + 8, spwanPoint);

            generaterManager.AddTicks();
            generaterManager.SetNextGenerater(new MiniMapPresenter(generaterManager));
        }
    }

    public class PresentMap : IMapPresenter
    {
        private float tilemapRefreshDelay = 1f;

        public PresentMap(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Initail()
        {
            tileMapBuilder.PresentTileMap();
        }

        public override void Update()
        {
            if (tilemapRefreshDelay > 0)
                tilemapRefreshDelay -= Time.deltaTime;
            else
            {
                GridGraph gridGraph = AstarPath.active.data.gridGraph;

                int width = (Mathf.Abs(columnMax) + Mathf.Abs(columnMin)) * 15, depth = (Mathf.Abs(rowMax) + Mathf.Abs(rowMin)) * 15;
                Vector3 centerPosition = generaterManager.grid.CellToWorld(
                    new Vector3Int(((columnMax + columnMin) / 2) * 15, ((rowMax + rowMin) / 2) * 15, 0));

                gridGraph.SetDimensions(width * 5, depth * 5, 0.2f);
                gridGraph.center = centerPosition;

                AstarPath.active.Scan(gridGraph);

                generaterManager.AddTicks(10);
                generaterManager.SetNextGenerater(new MapActive(generaterManager));
            }
        }
    }

    public class MapActive : IMapPresenter
    {
        public MapActive(MapGenerateManager generaterManager) : base(generaterManager)
        {
        }

        public override void Update()
        {
            if(!AstarPath.active.isScanning)
            {
                tileMapBuilder.GetEntryPosition(out float x, out float y);
                generaterManager.SetPlayerPosition(x, y);
                tileMapBuilder.ActiveSpwanPoint();
                generaterManager.SetNextGenerater(new MapPresentCompelete(generaterManager));
            }
        }

        public override void End()
        {
            generaterManager.UIManager.LoadCompelete();
        }
    }

    public class MapPresentCompelete : IMapPresenter
    {
        public MapPresentCompelete(MapGenerateManager generaterManager) : base(generaterManager)
        {
        }
    }
    #endregion
}
