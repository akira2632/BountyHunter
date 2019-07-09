using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItem : MonoBehaviour
{
    [Header("ItemSpeed")]
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        Speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Translate(Vector3.up * Speed * 0.5f * Time.deltaTime);
        else if (Input.GetKey(KeyCode.DownArrow))
            transform.Translate(Vector3.down * Speed * 0.5f * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Translate(Vector3.left * Speed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.RightArrow))
            transform.Translate(Vector3.right * Speed * Time.deltaTime);
    }
}
