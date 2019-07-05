using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
