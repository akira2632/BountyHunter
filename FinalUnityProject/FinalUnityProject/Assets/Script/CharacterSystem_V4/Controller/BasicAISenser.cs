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

        public GameObject Character;

        [SerializeField]
        private Seeker seeker;
        [SerializeField]
        private Path path;

        private Vector3 PlayerPosition;

        private int currentWayPoint = 0;

        private void LateUpdate()
        {
            transform.position = Character.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
                OnPlayerCloseBy?.Invoke(true);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
                PlayerPosition = collision.transform.position;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
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
