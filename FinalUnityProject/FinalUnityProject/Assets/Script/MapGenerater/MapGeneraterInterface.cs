using System.Collections.Generic;
using UnityEngine;

//區塊建造者介面
public interface IBlockBuilder
{
    bool HasCompleteBlock(Coordinate coordinate);
    bool HasBoundary(Coordinate coordinate, Direction direction);

    void ConectBoundary(Coordinate coordinate, Direction direction);

    void MakeWall(Coordinate coordinate, Direction direction);
    void MakePath(Coordinate coordinate, Direction direction);
    void MakeBlock(Coordinate coordinate);
}

//生成規則介面
public abstract class IAreaGenerater
{
    protected IBlockBuilder blockBuilder;
    protected Coordinate startPoint;

    /// <summary>
    /// 取得區塊建造者與生成起點
    /// </summary>
    /// <param name="blockBuilder">區塊建造者</param>
    /// <param name="startPoint">生成起點</param>
    public IAreaGenerater(IBlockBuilder blockBuilder, Coordinate startPoint)
    {
        this.blockBuilder = blockBuilder;
        this.startPoint = startPoint;
    }

    /// <summary>
    /// 生成區域並返回可繼續生成的生成點
    /// </summary>
    /// <returns>生成點列表</returns>
    public abstract List<Coordinate> GenerateArea();
}