using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class AIActiver : MonoBehaviour
    {
        public GameObject Character;
        public AIStateManager AI;
        public BasicAISenser Senser;

        private void LateUpdate()
        {
            transform.position = Character.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                AI.enabled = true;
                Senser.enabled = true;
                Character.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                AI.enabled = false;
                Senser.enabled = false;
                Character.SetActive(false);
            }
        }
    }

}
