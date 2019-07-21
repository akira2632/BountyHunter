using System.Collections.Generic;
using UnityEngine;

#region 建造者介面
/// <summary>
/// 區塊建造者介面
/// </summary>
public interface IAreaBuilder
{
    //bool HasCloseEmptyArea(Coordinate coordinate, int areaSize);
    bool HasBlock(Coordinate coordinate);
    bool HasCompleteBlock(Coordinate coordinate);
    bool HasBoundary(Coordinate coordinate, Direction direction);

    void ConectBoundary(Coordinate coordinate, Direction direction);

    void MakeWall(Coordinate coordinate, Direction direction);
    void MakePath(Coordinate coordinate, Direction direction);
    void MakeBlock(Coordinate coordinate);

    void ShowArea();
}

/// <summary>
/// 區塊建造者工廠介面
/// </summary>
public interface IAreaGeneraterFactory
{
    IAreaBuilder GetAreaBuilder();
}
#endregion

#region 簡單狀態機
//流程管理者
public abstract class IStateManager
{
    protected IState nowState;
    protected bool hasInitail;

    public IStateManager()
    {
        hasInitail = false;
    }

    public void Update()
    {
        if (!hasInitail)
        {
            hasInitail = true;
            nowState.Initail();
        }
        else
            nowState.Update();
    }

    public void SetState(IState nextState)
    {
        nowState.End();
        nowState = nextState;
        hasInitail = false;
    }
}

//流程狀態
public abstract class IState
{
    protected IStateManager stateManager;

    public IState(IStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    public virtual void Initail() { }
    public virtual void Update() { }
    public virtual void End() { }
}
#endregion
