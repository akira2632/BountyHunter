using System.Collections.Generic;
using UnityEngine;

#region 建造者介面
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

#endregion

#region 建造流程控制
//回乎函式
public delegate void IsCompelet();

//流程管理者
public abstract class IGeneraterManager
{
    IGenerateState generater;
    Queue<IGenerateState> taskQueue;
    IsCompelet isCompelet;
    bool hasInitail;

    public IGeneraterManager(IsCompelet isCompelet)
    {
        this.isCompelet = isCompelet;
        hasInitail = false;
        taskQueue = new Queue<IGenerateState>();
    }

    void Update()
    {
        if (!hasInitail)
        {
            hasInitail = true;
            generater.Initail();
        }
        else
            generater.Update();
    }

    public void SetNextTask()
    {
        if (taskQueue.Count > 0)
        {
            generater.End();
            generater = taskQueue.Dequeue();
            hasInitail = false;
        }
        else
            isCompelet.Invoke();
    }

    public abstract void AddTask(List<Coordinate> GeneratePoint);
}

//生成流程介面
public abstract class IGenerateState
{
    protected IGeneraterManager manager;

    public IGenerateState(IGeneraterManager manager)
    {
        this.manager = manager;
    }

    public abstract void Initail();
    public abstract void Update();
    public abstract void End();
}
#endregion
