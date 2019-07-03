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

        struct Coordinate
        {
            public Coordinate(int column, int row)
            {
                this.column = column;
                this.row = row;
            }

            public int column;
            public int row;
        }

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
            int maxSize = mapScale * 2 + 1;
            map = new ConsoleColor[maxSize, maxSize];

            //Initialze empty map
            for (int column = 0; column < maxSize; column++)
                for (int row = 0; row < maxSize; row++)
                    map[column, row] = ConsoleColor.Black;

            //Generate Map
            Random random = new Random();
            List<Coordinate> GeneratePoint = new List<Coordinate>();
            GeneratePoint.Add(new Coordinate(mapScale, mapScale));

            while (mapScale > 0)
            {
                for(int ctr = 0; ctr < GeneratePoint.Count; ctr++)
                {
                    bool hasMake = false;
                    int hasBlock = 0;
                    Coordinate target = GeneratePoint[ctr];

                    while (!hasMake)
                    {
                        if (map[target.column + 1, target.row] == ConsoleColor.Green)
                            hasBlock++;
                        else if (random.Next(4) == 1)
                        {
                            map[target.column + 1, target.row] == ConsoleColor.Green;
                            GeneratePoint.Add(new Coordinate(target.column + 1, target.row));
                            hasMake = true;
                        }

                        if (map[target.column - 1, target.row] == ConsoleColor.Green)
                            hasBlock++;
                        else if (random.Next(4) == 1)
                        {
                            map[target.column - 1, target.row] == ConsoleColor.Green;
                            GeneratePoint.Add(new Coordinate(target.column - 1, target.row));
                            hasMake = true;
                        }

                        if (map[target.column, target.row + 1] == ConsoleColor.Green)
                            hasBlock++;
                        else if (random.Next(4) == 1)
                        {
                            map[target.column, target.row + 1] == ConsoleColor.Green;
                            GeneratePoint.Add(new Coordinate(target.column, target.row + 1));
                            hasMake = true;
                        }

                        if (map[target.column, target.row - 1] == ConsoleColor.Green)
                            hasBlock++;
                        else if (random.Next(4) == 1)
                        {
                            map[target.column, target.row - 1] == ConsoleColor.Green;
                            GeneratePoint.Add(new Coordinate(target.column, target.row - 1));
                            hasMake = true;
                        }

                        if (hasBlock >= 4)
                            return;
                    }
                    GeneratePoint.Remove(GeneratePoint.FindIndex(target));
                }

                mapScale--;
            }

            map[mapScale, mapScale] = ConsoleColor.Blue;
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