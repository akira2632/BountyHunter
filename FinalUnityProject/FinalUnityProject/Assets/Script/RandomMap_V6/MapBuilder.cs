using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace RandomMap_V6
{
    public class GeneraterFactry
    {
        //建造所需的參數
        MapGenerateManager manager;

        MapBuilder mapBuilder;
        MapPrinter mapPrinter;
        EntryAreaGenerater entryAreaGenerater;
        BasicAreaGenerater basicAreaGenerater;
        BossRoomGenerater bossRoomGenerater;
        AreaSealder areaSealder;

        public GeneraterFactry(MiniMapSetting miniMapSetting, 
            GameMapSetting gameMapSetting,
            SpwanPointSetting spwanPointSetting,
            MapGenerateManager manager)
        {
            this.manager = manager;
            mapBuilder = new MapBuilder(spwanPointSetting);
            mapPrinter = new MapPrinter(miniMapSetting, gameMapSetting);
        }

        public MapBuilder GetBuilder() => mapBuilder;
        public MapPrinter GetMapPrinter() => mapPrinter;

        #region AreaGenerater 建造區域生成策略
        public EntryAreaGenerater GetEntryAreaGenerater()
        {
            if (entryAreaGenerater == null)
                entryAreaGenerater = new EntryAreaGenerater(manager);

            return entryAreaGenerater;
        }

        public BasicAreaGenerater GetBasicAreaGenerater()
        {
            if (basicAreaGenerater == null)
                basicAreaGenerater = new BasicAreaGenerater(manager);

            return basicAreaGenerater;
        }

        public BossRoomGenerater GetBossRoomGenerater()
        {
            if (bossRoomGenerater == null)
                bossRoomGenerater = new BossRoomGenerater(manager);

            return bossRoomGenerater;
        }

        public AreaSealder GetAreaSealder()
        {
            if (areaSealder == null)
                areaSealder = new AreaSealder(manager);

            return areaSealder;
        }
        #endregion
    }

    public class MapBuilder
    {
        SpwanPointSetting spwanPointSetting;
        Dictionary<Coordinate, MapBlock> map = new Dictionary<Coordinate, MapBlock>();
        List<GameObject> selected;

        public MapBuilder(SpwanPointSetting spwanPointSetting)
        {
            selected = new List<GameObject>();
            this.spwanPointSetting = spwanPointSetting;
        }

        #region AreaBuilder 區域建造者
        #region 檢查方法
        /// <summary>
        /// 檢查目標座標上是否有區塊
        /// </summary>
        /// <param name="target">目標座標</param>
        public bool HasBlock(Coordinate target)
        {
            return map.ContainsKey(target);
        }

        /// <summary>
        /// 檢查目標座標方向上是否有邊界
        /// </summary>
        /// <param name="target">目標座標</param>
        /// <param name="direction">目標方向</param>
        public bool HasBoundary(Coordinate target, Direction direction)
        {
            return !(map[target].boundarys[direction] == null);
        }

        /// <summary>
        /// 檢查目標座標方向上是否有牆壁
        /// </summary>
        /// <param name="target">目標座標</param>
        /// <param name="direction">目標方向</param>
        public bool HasWall(Coordinate target, Direction direction)
        {
            return map[target].boundarys[direction].Type == BoundaryType.Wall;
        }

        internal bool HasOpenBoundary(Coordinate target, Direction direction)
        {
            return map[target].boundarys[direction].Type == BoundaryType.OpenBoundary;
        }

        internal bool HasEmptyArea(Coordinate coordinate, int nextAreaSize)
        {
            List<Coordinate> EmptyBlocks = new List<Coordinate>();
            List<Coordinate> Temp = new List<Coordinate>();
            EmptyBlocks.Add(coordinate);

            for (int i = 0; i < nextAreaSize; i++)
            {
                foreach (Coordinate point in EmptyBlocks)
                    for (int d = 0; d < Direction.DirectionCount; d++)
                        if (!Temp.Contains(point + d) && !map.ContainsKey(point + d))
                            Temp.Add(point + d);

                EmptyBlocks = Temp;
                Temp = new List<Coordinate>();
            }

            return EmptyBlocks.Count * 2 > nextAreaSize;
        }
        #endregion

        #region 建造方法
        /// <summary>
        /// 於目標座標上建造新區塊
        /// </summary>
        /// <param name="target">目標座標</param>
        /// <param name="type">區塊類型</param>
        public void MakeBlock(Coordinate target, BlockType type)
        {
            map.Add(target, new MapBlock(type));

            for (int d = 0; d < Direction.DirectionCount; d++)
            {
                if (map.ContainsKey(target + d))
                {
                    map[target].boundarys[d] = map[target + d].boundarys[Direction.Reverse(d)];
                    map[target + d].boundarys[Direction.Reverse(d)].NextBLock = map[target];
                }
                else if (type == BlockType.Null)
                    MakeBoundary(target, d, BoundaryType.Wall);
            }
        }

        /// <summary>
        /// 於目標座標方向上建造邊界
        /// </summary>
        /// <param name="target">目標座標</param>
        /// <param name="direction">目標方向</param>
        /// <param name="boundaryType">邊界類型</param>
        public void MakeBoundary(Coordinate target, Direction direction, BoundaryType boundaryType)
        {
            map[target].boundarys[direction] =
                new Boundary(map[target], boundaryType);
        }
        #endregion
        #endregion

        #region TerrainBuilder 地形建造者
        #region 取得資訊 & 檢查方法
        /// <summary>
        /// 回傳目前地圖中所有區塊的座標隊列
        /// </summary>
        public Queue<Coordinate> GetTargets()
        {
            return new Queue<Coordinate>(map.Keys);
        }

        /// <summary>
        /// 回傳目標區域的類型
        /// </summary>
        /// <param name="target">目標座標</param>
        /// <returns>區域類型</returns>
        public BlockType GetBlockType(Coordinate target)
        {
            return map[target].blockType;
        }

        /// <summary>
        /// 回傳目標區域方向上的邊界類型
        /// </summary>
        /// <param name="target">目標座標</param>
        /// <param name="d">目標方向</param>
        /// <returns>邊界類型</returns>
        public BoundaryType GetBoundaryType(Coordinate target, Direction d)
        {
            return map[target].boundarys[d].Type;
        }

        public bool HasSetBoudary(Coordinate target, Direction d)
        {
            return map[target].boundarys[d].HasSet;
        }
        #endregion

        #region 建造方法
        public void SetBoundaryTerrain(Coordinate target, Direction d)
        {
            if (!(map[target].boundarys[d].Type == BoundaryType.Entry))
                SetBoundaryTerrain(target, d, map[target].boundarys[d].Size, map[target].boundarys[d].Offset);
            else
                SetBoundaryTerrain(target, d, 0, 0);
        }

        public void SetBoundaryTerrain(Coordinate target, Direction direction, int size, int offset)
        {
            if (!map[target].boundarys[direction].HasSet)
            {
                map[target].boundarys[direction].Size = size;
                map[target].boundarys[direction].Offset = offset;
                map[target].boundarys[direction].HasSet = true;
            }

            //找出方向起點與方向偏移量
            int startColumn = direction.Column == 1 ? 14 : 0;
            int startRow = direction.Row == 1 ? 14 : 0;
            int columnSideDisp = direction.Row != 0 ? 1 : 0;
            int rowSideDisp = direction.Column != 0 ? 1 : 0;

            Direction centerDirection = Direction.Reverse(direction);
            int leftWallEnd, rightWallStart, leftSideDisp, rightSideDisp;

            //設定左邊牆面的結束位置
            if (map[target].boundarys[direction].Type == BoundaryType.OpenBoundary
            && map[target].boundarys[Direction.LeftSide(direction)].Type == BoundaryType.OpenBoundary
            && map.ContainsKey(target + direction + Direction.LeftSide(direction))
            && map[target + direction + Direction.LeftSide(direction)].boundarys[Direction.Reverse(direction)].Type == BoundaryType.OpenBoundary
            && map[target + direction + Direction.LeftSide(direction)].boundarys[Direction.RightSide(direction)].Type == BoundaryType.OpenBoundary)
                leftWallEnd = 0;
            else
                //leftWallEnd = 7 - (size / 2) + offset;
                leftWallEnd = (8 - (size / 2) - size % 2) + offset;
            //計算通道左邊邊界至中心的偏移量
            leftSideDisp = 6 - leftWallEnd;

            //設定右邊牆面的起始位置
            if (map[target].boundarys[direction].Type == BoundaryType.OpenBoundary
            && map[target].boundarys[Direction.RightSide(direction)].Type == BoundaryType.OpenBoundary
            && map.ContainsKey(target + direction + Direction.RightSide(direction))
            && map[target + direction + Direction.RightSide(direction)].boundarys[Direction.Reverse(direction)].Type == BoundaryType.OpenBoundary
            && map[target + direction + Direction.RightSide(direction)].boundarys[Direction.LeftSide(direction)].Type == BoundaryType.OpenBoundary)
                rightWallStart = 14;
            else
                rightWallStart = 7 + (size / 2) + offset;
            //計算通道右邊邊界至中心的偏移量
            rightSideDisp = 9 - rightWallStart;

            int ToCenterDisp = map[target].boundarys[direction].Type == BoundaryType.Wall ? 1 : 6;

            //鋪上固定的牆壁與地面
            //自邊緣至中心
            for (int CenterDispValue = 0; CenterDispValue < ToCenterDisp; CenterDispValue++)
            {
                //自左側到右側
                for (int SideDispValue = 0; SideDispValue < 15; SideDispValue++)
                {
                    if (SideDispValue >= leftWallEnd + ((CenterDispValue * leftSideDisp) / 5)
                        && SideDispValue <= rightWallStart + ((CenterDispValue * rightSideDisp) / 5))
                    {
                        map[target].terrain[
                            startColumn + (columnSideDisp * SideDispValue) + (CenterDispValue * centerDirection.Column)
                            , startRow + (rowSideDisp * SideDispValue) + (CenterDispValue * centerDirection.Row)] = 0;
                    }
                    else if (CenterDispValue == 0)
                    {
                        map[target].terrain[startColumn + (columnSideDisp * SideDispValue)
                            , startRow + (rowSideDisp * SideDispValue)] = 10;
                    }
                }
            }
        }

        public void SetBassTerrain(Coordinate target)
        {
            //中心區域
            for (int column = 6; column < 9; column++)
                for (int row = 6; row < 9; row++)
                    map[target].terrain[column, row] = 0;
        }

        public ref sbyte[,] GetTerrainData(Coordinate target)
        {
            return ref map[target].terrain;
        }
        #endregion
        #endregion

        #region 生成點相關
        public void SetSpwanPoint(Coordinate target, GameObject spwanPoint)
        {
            map[target].spwanPoint = spwanPoint;
        }

        public GameObject GetSpwanPoint(Coordinate target)
        {
            return map[target].spwanPoint;
        }

        public bool TryGetRandomedSpwanPoint(int targetScale, out GameObject spwanPoint)
        {
            spwanPoint = null;
            selected.Clear();

            foreach (SpwanPointPrafeb item in spwanPointSetting.SpwanPoints)
                if (targetScale >= item.FromMapDeep && targetScale <= item.ToMapDeep)
                    selected.Add(item.SpwanPoint);

            if (selected.Count > 0)
            {
                spwanPoint = selected[Random.Range(0, selected.Count - 1)];
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameObject GetBossRoomSpwanPoint()
        {
            return spwanPointSetting.BossSpwanPoint.SpwanPoint;
        }
        #endregion
    }

    public class MapPrinter
    {
        private Color entryColor, safeBlockColor;
        private MiniMapSetting miniMapSetting;
        private GameMapSetting gameMapSetting;
        private List<GameObject> spwanPoints;

        public MapPrinter(MiniMapSetting miniMapSetting, GameMapSetting gameMapSetting)
        {
            this.miniMapSetting = miniMapSetting;
            this.gameMapSetting = gameMapSetting;
            //bossRoomColor = new Color32(255, 240, 109, 255);
            entryColor = new Color32(243, 105, 228, 255);
            safeBlockColor = new Color32(251, 172, 235, 255);
            spwanPoints = new List<GameObject>();
        }

        #region 印出遊戲地圖
        public void PrintGameMapGround(int x, int y)
        {
            gameMapSetting.GameMap_Ground.SetTile(new Vector3Int(x, y, 0), gameMapSetting.GameMapGround);
        }

        public void PrintGameMapWall(int x, int y)
        {
            gameMapSetting.GameMap_Wall.SetTile(new Vector3Int(x, y, 0), gameMapSetting.GameMapWall);
        }

        public void PrintWallDecorates(int random, int x, int y)
        {
            gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0),
                gameMapSetting.WallDecotates[random % gameMapSetting.WallDecotates.Length]);
        }

        public void PrintGroundDecorates(int random, int x, int y)
        {
            gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0),
                gameMapSetting.GroundDecorates[random % gameMapSetting.GroundDecorates.Length]);
        }

        public void PrintBoxDecorates(int random, int x, int y)
        {
            gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0),
               gameMapSetting.BoxDecotates[random % gameMapSetting.BoxDecotates.Length]);
        }

        public void PRintSkullDecorates(int random, int x, int y)
        {
            gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0),
               gameMapSetting.SkullDecotates[random % gameMapSetting.SkullDecotates.Length]);
        }

        internal void PrintGameMapEntry(int x, int y, Direction d)
        {
            if (d == Direction.Left)
                gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0)
                    , gameMapSetting.TopLeftEntry);
            else if (d == Direction.Right)
                gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0)
                    , gameMapSetting.BottomRightEntry);
            else if (d == Direction.Top)
                gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0)
                    , gameMapSetting.TopRightEntry);
            else if (d == Direction.Bottom)
                gameMapSetting.GameMap_Decorate.SetTile(new Vector3Int(x, y, 0)
                    , gameMapSetting.BottomLeftEntry);
        }
        /*
        public void BossRoom(bool isBossRoom)
        {
            if(isBossRoom)
            {
                gameMapSetting.GameMapGround.color = bossRoomColor;
                gameMapSetting.GameMapWall.color = bossRoomColor;
            }
            else
            {
                gameMapSetting.GameMapGround.color = Color.white;
                gameMapSetting.GameMapWall.color = Color.white;
            }
        }*/
        #endregion

        #region 印出小地圖
        public void PrintMiniMapCorner(Coordinate target, Direction direction1, Direction direction2)
        {
            int columnDisp = direction1.Column + direction2.Column > 0 ? 1 : 0;
            int rowDisp = direction1.Row + direction2.Row > 0 ? 1 : 0;

            miniMapSetting.MiniMap.SetTile(
                new Vector3Int(target.Column * 15 + columnDisp * 14, target.Row * 15 + rowDisp * 14, 0)
                , miniMapSetting.MiniMapWall);
        }

        public void PrintMiniMapWall(Coordinate target, Direction direction)
        {
            int startColumn = direction.Column == 1 ? 14 : 0;
            int startRow = direction.Row == 1 ? 14 : 0;
            int columnDisp = direction.Row != 0 ? 1 : 0;
            int rowDisp = direction.Column != 0 ? 1 : 0;

            for (int column = 1; column < 14; column++)
                for (int row = 1; row < 14; row++)
                    miniMapSetting.MiniMap.SetTile(
                        new Vector3Int(target.Column * 15 + startColumn + columnDisp * column,
                        target.Row * 15 + startRow + rowDisp * row, 0)
                        , miniMapSetting.MiniMapWall);
        }

        public void PrintMiniMapEntry(Coordinate target, Direction direction)
        {
            miniMapSetting.MiniMapWall.color = entryColor;
            PrintMiniMapWall(target, direction);
            miniMapSetting.MiniMapWall.color = Color.white;
        }

        internal void PrintMiniMapSafeBlockWall(Coordinate target, Direction direction)
        {
            miniMapSetting.MiniMapWall.color = safeBlockColor;
            PrintMiniMapWall(target, direction);
            miniMapSetting.MiniMapWall.color = Color.white;
        }

        internal void PrintMiniMapSafeBlockCorner(Coordinate target, Direction direction1, Direction direction2)
        {
            miniMapSetting.MiniMapWall.color = safeBlockColor;
            PrintMiniMapCorner(target, direction1, direction2);
            miniMapSetting.MiniMapWall.color = Color.white;
        }
        #endregion

        public void SetSpwanPoint(int x, int y, GameObject spwanPoint)
        {
            var spwanPointPosition = gameMapSetting.GameMap_Ground.CellToWorld(new Vector3Int(x, y, 0));
            var newSpwanPoint = GameObject.Instantiate(spwanPoint, spwanPointPosition, Quaternion.identity);
            spwanPoints.Add(newSpwanPoint);
        }

        public void ActiveSpwanPoint()
        {
            foreach (GameObject item in spwanPoints)
            {
                item.SetActive(true);
            }
        }

        internal void GetEntryPosition(out float x, out float y)
        {
            var temp = gameMapSetting.GameMap_Ground.CellToLocal(new Vector3Int(8, 8, 0));
            x = temp.x;
            y = temp.y;
        }
    }
}
