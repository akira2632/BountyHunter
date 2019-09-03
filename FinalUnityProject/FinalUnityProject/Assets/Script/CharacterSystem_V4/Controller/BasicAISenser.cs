using Pathfinding;
using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAISenser : MonoBehaviour
    {
        public delegate void SenerEventBool(bool data);

        public event SenerEventBool OnPlayerCloseBy;

        public Seeker seeker;
        public Path path;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
                OnPlayerCloseBy?.Invoke(true);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
                OnPlayerCloseBy?.Invoke(false);
        }
    }
}
