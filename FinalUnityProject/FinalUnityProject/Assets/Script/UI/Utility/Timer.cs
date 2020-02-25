using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Min(0)]
    public float InvokeTime;
    public bool InvokeRepeat = false;

    public UnityEvent InvokeEvents;

    private float timer;

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        if(timer < InvokeTime)
        {
            timer += Time.deltaTime;

            if (timer > InvokeTime)
                InvokeEvents?.Invoke();
        }
        else if(InvokeRepeat)
        {
            timer = 0;
        }
    }
}
