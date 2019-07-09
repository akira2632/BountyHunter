using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ShowTileCoordnate : MonoBehaviour
{
    public Text PlayerPosition, TileCoordinate, TilePosition;
    public GridLayout MyGrid;
    public Tilemap MyTilemap;
    public Tile MyTile;
    private int tilePositionX, tilePositionY;

    // Start is called before the first frame update
    void Start()
    {
        tilePositionX = 0;
        tilePositionY = 0;

        MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), MyTile);

        PlayerPosition.text = ShowPlayerCoordinate();
        TileCoordinate.text = ShowTileCoordinate();
        TilePosition.text = ShowTilePosition();
    }

    // Update is called once per frame
    void Update()
    {
        #region PlayerMove
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * Time.deltaTime);
        else if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.down * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * Time.deltaTime);
        else if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
            transform.SetPositionAndRotation(
                MyGrid.CellToWorld(new Vector3Int(tilePositionX, tilePositionY, 0)),
                Quaternion.identity);
        #endregion

        #region TileMove
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), null);
            tilePositionX++;
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), MyTile);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), null);
            tilePositionX--;
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), MyTile);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), null);
            tilePositionY++;
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), MyTile);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), null);
            tilePositionY--;
            MyTilemap.SetTile(new Vector3Int(tilePositionX, tilePositionY, 0), MyTile);
        }
        #endregion

        PlayerPosition.text = ShowPlayerCoordinate();
        TileCoordinate.text = ShowTileCoordinate();
        TilePosition.text = ShowTilePosition();
    }

    private string ShowPlayerCoordinate()
    {
        return "玩家位置:\nx = " + transform.localPosition.x + "\ny = " + transform.localPosition.y;
    }

    private string ShowTileCoordinate()
    {
        return "地圖座標:\nx = " + tilePositionX + "\ny = " + tilePositionY;
    }

    private string ShowTilePosition()
    {
        return "地圖位置:\nx = " + MyGrid.CellToWorld(new Vector3Int(tilePositionX, tilePositionY, 0)).x
            + "\ny = " + MyGrid.CellToWorld(new Vector3Int(tilePositionX, tilePositionY, 0)).y;
    }
}
