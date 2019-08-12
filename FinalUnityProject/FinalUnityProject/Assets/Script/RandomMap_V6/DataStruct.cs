
using System.Collections.Generic;

namespace RandomMap_V6
{
    public abstract class IGenerater
    {
        protected static MapBuilder mapBuilder;
        protected MapGenerateManager generaterManager;

        public IGenerater(MapGenerateManager generaterManager)
        {
            this.generaterManager = generaterManager;
        }

        public virtual void Initail() { }
        public virtual void Update()
        {
            generaterManager.AddTicks();
        }
        public virtual void End() { }
    }

    #region 座標
    /// <summary>
    /// 區塊座標 地圖區塊的區塊目錄
    /// </summary>
    public struct Coordinate
    {
        public Coordinate(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public int Column { get; }
        public int Row { get; }

        #region Equals 跟 HashCode方法
        public override bool Equals(object obj)
        {
            if (obj is Coordinate)
            {
                return ((Coordinate)obj).Column == Column &&
                    ((Coordinate)obj).Row == Row;
            }
            else
                return false;

            //return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Column * Row;
        }
        #endregion

        #region 計算指定方向的座標
        public static Coordinate operator +(Coordinate c, Direction d)
        {
            return new Coordinate(c.Column + d.Column, c.Row + d.Row);
        }

        public static Coordinate operator -(Coordinate c, Direction d)
        {
            return new Coordinate(c.Column - d.Column, c.Row - d.Row);
        }
        #endregion

        #region 計算座標相不相等
        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.Column == c2.Column && c1.Row == c2.Row;
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return c1.Column != c2.Column || c1.Row != c2.Row;
        }
        #endregion

        #region 比較規則
        public class ColumnFirst_Upward : Comparer<Coordinate>
        {
            public override int Compare(Coordinate c1, Coordinate c2)
            {
                if (c1.Column < c2.Column)
                    return -1;
                else if (c1.Column > c2.Column)
                    return 1;
                else
                {
                    if (c1.Row < c2.Row)
                        return -1;
                    else if (c1.Row > c2.Row)
                        return 1;
                    else
                        return 0;
                }
            }
        }

        public class ColumnFirst_Downward : Comparer<Coordinate>
        {
            public override int Compare(Coordinate c1, Coordinate c2)
            {
                if (c1.Column < c2.Column)
                    return 1;
                else if (c1.Column > c2.Column)
                    return -1;
                else
                {
                    if (c1.Row < c2.Row)
                        return 1;
                    else if (c1.Row > c2.Row)
                        return -1;
                    else
                        return 0;
                }
            }
        }

        public class RowFirst_Upward : Comparer<Coordinate>
        {
            public override int Compare(Coordinate c1, Coordinate c2)
            {
                if (c1.Row < c2.Row)
                    return -1;
                else if (c1.Row > c2.Row)
                    return 1;
                else
                {
                    if (c1.Column < c2.Column)
                        return -1;
                    else if (c1.Column > c2.Column)
                        return 1;
                    else
                        return 0;
                }
            }
        }

