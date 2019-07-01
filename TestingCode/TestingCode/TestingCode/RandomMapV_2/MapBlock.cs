using System;

namespace RandomMapV_2
{
    public class MapBlcok
    {
        private WallType[] walls;

        public MapBlcok()
        {
            walls = new WallType[4];
            for (int i = 0; i < 4; i++)
                walls[i] = WallType.Null;
        }

        public void SetWall(Direction direction, WallType wall)
        {
            walls[(int)direction] = wall;
        }
    }
}
