using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 基底原件
/// <summary>
/// 區塊建造者基底類別、加入靜態共用參數
/// </summary>
public abstract class IAreaGenerater : IState
{
    public static IAreaBuilder areaBuilder;

    public IAreaGenerater(
        IStateManager stateManager)
        : base(stateManager) { }
}

/// <summary>
/// 區域建造者參數介面
/// </summary>
public abstract class IAreaGenerateParms
{
    public int mapScale;
}
#endregion

/// <summary>
/// 區域建造管理者
/// 紀錄總體生成狀態、生成任務列表、並處理生成決策
/// </summary>
public class AreaGenerateManager : IState
{
    #region 基本資料欄位、初始化相關
    readonly Queue<GenerateTask> GenerateList;
    int mapScale;

    public static BasicAreaGenerater basicAreaGenerater;
    public static AreaSealder areaSealder;

    /// <summary>
    /// 管理者初始化
    /// </summary>
    /// <param name="areaBuilder">取得區塊建造者</param>
    /// <param name="stateManager">取得流程管理者</param>
    /// <param name="mapScale">取得目標地圖規模</param>
    public AreaGenerateManager(IAreaBuilder areaBuilder, IStateManager stateManager, int mapScale) : base(stateManager)
    {
        //有空的話用Factory重構、利用GeneraterManager
        //(繼成自IStateManager、兼具流程管理權責)取得物件工廠
        IAreaGenerater.areaBuilder = areaBuilder;
        this.mapScale = mapScale;
        GenerateList = new Queue<GenerateTask>();


    }

    /// <summary>
    /// 管理用任務結構、紀錄生成用的策略與其參數
    /// </summary>
    struct GenerateTask
    {
        public IAreaGenerater generater;
        public IAreaGenerateParms parms;

        public GenerateTask(IAreaGenerater generater, IAreaGenerateParms parms)
        {
            this.generater = generater;
            this.parms = parms;
        }
    }
    #endregion
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