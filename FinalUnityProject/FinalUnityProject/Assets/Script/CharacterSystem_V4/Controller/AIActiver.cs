using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class AIActiver : MonoBehaviour
    {
        public GameObject Character;
        public AIStateManager AI;

        private void LateUpdate()
        {
            transform.position = Character.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "Player")
            {
                AI.enabled = true;
                Character.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "Player")
            {
                AI.enabled = false;
                Character.SetActive(false);
            }
        }
    }

}
