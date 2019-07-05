using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAddTest : MonoBehaviour
{
    Tilemap tilemap;
    int x = 0, y = 0, z = 0;
    public TileBase testTile;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        tilemap.SetTile(new Vector3Int(x, y, z), testTile);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (tilemap.GetTile(new Vector3Int(x, y, z)) != null)
                tilemap.SetTile(new Vector3Int(x, y, z), null);
            else
                tilemap.SetTile(new Vector3Int(x, y, z), testTile);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            x++;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            x--;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            y++;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            y--;

        if (Input.GetKeyDown(KeyCode.PageUp))
            z++;
        else if (Input.GetKeyDown(KeyCode.PageDown))
            z--;
    }
}
