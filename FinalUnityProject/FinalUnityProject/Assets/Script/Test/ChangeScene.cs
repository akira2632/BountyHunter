using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    bool GoIntoCave;


    public void Update()
    {
        

        if (GoIntoCave)
        {
            GoToCave();
        }


    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        // 當玩家進入洞穴
        if (hit.GetComponent<Collider2D>().name == "Tilemap_Cave")
        {
            GoIntoCave = true;
        }
    }


    public void GoToCave()
    {
        Application.LoadLevel("RandomMap_V6_AddCharacter");
    }
}
