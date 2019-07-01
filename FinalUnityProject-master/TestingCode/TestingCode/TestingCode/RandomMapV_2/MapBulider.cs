using System;

namespace RandomMapV_2
{
    public class MapBulider
    {
        private MapBlock[,] map;

        public MapBulider(MapBlock[,] map)
        {
            this.map = map;
        }
        
        public bool NotIndexOut(int column, int row)
        {
            return column >= 0 && column < map.GetLength(0)
                && row >= 0 && row < map.GetLength(1);
        }

        public bool HasBlock(int column, int row)
        {
                return map[column, row] != null;
        }

        public bool HasWall(int column, int row, Direction d)
        {
                return map[column, row].walls[(int)Direction] != WallType.Null;
        }

        public bool Passeble(int column, int row, Direction d)
        {

        }
    }
}
