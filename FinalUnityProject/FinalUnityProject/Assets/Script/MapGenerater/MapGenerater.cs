using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerater : MonoBehaviour
{
    GeneraterManager generaterManager;
    public MapBuilder mapBuilder;

    // Start is called before the first frame update
    void Start()
    {
        generaterManager = new GeneraterManager(mapBuilder);
    }

    // Update is called once per frame
    void Update()
    {
        generaterManager.Update();
    }
}
