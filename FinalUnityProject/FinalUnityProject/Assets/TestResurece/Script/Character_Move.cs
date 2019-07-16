using UnityEngine;
using UnityEngine.Animations;

public class Character_Move : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rigidbody2d;

    Vector2 position;
    int x, y, action;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        Speed = 5.0f;
        x = -1;
        y = 1;
        action = 0;
    }

    // Update is called once per frame
    void Update()
    {
        position = rigidbody2d.position;

        if(action != 2)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                x = -1;
                action = 1;
                position += Vector2.left * Speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                x = 1;
                action = 1;
                position += Vector2.right * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                y = 1;
                action = 1;
                position += Vector2.up * 0.5f * Speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                y = -1;
                action = 1;
                position += Vector2.down * 0.5f * Speed * Time.deltaTime;
            }
            
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                action = 0;
                x = 0;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                action = 0;
                y = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
            action = 2;

        rigidbody2d.MovePosition(position);
        
        animator.SetInteger("Action", action);
        
        if(! (x==0 && y == 0))
        {
            animator.SetFloat("X", x);
            animator.SetFloat("Y", y);
        }
    }

    public void AttackEnd()
    {
        action = 0;
        Debug.Log("has called");
    }
}
