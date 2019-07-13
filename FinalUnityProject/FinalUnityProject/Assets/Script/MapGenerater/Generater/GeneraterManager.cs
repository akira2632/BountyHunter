using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneraterManager : IStateManager, IAreaGeneraterFactory
{
    static MapBuilder mapBuilder;

    public GeneraterManager() : base()
    {
        nowState = new AreaGenerateManager(this, 50);
        mapBuilder = new MapBuilder();
    }
    
    public IAreaBuilder GetAreaBuilder()
    {
        return mapBuilder;
    }
}