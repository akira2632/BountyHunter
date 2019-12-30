using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace RandomMap_V6
{
    #region 地圖輸出者基底類別與輸出階段初始化
    public abstract class IMapPresenter : IGenerater
    {
        protected static Queue<Coordinate> printTargets;
        protected static Coordinate target;
        protected static MapPrinter mapPrinter;

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
            mapPrinter = generaterManager.GetGeneraterFactry().GetMapPrinter();
            printTargets = mapBuilder.GetTargets();
            columnMax = columnMin = rowMax = rowMin = 0;

            generaterManager.SetNextGenerater(new MiniMapPresenter(generaterManager));
        }
    }
    #endregion

    #region 地圖輸出者
    public class MiniMapPresenter : IMapPresenter
    {
        public MiniMapPresenter(MapGenerateManager generaterManager)
            : base(generaterManager) { }

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
                            if (mapBuilder.GetBlockType(target) == BlockType.Safe)
                                mapPrinter.PrintMiniMapSafeBlockWall(target, d);
                            else
                                mapPrinter.PrintMiniMapWall(target, d);
                        }
                        else if (mapBuilder.GetBoundaryType(target, d) == BoundaryType.Entry)
                            mapPrinter.PrintMiniMapEntry(target, d);

                        if (!(mapBuilder.HasBlock(target + d + Direction.LeftSide(d))
                            && mapBuilder.HasOpenBoundary(target, d)
                            && mapBuilder.HasOpenBoundary(target, Direction.LeftSide(d))
                            && mapBuilder.HasOpenBoundary(target + d + Direction.LeftSide(d), Direction.RightSide(d))
                            && mapBuilder.HasOpenBoundary(target + d + Direction.LeftSide(d), Direction.Reverse(d))))
                        {
                            if (mapBuilder.GetBlockType(target) == BlockType.Safe)
                                mapPrinter.PrintMiniMapSafeBlockCorner(target, d, Direction.LeftSide(d));
                            else
                                mapPrinter.PrintMiniMapCorner(target, d, Direction.LeftSide(d));
                        }
                    }

                    generaterManager.AddTicks();
                    generaterManager.SetNextGenerater(new GameMapPresenter(generaterManager));
                }
                else
                {
                    generaterManager.AddTicks();
                    generaterManager.SetNextGenerater(new NullBlockPresenter(generaterManager));
                }
            }
            else
                generaterManager.SetNextGenerater(new NaveGraphGenerate(generaterManager));
        }
    }

    public class NullBlockPresenter : IMapPresenter
    {
        public NullBlockPresenter(MapGenerateManager generaterManager) : base(generaterManager)
        {
        }

        public override void Update()
        {
            for (int column = 0; column < 15; column++)
                for (int row = 0; row < 15; row++)
                {
                    mapPrinter.PrintGameMapWall(target.Column * 15 + column, target.Row * 15 + row);
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
            /*if (mapBuilder.GetBlockType(target) == BlockType.BossRoom)
                mapPrinter.BossRoom(true);
            else
                mapPrinter.BossRoom(false);*/
        }

        public override void Update()
        {
            for (int column = 0; column < terrainData.GetLength(0); column++)
                for (int row = 0; row < terrainData.GetLength(1); row++)
                {
                    if (terrainData[column, row] < 10)
                        mapPrinter.PrintGameMapGround(target.Column * 15 + column, target.Row * 15 + row);
                    else
                        mapPrinter.PrintGameMapWall(target.Column * 15 + column, target.Row * 15 + row);
                }

            generaterManager.AddTicks();
            generaterManager.SetNextGenerater(new MiniMapPresenter(generaterManager));
        }
    }

    public class NaveGraphGenerate : IMapPresenter
    {
        public NaveGraphGenerate(MapGenerateManager generaterManager)
            : base(generaterManager) { }

        public override void Initail()
        {
            GridGraph gridGraph = AstarPath.active.data.gridGraph;

            int width = (Mathf.Abs(columnMax) + Mathf.Abs(columnMin)) * 15, depth = (Mathf.Abs(rowMax) + Mathf.Abs(rowMin)) * 15;
            Vector3 centerPosition = generaterManager.grid.CellToWorld(
                new Vector3Int(((columnMax + columnMin) / 2) * 15, ((rowMax + rowMin) / 2) * 15, 0));

            gridGraph.SetDimensions(width * 5, depth * 5, 0.2f);
            gridGraph.center = centerPosition;

            AstarPath.active.Scan(gridGraph);
            Debug.Log(Time.time - generaterManager.StartTime);

            float x, y;
            mapPrinter.GetEntryPosition(out x, out y);
            generaterManager.SetPlayerPosition(x, y);
        }
    }
    #endregion
}
