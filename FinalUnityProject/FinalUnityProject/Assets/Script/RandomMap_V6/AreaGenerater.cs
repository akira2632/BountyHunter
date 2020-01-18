using System.Collections.Generic;
using UnityEngine;

namespace RandomMap_V6
{
    #region 區域生成基底資料
    /// <summary>
    /// 區域建造者參數。
    /// 紀錄剩餘地圖扣打、區域大小、生成點。
    /// </summary>
    public struct AreaGeneraterParms
    {
        public int MapQuota;
        public int AreaScale;
        public Coordinate StartPoint;
    }

    /// <summary>
    /// 區域任務。
    /// 紀錄參數及指定的生成策略。
    /// </summary>
    public struct AreaGenerateTask
    {
        public AreaGeneraterParms parms;
        public IAreaGenerater generater;
    }

    /// <summary>
    /// 生成策略基底類別。
    /// 紀錄建造者、管理者以及各自的參數。
    /// </summary>
    public abstract class IAreaGenerater : IGenerater
    {
        protected AreaGeneraterParms parms;

        public IAreaGenerater(MapGenerateManager generaterManager)
        : base(generaterManager) { }

        public void SetParamater(AreaGeneraterParms parms)
        {
            this.parms = parms;
        }
    }
    #endregion

    #region 區域生成管理
    /// <summary>
    /// 區域生成初始化。
    /// </summary>
    public class AreaGenerateInitail : IAreaGenerater
    {
        int mapScale;
        GeneraterFactry factry;

        public AreaGenerateInitail(int mapScale, MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            this.mapScale = mapScale;

            factry = generaterManager.GetGeneraterFactry();
            mapBuilder = factry.GetBuilder();
            AreaGeneraterDecider.factry = factry;
            generaterManager.StartTime = Time.time;

            //Debug.Log("AreaGenerateInitailCreate");
        }

        public override void Update()
        {
            generaterManager.SetNextGenerater(
                new AreaTaskManager(
            new AreaGenerateTask
            {
                generater = factry.GetEntryAreaGenerater(),
                parms = new AreaGeneraterParms
                {
                    MapQuota = mapScale,
                    StartPoint = new Coordinate(),
                }
            }, generaterManager));
        }
    }

    /// <summary>
    /// 區域生成策略決策者。
    /// 決定新的生成任務並將之交給生成任務管理者。
    /// 目前是假的 會動而已 :3
    /// </summary>
    public class AreaGeneraterDecider : IAreaGenerater
    {
        public static GeneraterFactry factry;
        static bool hasBossRoom = false;

        List<Coordinate> generaterPointList;
        Queue<AreaGenerateTask> taskQueue = new Queue<AreaGenerateTask>();

        public AreaGeneraterDecider(IEnumerable<Coordinate> generaterPointList,
            AreaGeneraterParms parms, MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("AreaGeneraterDeciderCreate");
            this.parms = parms;
            this.generaterPointList = new List<Coordinate>(generaterPointList);
        }

        public override void Initail()
        {
            //Debug.Log("AreaGeneraterDeciderInitail");
            taskQueue.Clear();
            parms.MapQuota -= parms.AreaScale;
        }

        public override void Update()
        {
            //Debug.Log("AreaGeneraterDeciderUpdate");
            if (generaterPointList.Count > 0)
            {
                if (parms.MapQuota > 0)
                    AddBasicAreaTask();
                else if (!hasBossRoom)
                    AddBossRoomTask();
                else
                    AddAreaSealderTask();
            }
            else
            {
                generaterManager.SetNextGenerater(
                    new AreaTaskManager(taskQueue, generaterManager));
            }
        }

        public override void End()
        {
            //Debug.Log("AreaGeneraterDeciderEnd");
            generaterManager.AddTicks();
        }

        private void AddBossRoomTask()
        {
            List<Coordinate> placeablePoints = new List<Coordinate>(generaterPointList.Count);

            foreach (Coordinate coordinate in generaterPointList)
                if (mapBuilder.HasEmptyArea(coordinate, 3))
                    placeablePoints.Add(coordinate);

            if (placeablePoints.Count > 0)
            {
                int randomIndex = Random.Range(0, placeablePoints.Count * 10) % placeablePoints.Count;

                taskQueue.Enqueue(new AreaGenerateTask
                {
                    generater = factry.GetBossRoomGenerater(),
                    parms = new AreaGeneraterParms
                    {
                        AreaScale = 3,
                        MapQuota = 0,
                        StartPoint = placeablePoints[randomIndex]
                    }
                });

                generaterPointList.Remove(placeablePoints[randomIndex]);
                hasBossRoom = true;
            }
        }

