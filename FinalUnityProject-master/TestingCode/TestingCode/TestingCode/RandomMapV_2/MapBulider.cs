using System;
namespace RandomMapV_2
{
    public class MapBulider
    {
        #region 變數
        private MapBlock[,] map;
        #endregion

        public MapBulider(MapBlock[,] map)
        {
            this.map = map;
        }

        #region 私有方法
        private bool NotIndexOut(int column, int row)
        {
            return column >= 0 && column < map.GetLength(0)
                && row >= 0 && row < map.GetLength(1);
        }
        #endregion

        #region 檢查方法
        public bool HasBlock(int column, int row)
        {
            if (NotIndexOut(column, row))
                return map[column, row] != null;
            else
                return true;
        }

        public bool HasWall(int column, int row, Direction d)
        {
            if (NotIndexOut(column, row))
                return map[column, row].wall[(int)Direction] != WallType.Null;
        }
        #endregion
    }
}
