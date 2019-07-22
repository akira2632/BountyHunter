using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneraterManager : IStateManager
{
    public GeneraterManager(MapBuilder mapBuilder) : base()
    {
        Debug.Log("Generater manager create");
        nowState = new AreaGenerateManager(this, mapBuilder, 30);
    }
}