using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class TriggerEnterListener : MonoBehaviour
    {
        public string TargetTag;
        public UnityEvent InvokeEvents;

        void OnTriggerEnter2D(Collider2D hit)
        {
            if (hit.gameObject.tag == TargetTag)
                InvokeEvents?.Invoke();
        }
    }
}
