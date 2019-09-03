using System;
using System.Collections;
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

        private int currentWayPoint = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("PlayerEnter");
            if (collision.gameObject.tag == "Player")
                OnPlayerCloseBy?.Invoke(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log("PlayerLeve");
            if (collision.gameObject.tag == "Player")
                OnPlayerCloseBy?.Invoke(false);
        }

        public bool NextWayPoint(out Vector3 nextPoint)
        {
            if (currentWayPoint < path.vectorPath.Count)
            {
                nextPoint = path.vectorPath[currentWayPoint++];
                return true;
            }
            else
            {
                nextPoint = new Vector3();
                return false;
            }
        }

        public void FindPath(Vector3 start, Vector3 end, Action<bool?> pathFinded)
            => StartCoroutine(MyFindPath(start, end, pathFinded));

        private IEnumerator MyFindPath(Vector3 start, Vector3 end, Action<bool?> pathFinded)
        {
            seeker.StartPath(start, end, (Path path) => this.path = path);

            while (!seeker.IsDone())
                yield return new WaitForSeconds(0.5f);

            if (path.error)
                pathFinded(null);
            else
            {
                currentWayPoint = 0;
                pathFinded(true);
            }
        }
    }
}
