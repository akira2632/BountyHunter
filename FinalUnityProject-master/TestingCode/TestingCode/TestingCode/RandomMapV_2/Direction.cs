using System;

namespace RandomMapV_2
{
    sealed class Direction
    {
        private int _column, _row, _id;

        public int Column { get => _column; }
        public int Row { get => _row; }
        public int ID { get => _id; }

        private Direction(int column, int row, int id)
        {
            _column = column;
            _row = row;
            _id = id;
        }

        public static explicit operator Direction(int i)
        {
            switch (i)
            {
                case 0:
                    return Top;
                case 1:
                    return Bottom;
                case 2:
                    return Left;
                case 3:
                    return Right;
                default:
                    throw new System.ArgumentException();
                    break;
            }
        }

        public static readonly Direction Top = new Direction(0, 1, 0);
        public static readonly Direction Bottom = new Direction(0, -1, 1);
        public static readonly Direction Left = new Direction(1, 0, 2);
        public static readonly Direction Right = new Direction(-1, 0, 3);
    }

    enum WallType { Null = -1, Wall, Pass }
}
