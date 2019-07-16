using UnityEngine;
using UnityEngine.Animations;

public class Character_Move : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    Rigidbody2D rigidbody2d;

    Vector2 position;
    int x, y, action;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        Speed = 5.0f;
        x = 0;
        y = 0;
        action = 0;
    }

    // Update is called once per frame
    void Update()
    {
        position = rigidbody2d.position;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = 0;
            action = 1;
            position += Vector2.left * Speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            x = 2;
            action = 1;
            position += Vector2.right * Speed * Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            action = 0;
            x = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            y = 0;
            action = 1;
            position += Vector2.up * 0.5f * Speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            y = 6;
            action = 1;
            position += Vector2.down * 0.5f * Speed * Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            action = 0;
            y = 3;
        }

        rigidbody2d.MovePosition(position);

        
        if (action == 0)
            animator.SetInteger("Action", 0);
        else if (action == 1)
            animator.SetInteger("Action", 1);

        if ((x + y) != 4)
            animator.SetInteger("Direction", x + y);
    }
}
