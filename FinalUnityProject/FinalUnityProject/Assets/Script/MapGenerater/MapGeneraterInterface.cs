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
//流程管理者
public abstract class IGeneraterManager
{
    IGeneraterState generater;
    Queue<IGeneraterState> taskQueue;
    bool hasInitail;

    void Start()
    {
        hasInitail = false;
        taskQueue = new Queue<IGeneraterState>();
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

    public bool SetNextTask()
    {
        if (taskQueue.Count > 0)
        {
            generater.End();
            generater = taskQueue.Dequeue();

            return true;
        }
        else
            return false;
    }

    public abstract void AddTask(List<Coordinate> GeneratePoint);
}

//生成流程介面
public abstract class IGeneraterState
{
    protected IGeneraterManager manager;

    public IGeneraterState(IGeneraterManager manager)
    {
        this.manager = manager;
    }

    public abstract void Initail();
    public abstract void Update();
    public abstract void End();
}
#endregion
