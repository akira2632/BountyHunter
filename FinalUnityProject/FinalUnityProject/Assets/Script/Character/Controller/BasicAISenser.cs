using Pathfinding;
using System;
using System.Collections;
using UnityEngine;

namespace Character.Controller
{
    [RequireComponent(typeof(Seeker))]
    public class BasicAISenser : MonoBehaviour
    {
        [SerializeField]
        private Seeker seeker;
        private Path path;

        private int currentWayPoint = 0;
        private bool continueFinding = false;

        public bool PathFinded { get; private set; } = false;

        private void Start()
        {
            seeker = GetComponent<Seeker>();
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
        
        public void FindPath(Vector3 target, Action<Vector3> returnFirstPoint)
        {
            PathFinded = false;
            StartCoroutine(MyFindPath(target, returnFirstPoint));
        }

        public void FindPath(Transform target, Action<Vector3> returnFirstPoint)
        {
            PathFinded = false;
            StartCoroutine(ContinueFinding(target, returnFirstPoint));
        }

        public void StopFindPath() => continueFinding = false;

        #region A*Seekera
        private IEnumerator ContinueFinding(Transform target, Action<Vector3> returnFirstPoint)
        {
            seeker.CancelCurrentPathRequest();
            continueFinding = true;

            while (continueFinding)
                yield return MyFindPath(target.transform.position, returnFirstPoint);
        }
        
        private IEnumerator MyFindPath(Vector3 target, Action<Vector3> returnFirstPoint)
        {
            seeker.CancelCurrentPathRequest();
            seeker.StartPath(transform.position, target, (Path path) => this.path = path);
            //Debug.Log(target);

            while (!seeker.IsDone())
                yield return new WaitForSeconds(0.5f);

            if (!path.error)
            {
                returnFirstPoint(path.vectorPath[0]);
                currentWayPoint = 1;
                PathFinded = true;
            }
        }
        #endregion
    }
}
