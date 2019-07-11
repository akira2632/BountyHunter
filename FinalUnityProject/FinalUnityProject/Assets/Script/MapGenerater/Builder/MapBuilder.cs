using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : IAreaBuilder
{
    private Dictionary<Coordinate, MapBlock> map;

    public MapBuilder()
    {
        map = new Dictionary<Coordinate, MapBlock>();
    }

    #region 區塊檢查
    /// <summary>
    /// 確認座標上是否有已完成區塊
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    /// <returns>是否有已完成區塊</returns>
    public bool HasCompleteBlock(Coordinate coordinate)
    {
        if (map.ContainsKey(coordinate))
            return false;
        else
            for (int d = 0; d < Direction.DirectionCount; d++)
                if (map[coordinate].boundarys[d] == Boundary.Null)
                    return false;

        return true;
    }

    /// <summary>
    /// 確認指定座標的方向上是否有邊界
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    /// <param name="direction">目標方向</param>
    /// <returns>是否有邊界</returns>
    public bool HasBoundary(Coordinate coordinate, Direction direction)
    {
        return map[coordinate].boundarys[direction] != Boundary.Null;
    }
    #endregion

    #region 建造區塊
    /// <summary>
    /// 連結指定座標方向上的邊界
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    /// <param name="direction">目標方向</param>
    public void ConectBoundary(Coordinate coordinate, Direction direction)
    {
        map[coordinate].boundarys[direction] = map[coordinate + direction].boundarys[!direction];
    }

    /// <summary>
    /// 於指定邊界上建造牆壁
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    /// <param name="direction">目標方向</param>
    public void MakeWall(Coordinate coordinate, Direction direction)
    {
        map[coordinate].boundarys[direction] = Boundary.Wall;
    }

    /// <summary>
    /// 於指定邊界上建造通道
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    /// <param name="direction">目標方向</param>
    public void MakePath(Coordinate coordinate, Direction direction)
    {
        map[coordinate].boundarys[direction] = Boundary.Path;
    }

    /// <summary>
    /// 於指定座標上建造新區塊
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    public void MakeBlock(Coordinate coordinate)
    {
        map.Add(coordinate, new MapBlock());
    }
    #endregion
}

