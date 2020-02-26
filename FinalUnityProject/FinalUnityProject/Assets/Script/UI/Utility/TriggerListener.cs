using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class TriggerListener : MonoBehaviour
    {
        public string TargetTag;
        public UnityEvent EnterEvents, ExitEvents;

        private void OnTriggerEnter2D(Collider2D hit)
        {
            if (hit.gameObject.tag == TargetTag)
                EnterEvents?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D hit)
        {
            if (hit.gameObject.tag == TargetTag)
                ExitEvents?.Invoke();
        }
    }
}
