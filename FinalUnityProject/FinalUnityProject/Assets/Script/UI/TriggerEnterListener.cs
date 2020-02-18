using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class TriggerEnterListener : MonoBehaviour
    {
        public string targetTag;
        public UnityEvent invokeEvents;

        void OnTriggerEnter2D(Collider2D hit)
        {
            if (hit.gameObject.tag == targetTag)
                invokeEvents?.Invoke();
        }
    }
}
