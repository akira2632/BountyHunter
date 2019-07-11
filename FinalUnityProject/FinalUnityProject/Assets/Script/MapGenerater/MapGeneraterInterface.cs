using System.Collections.Generic;
using UnityEngine;

#region 建造者介面
//區塊建造者介面
public interface IAreaBuilder
{
    bool HasCompleteBlock(Coordinate coordinate);
    bool HasBoundary(Coordinate coordinate, Direction direction);

    void ConectBoundary(Coordinate coordinate, Direction direction);

    void MakeWall(Coordinate coordinate, Direction direction);
    void MakePath(Coordinate coordinate, Direction direction);
    void MakeBlock(Coordinate coordinate);
}
#endregion

#region 簡單狀態機
//流程管理者
public abstract class IStateManager
{
    IState nowState;
    bool hasInitail;

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

//生成流程介面
public abstract class IState
{
    protected IStateManager manager;

    public IState(IStateManager manager)
    {
        this.manager = manager;
    }

    public virtual void Initail() { }
    public virtual void Update() { }
    public virtual void End() { }
}
#endregion
