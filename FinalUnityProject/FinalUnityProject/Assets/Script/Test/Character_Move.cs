using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
public class Character_Move : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rigidbody2d;
    public AudioSource moveingsound,swing;
    Vector2 position;
    int x, y, action;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        x = -1;
        y = 1;
        
        action = 0;
        AudioSource source = GameObject.FindGameObjectWithTag("movingsound").GetComponent<AudioSource>();
    }

    
    // Update is called once per frame
    void Update()
    {
        
        position = rigidbody2d.position;
        
        
        if (action != 2)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveingsound.Play();
                x = -1;
                action = 1;
                position += Vector2.left * Speed * Time.deltaTime;

            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveingsound.Play();
                x = 1;
                action = 1;
                position += Vector2.right * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveingsound.Play();
                y = 1;
                action = 1;
                position += Vector2.up * 0.5f * Speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveingsound.Play();
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

        if (Input.GetKeyDown(KeyCode.G) || Input.GetMouseButtonDown(1) && action != 2)
        {
            swing.Play();
            action = 2;
        }

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