        private void AddAreaSealderTask()
        {
            taskQueue.Enqueue(
                new AreaGenerateTask
                {
                    generater = factry.GetAreaSealder(),
                    parms = new AreaGeneraterParms
                    {
                        StartPoint = generaterPointList[0]
                    }
                });

            generaterPointList.RemoveAt(0);
        }

        private void AddBasicAreaTask()
        {
            //int nextAreaSize = GetNextAreaSize();

            //if (mapBuilder.HasEmptyArea(generaterPointList[0], nextAreaSize))
            //{
            //    taskQueue.Enqueue(
            //        new AreaGenerateTask
            //        {
            //            generater = GetNextGenerater(),
            //            parms = new AreaGeneraterParms
            //            {
            //                MapQuota = parms.MapQuota - nextAreaSize,
            //                AreaScale = nextAreaSize,
            //                StartPoint = generaterPointList[0]
            //            }
            //        });
            //}

            taskQueue.Enqueue(
                new AreaGenerateTask
                {
                    generater = GetNextGenerater(),
                    parms = new AreaGeneraterParms
                    {
                        MapQuota = 0,
                        AreaScale = parms.MapQuota,
                        StartPoint = generaterPointList[0]
                    }
                });

            generaterPointList.RemoveAt(0);
        }

        private IAreaGenerater GetNextGenerater()
        {
            //int weight = Random.Range(0, 100);

            //if (weight < 50)
            //    return factry.GetBasicAreaGenerater();
            //else
            //    return null;

            return factry.GetBasicAreaGenerater();
        }

        private int GetNextAreaSize()
        {
            return Mathf.Clamp(Random.Range(5, 10), 5, parms.MapQuota);
        }
    }

    /// <summary>
    /// 區域生成任務管理者。
    /// 負責保有並管理生成任務列表、及指定接下來的生成任務。
    /// </summary>
    public class AreaTaskManager : IAreaGenerater
    {
        static Queue<AreaGenerateTask> TaskQueue = new Queue<AreaGenerateTask>();

        public AreaTaskManager(MapGenerateManager generateManager)
            : base(generateManager)
        {
            //Debug.Log("AreaTaskManagerCreate");
        }

        public AreaTaskManager(AreaGenerateTask newTask, MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("AreaTaskManagerCreate");
            TaskQueue.Enqueue(newTask);
        }

        public AreaTaskManager(Queue<AreaGenerateTask> newTasks, MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("AreaTaskManagerCreate");
            while (newTasks.Count > 0)
                TaskQueue.Enqueue(newTasks.Dequeue());
        }

        public override void Update()
        {
            //Debug.Log("AreaTaskManagerUpdate");
            //處理任務列表上的任務
            if (TaskQueue.Count > 0)
            {
                AreaGenerateTask temp = TaskQueue.Dequeue();
                if (!mapBuilder.HasBlock(temp.parms.StartPoint))
                {
                    temp.generater.SetParamater(temp.parms);
                    generaterManager.SetNextGenerater(temp.generater);
                }
            }
            else
            {
                generaterManager.SetNextGenerater(new OutSideFillter(generaterManager, mapBuilder.GetTargets()));
            }
            generaterManager.AddTicks();
        }

        public override void End()
        {
            //Debug.Log("AreaTaskManagerEnd");
            generaterManager.AddTicks();
        }
    }
    #endregion

    #region 區域生成策略
    /// <summary>
    /// 起點區域生成策略。
    /// </summary>
    public class EntryAreaGenerater : IAreaGenerater
    {
        List<Direction> nullBoundary = new List<Direction>(4);
        Queue<Coordinate> generatePoint = new Queue<Coordinate>();

        public EntryAreaGenerater(MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("EntryAreaGeneraterCreate");
        }

