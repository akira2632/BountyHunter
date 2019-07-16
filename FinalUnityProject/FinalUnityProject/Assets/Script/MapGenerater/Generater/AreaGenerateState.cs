using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 基底元件
/// <summary>
/// 區塊建造者基底類別、加入靜態共用參數
/// </summary>
public abstract class IAreaGenerater : IState
{
    // areaBuilder為共通的區塊建造者
    protected AreaGenerateParms parms;
    protected static IAreaBuilder areaBuilder;

    public IAreaGenerater(
        IStateManager GenerateManager)
        : base(GenerateManager) { }
    
    public void SetParameter(AreaGenerateParms parms)
    {
        this.parms = parms;
    }
}

/// <summary>
/// 區域建造策略參數
/// </summary>
public struct AreaGenerateParms
{
    /* mapQuota 剩餘的規模扣打
     * areaSclae 目標區域規模
     * startPoint 目標區域起始點
     * startDirection 起始點方向
     */
    public int mapQuota;
    public int areaScale;
    public Coordinate startPoint;
}

/// <summary>
/// 區域建造者工廠介面
/// </summary>
public interface IAreaGeneraterFactory
{
    IAreaBuilder GetAreaBuilder();
}
#endregion

/// <summary>
/// 區域建造管理者
/// 紀錄總體生成狀態、生成任務列表、並處理生成決策
/// </summary>
public class AreaGenerateManager : IAreaGenerater
{
    #region 基本資料欄位、初始化相關
    Queue<GenerateTask> GenerateList;
    Queue<AreaGenerateParms> GeneratePoint;

    protected static BasicAreaGenerater basicAreaGenerater;
    protected static AreaSealder areaSealder;

    /// <summary>
    /// 管理者初始化
    /// </summary>
    /// <param name="GenerateManager">取得流程管理者</param>
    /// <param name="mapScale">取得目標地圖規模</param>
    public AreaGenerateManager(GeneraterManager GenerateManager, int mapScale) : base(GenerateManager)
    {
        //外部參數、取得區域建造者
        areaBuilder = GenerateManager.GetAreaBuilder();

        //任務隊列、紀錄生成用的參數與策略
        GenerateList = new Queue<GenerateTask>();

        //預先建立共用的生成策略物件
        basicAreaGenerater = new BasicAreaGenerater(GenerateManager);
        areaSealder = new AreaSealder(GenerateManager);

        //設定起始任務
        GenerateList.Enqueue(new GenerateTask(basicAreaGenerater,
            new AreaGenerateParms
            {
                mapQuota = 0,
                areaScale = mapScale,
                startPoint = new Coordinate()
            }));
    }

    /// <summary>
    /// 管理用任務結構、紀錄生成用的策略與其參數
    /// </summary>
    struct GenerateTask
    {
        public IAreaGenerater generater;
        public AreaGenerateParms parms;

        public GenerateTask(IAreaGenerater generater, AreaGenerateParms parms)
        {
            this.generater = generater;
            this.parms = parms;
        }
    }
    #endregion

    //目前只會動而已
    #region 執行期決策
    public override void Update()
    {
        //將未處理的生成點列入任務列表
        if (GeneratePoint.Count > 0)
        {
            AreaGenerateParms temp = GeneratePoint.Dequeue();
            //剩餘扣打大於8
            if (temp.mapQuota > 8)
            {
                int newAreaScale = Random.Range(3, 5);
                GenerateList.Enqueue(new GenerateTask(basicAreaGenerater,
                new AreaGenerateParms
                {
                    mapQuota = temp.mapQuota - newAreaScale,
                    areaScale = newAreaScale,
                    startPoint = temp.startPoint
                }));
            }
            //若剩餘扣打不足8
            else if (temp.mapQuota > 0)
            {
                GenerateList.Enqueue(new GenerateTask(basicAreaGenerater,
                new AreaGenerateParms
                {
                    mapQuota = 0,
                    areaScale = temp.mapQuota,
                    startPoint = temp.startPoint
                }));
            }
            //扣打已用盡、閉鎖區域
            else
            {
                GenerateList.Enqueue(new GenerateTask(areaSealder,
                new AreaGenerateParms
                {
                    mapQuota = 0,
                    areaScale = 0,
                    startPoint = temp.startPoint
                }));
            }
        }
        //處理任務列表上的任務
        else if (GenerateList.Count > 0)
        {
            GenerateTask temp = GenerateList.Dequeue();
            if(!areaBuilder.HasCompleteBlock(temp.parms.startPoint))
            {
                temp.generater.SetParameter(temp.parms);
                stateManager.SetState(temp.generater);
            }
        }
        //轉換至下一處理階段
        else
        {

        }
    }

    public void AddTask()
    {

    }
    #endregion
}

/// <summary>
/// 基礎生成規則
/// </summary>
public class BasicAreaGenerater : IAreaGenerater
{
    int generateTurn, hasBlock;
    bool hasMake;
    Queue<Coordinate> GeneratePoint, GenerateTemp;
    Coordinate target;

    public BasicAreaGenerater(IStateManager GenerateManager) : base(GenerateManager) { }

    public override void Initail()
    {
        generateTurn = 0;
        GeneratePoint = new Queue<Coordinate>();
        GenerateTemp = new Queue<Coordinate>();
        GeneratePoint.Enqueue(parms.startPoint);
    }

    public override void Update()
    {
        //直到完成指定區塊大小為止
        if(generateTurn < parms.areaScale)
        {
            //對生成清單內的所有點進行生成
            if(GeneratePoint.Count > 0)
            {
                hasMake = false;
                target = GeneratePoint.Dequeue();

                while (!hasMake)
                {
                    hasBlock = 0;

                    for (int d = 0; d < Direction.DirectionCount; d++)
                    {
                        if (areaBuilder.HasCompleteBlock(target + d))
                        {
                            hasBlock++;
                            areaBuilder.ConectBoundary(target, d);
                        }
                        else if  (Random.Range(0,100) < 30)
                        {
                            hasMake = true;
                            areaBuilder.MakePath(target, d);
                        }
                    }

                    if(hasBlock >3)
                    {
                        break;
                    }
                }

                /*
                for (int d= 0; d< Direction.DirectionCount; d++)
                {
                    if (!areaBuilder.HasBoundary(target, d))
                        areaBuilder.MakeWall(target, d);
                    else if(areaBuilder.)
                }*/
            }
            //完成一輪生成、清空生成清單並加入暫存清單
            else
            {
                generateTurn++;
                GeneratePoint = GenerateTemp;
                GenerateTemp = new Queue<Coordinate>();
            }
        }
        else
        {

        }
    }
}

/// <summary>
/// 區塊閉鎖規則
/// </summary>
public class AreaSealder : IAreaGenerater
{
    public AreaSealder(IStateManager GenerateManager) : base(GenerateManager) { }
}

#region 有空的話
public class OpeanAreaGenerater : IState
{
    public OpeanAreaGenerater(IStateManager GenerateManager) : base(GenerateManager) { }
}

public class SmallPathGenerater : IState
{
    public SmallPathGenerater(IStateManager GenerateManager) : base(GenerateManager) { }
}

public class LargPathGenerater : IState
{
    public LargPathGenerater(IStateManager GenerateManager) : base(GenerateManager) { }
}

public class SmallCurvePathGenerater : IState
{
    public SmallCurvePathGenerater(IStateManager GenerateManager) : base(GenerateManager) { }
}

public class LargCurvePathGenerater : IState
{
    public LargCurvePathGenerater(IStateManager GenerateManager) : base(GenerateManager) { }
}
#endregion