using Pathfinding;
using System.Collections;
using UnityEngine;

namespace CharacterSystem.Controller
{
    [RequireComponent(typeof(Seeker))]
    public class BasicAISenser : MonoBehaviour
    {
        [SerializeField]
        private Seeker seeker;
        private Path path;

        private int currentWayPoint = 0;
        private bool continueFinding = false;

        private void Start()
        {
            seeker = GetComponent<Seeker>();
        }

        public bool PathFinded { get; private set; } = false;

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
        
        public void FindPath(Vector3 target)
        {
            PathFinded = false;
            StartCoroutine(MyFindPath(target));
        }

        public void FindPath(Transform target)
        {
            PathFinded = false;
            StartCoroutine(ContinueFinding(target));
        }

        public void StopFindPath() => continueFinding = false;

        #region A*Seeker
        private IEnumerator ContinueFinding(Transform target)
        {
            seeker.CancelCurrentPathRequest();
            continueFinding = true;

            while (continueFinding)
                yield return MyFindPath(target.transform.position);
        }
        
        private IEnumerator MyFindPath(Vector3 target)
        {
            seeker.CancelCurrentPathRequest();
            seeker.StartPath(transform.position, target, (Path path) => this.path = path);
            //Debug.Log(target);

            while (!seeker.IsDone())
                yield return new WaitForSeconds(0.5f);

            if (!path.error)
            {
                currentWayPoint = 0;
                PathFinded = true;
            }
        }
        #endregion
    }
}
