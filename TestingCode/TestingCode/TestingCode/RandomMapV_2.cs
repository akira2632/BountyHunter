using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingCode
{
    class RandomMapV_2
    {
        public void Run()
        {
        }
    }

    #region 隨機生成演算法

    #endregion

    #region 地圖方塊
    //方向定義
    enum Direction { Top, Dowm, Left, Right }

    //地圖牆面
    abstract class WallType { }
    class Null : WallType { }
    class Open : WallType { }
    class Close : WallType { }

    //基本地圖區塊
    class MapBlock
    {
        WallType[] myWalls;

        public MapBlock()
        {
            myWalls = new WallType[4];

            for (int i = 0; i < 4; i++)
                myWalls[i] = new Null();
        }

        public void SetWall(Direction direction, WallType type)
        {
            myWalls[(int)direction] = type;
        }

        public WallType GetWall(Direction direction)
        {
            return myWalls[(int)direction];
        }

        public bool HasWall(Direction direction)
        {
            return !(myWalls[(int)direction] is Null);
        }

        public bool IsSealed()
        {
            foreach (WallType type in myWalls)
                if (type is Null)
                    return false;

            return true;
        }
    }
    #endregion
}
