using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAISenser : MonoBehaviour
    {
        public event Action<bool> OnPlayerCloseBy;

        public GameObject Character;

        [SerializeField]
        private Seeker seeker;
        private Path path;

        private GameObject player;
        private int currentWayPoint = 0;
        private bool continueFinding = false, playerCloseBy = false;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>().MyCharacter.gameObject;
        }

        private void Update()
        {
            if (playerCloseBy &&
                IsometricUtility.ToIsometricDistance(
                Character.transform.position, player.transform.position) > 10)
            {
                playerCloseBy = false;
                OnPlayerCloseBy?.Invoke(false);
            }
            else if (!playerCloseBy &&
                IsometricUtility.ToIsometricDistance(
                Character.transform.position, player.transform.position) <= 10)
            {
                playerCloseBy = true;
                OnPlayerCloseBy?.Invoke(true);
            }
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

        public void FindPath(Vector3 target, Action<bool?> pathFinded)
            => StartCoroutine(MyFindPath(target, pathFinded));

        public void FindPathToPlayer(Action<bool?> pathFinded)
            => StartCoroutine(ContinueFinding(pathFinded));

        public void StopFindPathToPlayer() => continueFinding = false;

        #region A*Seeker
        private IEnumerator ContinueFinding(Action<bool?> pathFinded)
        {
            seeker.CancelCurrentPathRequest();
            continueFinding = true;

            while (continueFinding)
                yield return MyFindPath(player.transform.position, pathFinded);
        }

        private IEnumerator MyFindPath(Vector3 target, Action<bool?> pathFinded)
        {
            seeker.CancelCurrentPathRequest();
            seeker.StartPath(transform.position, target, (Path path) => this.path = path);
            //Debug.Log(target);

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
        #endregion
    }
}