        public class RowFirst_Downward : Comparer<Coordinate>
        {
            public override int Compare(Coordinate c1, Coordinate c2)
            {
                if (c1.Row < c2.Row)
                    return 1;
                else if (c1.Row > c2.Row)
                    return -1;
                else
                {
                    if (c1.Column < c2.Column)
                        return 1;
                    else if (c1.Column > c2.Column)
                        return -1;
                    else
                        return 0;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 方向類別 輔助座標計算用
    /// </summary>
    public sealed class Direction
    {
        public int Column { get; }
        public int Row { get; }

        private Direction(int column, int row)
        {
            Column = column;
            Row = row;
        }

        #region 定義
        //定義方向總數
        public const int DirectionCount = 4;

        //固定方向定義
        /// <summary>
        /// Isometric 的右上
        /// </summary>
        public static readonly Direction Top = new Direction(1, 0);
        /// <summary>
        /// Isometric 的左下
        /// </summary>
        public static readonly Direction Down = new Direction(-1, 0);
        /// <summary>
        /// Isometric 的左上
        /// </summary>
        public static readonly Direction Left = new Direction(0, 1);
        /// <summary>
        /// Isometric 的右下
        /// </summary>
        public static readonly Direction Right = new Direction(0, -1);
        #endregion

        #region 方向運算
        /// <summary>
        /// 取得與原方向相反的方向
        /// </summary>
        /// <param name="direction">原方向</param>
        /// <returns>反方向</returns>
        public static Direction Reverse(Direction direction)
        {
            if (direction.Equals(Top))
                return Down;
            else if (direction.Equals(Down))
                return Top;
            else if (direction.Equals(Left))
                return Right;
            else if (direction.Equals(Right))
                return Left;
            else
                throw new System.ArgumentException();
        }

        /// <summary>
        /// 取得原方向左邊的方向
        /// </summary>
        /// <param name="direction">原方向</param>
        /// <returns>左邊方向</returns>
        public static Direction LeftSide(Direction direction)
        {
            if (direction.Equals(Top))
                return Left;
            else if (direction.Equals(Down))
                return Right;
            else if (direction.Equals(Left))
                return Down;
            else if (direction.Equals(Right))
                return Top;
            else
                throw new System.ArgumentException();
        }

        /// <summary>
        /// 取得原方向右邊的方向
        /// </summary>
        /// <param name="direction">原方向</param>
        /// <returns>右邊方向</returns>
        public static Direction RightSide(Direction direction)
        {
            if (direction.Equals(Top))
                return Right;
            else if (direction.Equals(Down))
                return Left;
            else if (direction.Equals(Left))
                return Top;
            else if (direction.Equals(Right))
                return Down;
            else
                throw new System.ArgumentException();
        }
        #endregion

        #region 整數和方向轉換
        /// <summary>
        /// 將座標轉換為對應的整數
        /// 0 - Top
        /// 1 - Down
        /// 2 - Left
        /// 3 - Right
        /// </summary>
        /// <param name="direction">座標下的靜態方向定義</param>
        public static implicit operator int(Direction direction)
        {
            if (direction.Equals(Top))
                return 0;
            else if (direction.Equals(Down))
                return 1;
            else if (direction.Equals(Left))
                return 2;
            else if (direction.Equals(Right))
                return 3;
            else
                throw new System.ArgumentException();
        }

        /// <summary>
        /// 將整數轉換為對應的座標
        /// 0 - Top
        /// 1 - Down
        /// 2 - Left
        /// 3 - Right
        /// </summary>
        /// <param name="direction">座標的整數目錄</param>
        public static implicit operator Direction(int direction)
        {
            switch (direction)
            {
                case 0:
                    return Top;
                case 1:
                    return Down;
                case 2:
                    return Left;
                case 3:
                    return Right;
                default:
                    throw new System.ArgumentException();
            }
        }
        #endregion
    }
    #endregion

    #region 地圖區塊、邊界
    //邊界類型
    public enum BoundaryType : byte { Entry, SmallPath, LargePath, OpenBoundary, Wall }

    //區塊類型
    public enum BlockType : byte { Safe, Normal, BossRoom, Null }

    public class Boundary
    {
        public int Size, Offset;
        public bool HasSet;
        public BoundaryType Type;
        public MapBlock MyBlock, NextBLock;

        public Boundary(MapBlock myBlock, BoundaryType type)
        {
            MyBlock = myBlock;
            Type = type;
            HasSet = false;
        }
    }

    public class MapBlock
    {
        public Boundary[] boundarys;
        public sbyte[,] terrain;
        public BlockType blockType;

        public MapBlock(BlockType type)
        {
            blockType = type;
            boundarys = new Boundary[Direction.DirectionCount];

            terrain = new sbyte[15, 15];

            for (int column = 0; column < 15; column++)
                for (int row = 0; row < 15; row++)
                    terrain[column, row] = -1;
        }
    }
    #endregion
}
