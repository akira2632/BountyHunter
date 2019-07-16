using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class MoveItem_Rigidbody2D : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbody2d;
    Vector2 position;
    int x, y;
    public float Speed;
    public Sprite[] sprites = new Sprite[9];

    // Start is called before the first frame update
    void Start()
    {
        //Get Reference
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Initiali
        Speed = 5.0f;
        x = 1;
        y = 3;
        spriteRenderer.sprite = sprites[x + y];
        rigidbody2d.gravityScale = 0;
        rigidbody2d.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        position = rigidbody2d.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = 0;
            position += Vector2.left * Speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            x = 2;
            position += Vector2.right * Speed * Time.deltaTime;
        }
        else if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            x = 1;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            y = 0;
            position += Vector2.up * 0.5f * Speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            y = 6;
            position += Vector2.down * 0.5f * Speed * Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            y = 3;

        rigidbody2d.MovePosition(position);

        if(x + y != 4)
            spriteRenderer.sprite = sprites[x + y];
    }
}
