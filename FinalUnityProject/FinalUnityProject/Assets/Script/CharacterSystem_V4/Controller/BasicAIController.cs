using Pathfinding;
using System.Collections;
using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    [RequireComponent(typeof(Seeker))]
    public class BasicAIController : MonoBehaviour
    {
        #region Pathfinding
        public Seeker seeker;
        Path path;
        int currentWayPoint = 0;
        bool chaseEnd = false;
        Vector2 tartget;
        #endregion

        #region Character
        public ICharacterActionManager character;
        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update() 
        {

        }

        IEnumerator FindPath()
        {
            while(tartget != null)
            {
                if(seeker.IsDone())
                    seeker.StartPath(character.transform.position, tartget, OnPathComplete);

                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        void OnPathComplete(Path path)
        {
            if(!path.error)
            {
                this.path = path;
                currentWayPoint = 0;
            }
        }
    }
}
