using UnityEngine;

namespace UI
{
    public class ChangeScene : MonoBehaviour
    {
        public string SceneName;

        void OnTriggerEnter2D(Collider2D hit)
        {
            if (hit.gameObject.tag == "Player")
                Application.LoadLevel(SceneName);
        }
    }
}
