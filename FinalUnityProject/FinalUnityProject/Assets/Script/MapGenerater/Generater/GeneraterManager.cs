using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneraterManager : IStateManager
{
    public GeneraterManager() : base()
    {
        nowState = new AreaGenerateInitail(this);
    }
}

public class AreaGenerateInitail : IState
{
    public AreaGenerateInitail(IStateManager stateManager) : base(stateManager) { }

    public override void Initail() { }

    public override void Update()
    {
        AreaGenerateManager areaGenerateManager =
            new AreaGenerateManager(new MapBuilder(), stateManager, 50);

        stateManager.SetState(areaGenerateManager);
    }
}