        public override void Update()
        {
            //Debug.Log("EntryAreaGeneraterUpdate");
            mapBuilder.MakeBlock(parms.StartPoint, BlockType.Safe);

            for (int d = 0; d < Direction.DirectionCount; d++)
                nullBoundary.Add(d);

            //選擇入口方向
            if (Random.Range(0, 1) == 0)
            {
                mapBuilder.MakeBoundary(parms.StartPoint, Direction.Left, BoundaryType.Entry);
                mapBuilder.MakeBlock(parms.StartPoint + Direction.Left, BlockType.Null);
                nullBoundary.Remove(Direction.Left);
            }
            else
            {
                mapBuilder.MakeBoundary(parms.StartPoint, Direction.Top, BoundaryType.Entry);
                mapBuilder.MakeBlock(parms.StartPoint + Direction.Top, BlockType.Null);
                nullBoundary.Remove(Direction.Top);
            }

            //隨機開口數量並建造邊界
            int pathCount = Random.Range(1, 3);
            for (int ctr = 0; ctr < pathCount; ctr++)
            {
                Direction pathDirection =
                    nullBoundary[Random.Range(0, nullBoundary.Count - 1)];
                nullBoundary.Remove(pathDirection);

                //隨機開口大小
                int pathSize = Random.Range(0, 2);
                if (pathSize == 0)
                    mapBuilder.MakeBoundary(parms.StartPoint, pathDirection, BoundaryType.SmallPath);
                else if (pathSize == 1)
                    mapBuilder.MakeBoundary(parms.StartPoint, pathDirection, BoundaryType.LargePath);
                else
                    mapBuilder.MakeBoundary(parms.StartPoint, pathDirection, BoundaryType.OpenBoundary);

                generatePoint.Enqueue(parms.StartPoint + pathDirection);
            }

            //補上牆壁
            if (nullBoundary.Count > 0)
                foreach (Direction direction in nullBoundary)
                    mapBuilder.MakeBoundary(parms.StartPoint, direction, BoundaryType.Wall);

            //生成結束、轉移至任務管理者
            generaterManager.SetNextGenerater(
                new AreaGeneraterDecider(generatePoint, parms, generaterManager));
        }

        public override void End()
        {
            //Debug.Log("EntryAreaGeneraterEnd");
            generaterManager.AddTicks();
        }
    }

    /// <summary>
    /// 簡單區域生成策略。
    /// </summary>
    public class BasicAreaGenerater : IAreaGenerater
    {
        int nowScale, pathCount, pathMax;
        List<Direction> nullBoundary = new List<Direction>(4);
        bool canMake;

        Queue<Coordinate> generatePoints = new Queue<Coordinate>();
        Queue<Coordinate> tempPoints = new Queue<Coordinate>();
        Coordinate target;

        public BasicAreaGenerater(MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("BasicAreaGeneraterCreate");
        }

        public override void Initail()
        {
            //Debug.Log("BasicAreaGeneraterInitail");
            nowScale = 0;
            canMake = true;

            generatePoints.Enqueue(parms.StartPoint);

            generaterManager.ScaleStartTime = Time.time;
        }

        public override void Update()
        {
            //Debug.Log("BasicAreaGeneraterUpdate, Scale = " + nowScale);
            //直至最大區塊規模或已無可建造區塊
            if (canMake && nowScale < parms.AreaScale)
            {
                //直至該輪所有生成點完成生成
                if (generatePoints.Count > 0)
                {
                    target = generatePoints.Dequeue();

                    //建造區塊並記錄空邊界
                    mapBuilder.MakeBlock(target, BlockType.Normal);
                    pathCount = 0;
                    pathMax = 4;
                    nullBoundary.Clear();

                    for (int d = 0; d < Direction.DirectionCount; d++)
                    {
                        if (!mapBuilder.HasBoundary(target, d))
                            nullBoundary.Add(d);
                        else
                        {
                            if (mapBuilder.HasWall(target, d))
                                pathMax--;
                            else
                                pathCount++;
                        }
                    }

                    //若還有未建造邊界
                    if (nullBoundary.Count > 0)
                    {
                        //隨機開口數量
                        int targetPathCount = 2;
                        int Weighted = Random.Range(0, 100);
                        if (Weighted <= 30)
                            targetPathCount += 1;
                        else if (Weighted <= 5)
                            targetPathCount += 2;

                        if (targetPathCount > pathMax)
                            targetPathCount = pathMax;

                        //計算還需要的開口數量
                        //Debug.Log("已有的開口數量 " + pathCount + " 建造的開口數量 " + targetPathCount + " 還需建造的開口數量 " + (targetPathCount - pathCount) + " 還能建造的邊界數量 " + nullBoundary.Count);
                        pathCount = targetPathCount - pathCount;

                        Direction targetDirectrion;

                        for (int ctr = 0; ctr < pathCount; ctr++)
                        {
                            //隨機開口方向
                            if (nullBoundary.Count > 1)
                                targetDirectrion = Random.Range(nullBoundary.Count, 100) % nullBoundary.Count;
                            else
                                targetDirectrion = 0;

                            //隨機開口大小
                            int pathSize = Random.Range(0, 2);
                            if (pathSize == 0)
                                mapBuilder.MakeBoundary(target, nullBoundary[targetDirectrion], BoundaryType.SmallPath);
                            else if (pathSize == 1)
                                mapBuilder.MakeBoundary(target, nullBoundary[targetDirectrion], BoundaryType.LargePath);
                            else
                                mapBuilder.MakeBoundary(target, nullBoundary[targetDirectrion], BoundaryType.OpenBoundary);

                            if (!tempPoints.Contains(target + nullBoundary[targetDirectrion]))
                                tempPoints.Enqueue(target + nullBoundary[targetDirectrion]);

                            nullBoundary.RemoveAt(targetDirectrion);
                        }

                        //補上牆壁
                        if (nullBoundary.Count > 0)
                            foreach (Direction direction in nullBoundary)
                                mapBuilder.MakeBoundary(target, direction, BoundaryType.Wall);

                        GameObject spwanPoint;
                        if (mapBuilder.TryGetRandomedSpwanPoint(nowScale, out spwanPoint))
                            mapBuilder.SetSpwanPoint(target, spwanPoint);
                    }
                }
                else
                {
                    /*Debug.Log("Scale " + nowScale + " : "
                        + (Time.time - generaterManager.ScaleStartTime) + " secends, Total "
                        + (Time.time - generaterManager.StartTime) + " secends");
                    generaterManager.ScaleStartTime = Time.time;*/

                    //轉移暫存列表、準備次輪生成
                    nowScale++;
                    generatePoints = tempPoints;
                    tempPoints = new Queue<Coordinate>();
                }

                //檢查是否還有生成空間
                if (tempPoints.Count <= 0 && generatePoints.Count <= 0)
                    canMake = false;

                generaterManager.AddTicks();
            }
            else
            {
                generaterManager.SetNextGenerater(
                    new AreaGeneraterDecider(generatePoints, parms, generaterManager));
            }
        }

