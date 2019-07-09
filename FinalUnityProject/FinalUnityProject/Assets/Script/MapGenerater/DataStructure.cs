using UnityEngine;

#region 座標
/// <summary>
/// 區塊座標 地圖區塊的區塊目錄
/// </summary>
public struct Coordinate
{
    int _column, _row;

    public Coordinate(int column, int row)
    {
        _column = column;
        _row = row;
    }

    public int Column { get => _column; }
    public int Row { get => _row; }

    #region 讓編譯器閉嘴
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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
        return c1._column == c2._column && c1._row == c2._row;
    }

    public static bool operator !=(Coordinate c1, Coordinate c2)
    {
        return c1._column != c2._column || c1._row != c2._row;
    }
    #endregion
}

/// <summary>
/// 方向類別 輔助座標計算用
/// </summary>
public sealed class Direction
{
    int _column, _row;
    public int Column { get => _column; }
    public int Row { get => _row; }

    private Direction(int column, int row)
    {
        _column = column;
        _row = row;
    }

    //定義方向總數
    public const int DirectionCount = 4;

    //固定方向定義
    public static readonly Direction Top = new Direction(1, 0);
    public static readonly Direction Bottom = new Direction(-1, 0);
    public static readonly Direction Left = new Direction(-1, 0);
    public static readonly Direction Right = new Direction(0, 1);

    #region 反方向運算
    /// <summary>
    /// 計算與原方向相反的方向
    /// </summary>
    /// <param name="d">原方向</param>
    /// <returns>反方向</returns>
    public static Direction operator !(Direction direction)
    {
        if (direction.Equals(Top))
            return Bottom;
        else if (direction.Equals(Bottom))
            return Top;
        else if (direction.Equals(Left))
            return Right;
        else if (direction.Equals(Right))
            return Left;
        else
            throw new System.ArgumentException();
    }
    #endregion

    #region 整數和方向轉換
    /// <summary>
    /// 將座標轉換為對應的整數
    /// 0 - Top
    /// 1 - Bottom
    /// 2 - Left
    /// 3 - Right
    /// </summary>
    /// <param name="direction">座標下的靜態方向定義</param>
    public static implicit operator int(Direction direction)
    {
        if (direction.Equals(Top))
            return 0;
        else if (direction.Equals(Bottom))
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
    /// 1 - Bottom
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
                return Bottom;
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
internal enum Boundary { Null, Path, Wall}

internal class MapBlock
{
    public Boundary[] boundarys;

    public MapBlock()
    {
        boundarys = new Boundary[Direction.DirectionCount];

        for (int d = 0; d < Direction.DirectionCount; d++)
            boundarys[d] = Boundary.Null;
    }
}
#endregion
