using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBuilder : MonoBehaviour, IAreaBuilder
{
    public Tile CenterTile, PathTile;
    public Tilemap tilemap;
    private Dictionary<Coordinate, MapBlock> map;

    public MapBuilder()
    {
        map = new Dictionary<Coordinate, MapBlock>();
    }

    #region 區塊檢查

    public bool HasBlock(Coordinate coordinate)
    {
        return map.ContainsKey(coordinate);
    }

    /// <summary>
    /// 確認座標上是否有已完成區塊
    /// </summary>
    /// <param name="coordinate">目標座標</param>
    /// <returns>是否有已完成區塊</returns>
    public bool HasCompleteBlock(Coordinate coordinate)
    {
        if (!map.ContainsKey(coordinate))
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

    /*
    public bool HasCloseEmptyArea(Coordinate coordinate, int areaSize)
    {
        for (int i = 0; i < length; i++)
        {

        }
    }*/
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

    public void ShowArea()
    {
        foreach (KeyValuePair<Coordinate, MapBlock> data in map)
        {
            int x = data.Key.Column * 3;
            int y = data.Key.Row * 3;

            Debug.Log("Set tile at (" + x + "," + y + ")");

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                    tilemap.SetTile(new Vector3Int(x + i, y + j, 0), CenterTile);

            tilemap.SetTile(new Vector3Int(x, y, 0), PathTile);

            if (data.Value.boundarys[Direction.Top] == Boundary.Path)
                tilemap.SetTile(new Vector3Int(x + 1, y, 0), PathTile);
            if (data.Value.boundarys[Direction.Down] == Boundary.Path)
                tilemap.SetTile(new Vector3Int(x - 1, y, 0), PathTile);
            if (data.Value.boundarys[Direction.Left] == Boundary.Path)
                tilemap.SetTile(new Vector3Int(x, y + 1, 0), PathTile);
            if (data.Value.boundarys[Direction.Right] == Boundary.Path)
                tilemap.SetTile(new Vector3Int(x, y - 1, 0), PathTile);
        }
    }
}

