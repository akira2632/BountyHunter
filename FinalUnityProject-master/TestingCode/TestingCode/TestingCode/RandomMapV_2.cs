using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingCode
{
    class RandomMapV_2
    {
        //程式狀態
        enum State { getScale, generateMap, ShowMap, endProgram }
        State nowState;

        MapBlock[,] map;
        int scale;

        #region 控制器程式
        int GetScale()
        {
            string userInput;
            while (true)
            {
                Console.Clear();
                Console.Write("Entry Scale : ");
                userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int scale))
                {
                    if (scale < 1)
                        Console.Write("Scale should greater than 1");
                    else
                        return scale;
                }
                else
                    Console.Write("Input should be an integer");

                Console.ReadKey();
            }
        }

        void ShowMap(MapBlock[,] map)
        {
            for (int column = 0; column < map.GetLength(0); column++)
            {
                for (int row = 0; row < map.GetLength(1); row++)
                {
                    if(map[column, row] != null)
                        map[column,row].ShowMap();
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine("  ");
                    }
                }
                Console.WriteLine();
            }
        }
        #endregion

        public void Run()
        {
            Console.SetWindowSize(160, 80);
            
            nowState = State.getScale;

            while (true)
            {
                Console.Clear();
                switch (nowState)
                {
                    case State.getScale:
                        scale = GetScale();
                        nowState = State.generateMap;
                        break;
                    case State.generateMap:
                        GenerateMap(scale, out map);
                        nowState = State.ShowMap;
                        break;
                    case State.ShowMap:
                        ShowMap(map);

                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.N:
                                nowState = State.getScale;
                                break;
                            case ConsoleKey.R:
                                nowState = State.generateMap;
                                break;
                            case ConsoleKey.Escape:
                                nowState = State.endProgram;
                                break;
                            default:
                                break;
                        }
                        break;
                    case State.endProgram:
                        return;
                }
            }
        }

        #region 隨機生成演算法
        void GenerateMap(int mapScale, out MapBlock[,] map)
        {
            int maxSize = mapScale * 2 - 1;
            map = new MapBlock[maxSize, maxSize];
            #region Generate Map
            Random random = new Random();

            MakeBlock(mapScale, mapScale, mapScale - 1, ref map, ref random);
            #endregion
        }

        void MakeBlock(int column, int row, int mapScale, ref MapBlock[,] map, ref Random random)
        {
            map[column, row] = new MapBlock();

            if (mapScale > 0)
            {
                int hasBlock = 0;
                bool hasMake = false;

                while (hasBlock < 4 && !hasMake)
                {
                    hasBlock = 0;
                    if(map[column])
                }

                //Console.Clear(); ShowMap(map); Console.ReadKey();
            }
        }
        #endregion
    }

    enum State { getScale, generateMap, ShowMap, endProgram }

    #region 地圖方塊
    //方向定義
    sealed class Direction
    {
        private int _x, _y, _id;

        public int X { get => _x; }
        public int Y { get => _y; }
        public int Id { get => _id; }

        private Direction(int x, int y, int id)
        {
            _x = x;
            _y = y;
            _id = id;
        }
        
        public static readonly Direction Top = new Direction(0,1,0);
        public static readonly Direction Bottom = new Direction(0, -1,1);
        public static readonly Direction Left = new Direction(1, 0,2);
        public static readonly Direction Right = new Direction(-1, 0,3);
    }

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
            myWalls[direction.Id] = type;
        }

        public WallType GetWall(Direction direction)
        {
            return myWalls[direction.Id];
        }

        public bool HasWall(Direction direction)
        {
            return !(myWalls[direction.Id] is Null);
        }

        public bool IsSealed()
        {
            foreach (WallType type in myWalls)
                if (type is Null)
                    return false;

            return true;
        }

        public void ShowMap()
        {

        }
    }
    #endregion
}
