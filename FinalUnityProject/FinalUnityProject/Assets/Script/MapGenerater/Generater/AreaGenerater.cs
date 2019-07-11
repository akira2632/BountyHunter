using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 區塊建造流程管理
public abstract class AreaGenerater : IState
{
    protected AreaGeneraterManager areaManager;
    protected IAreaGenerateParms Parms;

    public AreaGenerater(
        AreaGeneraterManager areaManager,
        GenerateManager stateManager) : base(stateManager)
    {
        this.areaManager = areaManager;
    }
}

//流程決策者
public class AreaGeneraterManager
{
    public Queue<Task> TaskQueue;
    public IAreaBuilder AreaBuilder;

    public AreaGeneraterManager(IAreaBuilder areaBuilder)
    {
        AreaBuilder = areaBuilder;
    }

    public bool NotComplete()
    {
        return TaskQueue.Count > 0;
    }
}

//內部管理用資料
public struct Task
{
    public AreaGenerater Generater;
    public IAreaGenerateParms Parms;
}

public interface IAreaGenerateParms { }
#endregion

#region 區塊生成規則
public class AreaGenerateStart : AreaGenerater
{
    public AreaGenerateStart(
        AreaGeneraterManager areaManager,
        GenerateManager stateManager) : 
        base(areaManager, stateManager) { }


}
#endregion