        public override void End()
        {
            //Debug.Log("BasicAreaGeneraterEnd");
            generaterManager.AddTicks();
        }
    }

    /// <summary>
    /// 王房生成策略
    /// </summary>
    public class BossRoomGenerater : IAreaGenerater
    {
        int nowScale, pathCount, pathMax, blockCount;
        List<Direction> nullBoundary = new List<Direction>(4);
        bool canMake;

        List<Coordinate> generatePoints = new List<Coordinate>();
        List<Coordinate> tempPoints = new List<Coordinate>();
        Coordinate target;

        public BossRoomGenerater(MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("BasicAreaGeneraterCreate");
        }

        public override void Initail()
        {
            //Debug.Log("BasicAreaGeneraterInitail");
            nowScale = 0;
            blockCount = 1;
            canMake = true;

            generatePoints.Add(parms.StartPoint);

            generaterManager.ScaleStartTime = Time.time;
        }

        public override void Update()
        {
            //Debug.Log("BasicAreaGeneraterUpdate, Scale = " + nowScale);
            //直至最大區塊規模或已無可建造區塊
            if (canMake && nowScale < parms.AreaScale)
            {
                //直至該輪所有生成點完成生成
                if (generatePoints.Count > 0 && ++blockCount < 8)
                {
                    target = generatePoints[Random.Range(0, generatePoints.Count - 1)];
                    generatePoints.Remove(target);

                    //建造區塊並記錄空邊界
                    mapBuilder.MakeBlock(target, BlockType.BossRoom);
                    pathCount = 0;
                    pathMax = 4;
                    nullBoundary.Clear();

                    for (int d = 0; d < Direction.DirectionCount; d++)
                    {
                        if (!mapBuilder.HasBoundary(target, d))
                            nullBoundary.Add(d);
                        else
                        {
                            if (mapBuilder.HasWall(target, d))
                                pathMax--;
                            else
                                pathCount++;
                        }
                    }

                    //若還有未建造邊界
                    if (nullBoundary.Count > 0)
                    {
                        //隨機開口數量
                        int targetPathCount;
                        int Weighted = Random.Range(0, 100);
                        if (Weighted <= 10)
                            targetPathCount = 2;
                        else if (Weighted <= 40)
                            targetPathCount = 3;
                        else
                            targetPathCount = 4;

                        if (targetPathCount > pathMax)
                            targetPathCount = pathMax;

                        //計算還需要的開口數量
                        //Debug.Log("已有的開口數量 " + pathCount + " 建造的開口數量 " + targetPathCount + " 還需建造的開口數量 " + (targetPathCount - pathCount) + " 還能建造的邊界數量 " + nullBoundary.Count);
                        pathCount = targetPathCount - pathCount;

                        Direction targetDirectrion;

                        for (int ctr = 0; ctr < pathCount; ctr++)
                        {
                            //隨機開口方向
                            if (nullBoundary.Count > 1)
                                targetDirectrion = Random.Range(nullBoundary.Count, 100) % nullBoundary.Count;
                            else
                                targetDirectrion = 0;

                            mapBuilder.MakeBoundary(target, nullBoundary[targetDirectrion], BoundaryType.OpenBoundary);

                            if (!tempPoints.Contains(target + nullBoundary[targetDirectrion]))
                                tempPoints.Add(target + nullBoundary[targetDirectrion]);

                            nullBoundary.RemoveAt(targetDirectrion);
                        }

                        //補上牆壁
                        if (nullBoundary.Count > 0)
                            foreach (Direction direction in nullBoundary)
                                mapBuilder.MakeBoundary(target, direction, BoundaryType.Wall);
                    }
                }
                else if (blockCount < 8)
                {
                    /*Debug.Log("Scale " + nowScale + " : "
                        + (Time.time - generaterManager.ScaleStartTime) + " secends, Total "
                        + (Time.time - generaterManager.StartTime) + " secends");
                    generaterManager.ScaleStartTime = Time.time;*/

                    //轉移暫存列表、準備次輪生成
                    nowScale++;
                    generatePoints = tempPoints;
                    tempPoints = new List<Coordinate>();
                }

                //檢查是否還有生成空間
                if ((tempPoints.Count == 0 && generatePoints.Count == 0) || blockCount >= 8)
                {
                    canMake = false;
                    generatePoints.AddRange(tempPoints);
                }

                generaterManager.AddTicks();
            }
            else
            {
                generaterManager.SetNextGenerater(
                    new AreaGeneraterDecider(generatePoints, new AreaGeneraterParms
                    {
                        MapQuota = 0
                    }, generaterManager));
            }
        }

