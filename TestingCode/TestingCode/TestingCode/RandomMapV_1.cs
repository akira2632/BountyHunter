using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingCode
{
    class RandomMapV_1
    {
        //程式狀態
        enum State { getScale, generateMap, ShowMap, endProgram }
        State nowState;

        ConsoleColor[,] map;
        int scale;

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

        void GenerateMap(int mapScale, out ConsoleColor[,] map)
        {
            int maxSize = mapScale * 2 - 1;
            map = new ConsoleColor[maxSize, maxSize];

            #region Initialze empty map
            for (int column = 0; column < maxSize; column++)
                for (int row = 0; row < maxSize; row++)
                    map[column, row] = ConsoleColor.Black;
            #endregion

            #region Generate Map
            Random random = new Random();

            MakeBlock(mapScale, mapScale, mapScale - 1, ref map, ref random);
            map[mapScale, mapScale] = ConsoleColor.Blue;
            #endregion
        }

        void MakeBlock(int column, int row, int mapScale, ref ConsoleColor[,] map, ref Random random)
        {
            map[column, row] = ConsoleColor.Green;
            //Console.Clear(); ShowMap(map); Console.ReadKey();

            if (mapScale > 0)
            {
                int hasBlock = 0;
                bool hasMake = false;

                while (hasBlock < 4 && !hasMake)
                {
                    hasBlock = 0;
                    if (map[column + 1, row] == ConsoleColor.Green)
                        hasBlock++;
                    else if (map[column + 1, row] == ConsoleColor.Black && random.Next(4) == 1)
                    {
                        hasMake = true;
                        MakeBlock(column + 1, row, mapScale - 1, ref map, ref random);
                    }

                    if (map[column - 1, row] == ConsoleColor.Green)
                        hasBlock++;
                    else if (map[column - 1, row] == ConsoleColor.Black && random.Next(4) == 1)
                    {
                        hasMake = true;
                        MakeBlock(column - 1, row, mapScale - 1, ref map, ref random);
                    }

                    if (map[column, row + 1] == ConsoleColor.Green)
                        hasBlock++;
                    else if (map[column, row + 1] == ConsoleColor.Black && random.Next(4) == 1)
                    {
                        hasMake = true;
                        MakeBlock(column, row + 1, mapScale - 1, ref map, ref random);
                    }

                    if (map[column, row - 1] == ConsoleColor.Green)
                        hasBlock++;
                    else if (map[column, row - 1] == ConsoleColor.Black && random.Next(4) == 1)
                    {
                        hasMake = true;
                        MakeBlock(column, row - 1, mapScale - 1, ref map, ref random);
                    }
                }
            }
        }

        void ShowMap(ConsoleColor[,] map)
        {
            for (int column = 0; column < map.GetLength(0); column++)
            {
                for (int row = 0; row < map.GetLength(1); row++)
                {
                    Console.BackgroundColor = map[column, row];
                    Console.Write("  ");
                }
                Console.WriteLine();
            }
        }

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
    }
}
