using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 區塊建造者基底類別、加入靜態共用參數
/// </summary>
public abstract class IAreaGenerateState : IState
{
    public static IAreaBuilder areaBuilder;
    public static AreaGenerateData generateData;

    public IAreaGenerateState(
        IStateManager stateManager)
        : base(stateManager) { }
}

/// <summary>
/// 共用參數管理者
/// </summary>
public class AreaGenerateData
{
    public BasicAreaGenerater basicAreaGenerater;
    public AreaSealder areaSealder;
}

/// <summary>
/// 區域建造者參數介面
/// </summary>
public interface IAreaGenerateParms { }

public class AreaGenerateManager : IState
{
    public AreaGenerateManager(IAreaBuilder areaBuilder, IStateManager stateManager, int mapScale) : base(stateManager)
    {
        IAreaGenerateState.generateData = new AreaGenerateData();
        IAreaGenerateState.areaBuilder = areaBuilder;
    }
}

/// <summary>
/// 迷宮起點建造者
/// </summary>
public class StartPointGenerater : IState
{
    public StartPointGenerater(IStateManager stateManager) : base(stateManager) { }
}

public class BasicAreaGenerater : IState
{
    public BasicAreaGenerater(IStateManager stateManager) : base(stateManager) { }
}

public class AreaSealder : IState
{
    public AreaSealder(IStateManager stateManager) : base(stateManager) { }
}

#region 有空的話
public class OpeanAreaGenerater : IState
{
    public OpeanAreaGenerater(IStateManager stateManager) : base(stateManager) { }
}

public class SmallPathGenerater : IState
{
    public SmallPathGenerater(IStateManager stateManager) : base(stateManager) { }
}

public class LargPathGenerater : IState
{
    public LargPathGenerater(IStateManager stateManager) : base(stateManager) { }
}

public class SmallCurvePathGenerater : IState
{
    public SmallCurvePathGenerater(IStateManager stateManager) : base(stateManager) { }
}

public class LargCurvePathGenerater : IState
{
    public LargCurvePathGenerater(IStateManager stateManager) : base(stateManager) { }
}
#endregion