        public override void End()
        {
            //Debug.Log("BasicAreaGeneraterEnd");
            generaterManager.AddTicks();
        }
    }

    /// <summary>
    /// 區域閉鎖策略。
    /// </summary>
    public class AreaSealder : IAreaGenerater
    {
        public AreaSealder(MapGenerateManager generaterManager)
            : base(generaterManager)
        {
            //Debug.Log("AreaSealderCreate");
        }

        public override void Update()
        {
            //Debug.Log("AreaSealderUpdate");
            mapBuilder.MakeBlock(parms.StartPoint, BlockType.Normal);

            for (int d = 0; d < Direction.DirectionCount; d++)
                if (!mapBuilder.HasBoundary(parms.StartPoint, d))
                    mapBuilder.MakeBoundary(parms.StartPoint, d, BoundaryType.Wall);

            generaterManager.SetNextGenerater(
                new AreaTaskManager(generaterManager));

            /*Debug.Log("Sealed point(" + parms.StartPoint.Column + 
                "," + parms.StartPoint.Row + "): " + (Time.time - generaterManager.ScaleStartTime) + 
                " secends, Total "  + (Time.time - generaterManager.StartTime) + " secends");*/
            generaterManager.ScaleStartTime = Time.time;
        }

        public override void End()
        {
            //Debug.Log("AreaSealderEnd");
            generaterManager.AddTicks();
        }
    }

    /// <summary>
    /// 外圍填充規則
    /// </summary>
    public class OutSideFillter : IAreaGenerater
    {
        static Queue<Coordinate> targets;

        public OutSideFillter(MapGenerateManager generaterManager, Queue<Coordinate> targets) : base(generaterManager)
        {
            OutSideFillter.targets = targets;
        }

        public OutSideFillter(MapGenerateManager generaterManager) : base(generaterManager)
        {
        }

        public override void Update()
        {
            if (targets.Count > 0)
            {
                Coordinate target = targets.Dequeue();

                for (int column = -1; column < 2; column++)
                    for (int row = -1; row < 2; row++)
                    {
                        Coordinate temp = new Coordinate(target.Column + column, target.Row + row);
                        if (!mapBuilder.HasBlock(temp))
                            mapBuilder.MakeBlock(temp, BlockType.Null);
                    }
            }
            else
                generaterManager.SetNextGenerater(new TerrainGenerateInitail(generaterManager));

            generaterManager.AddTicks();
        }
    }
    #endregion
}