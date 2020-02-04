using UnityEngine;

namespace CharacterSystem.Controller
{
    public class BasicAIController : MonoBehaviour
    {
        public CharacterActionController Character;
        public BasicAISenser Senser;
        public BasicAISetting AISetting;

        private GameObject player;

        #region StateControl
        private bool isInitial = false;
        private IBasicAIState nowState;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>().MyCharacter.gameObject;
        }

        private void OnEnable()
        {
            SetState(new AIIdel());
        }

        private void Update()
        {
            if (!isInitial)
            {
                nowState.Initial();
                isInitial = true;
            }

            nowState.Update();
        }

        private void SetState(IBasicAIState nextState)
        {
            nowState?.End();
            isInitial = false;
            nowState = nextState;
            nowState.SetManager(this);
        }
        #endregion

        #region AIState
        protected abstract class IBasicAIState
        {
            protected BasicAIController manager;
            protected bool? pathFinded;
            protected Vector3 nextPoint;

            public void SetManager(BasicAIController manager)
                => this.manager = manager;

            protected void PathFinded(bool? finded)
            {
                pathFinded = finded;
                manager.Senser.NextWayPoint(out nextPoint);
            }

            public virtual void Initial() { }
            public virtual void Update() { }
            public virtual void End() { }
        }

        protected class AIIdel : IBasicAIState
        {
            float idelTimer;

            public override void Initial()
            {
                //Debug.Log("Idel Start");

                idelTimer = Random.Range
                    (manager.AISetting.IdelTimeMin, manager.AISetting.IdelTimeMax);

                manager.Character.Move(Vector2.zero);
            }

            public override void Update()
            {
                idelTimer -= Time.deltaTime;
                if (idelTimer < 0)
                    manager.SetState(new AIWandering());

                if (IsometricUtility.ToIsometricDistance
                        (manager.Character.transform.position, manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase());
            }
        }

        protected class AIWandering : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("Wandering Start");

                pathFinded = false;
                float distance = Random.Range
                    (manager.AISetting.WounderDistanceMin, manager.AISetting.WounderDistanceMax);
                float degree = Random.Range(0, 360);

                manager.Senser.FindPath(manager.Character.transform.position +
                    IsometricUtility.ToIsometricVector3(
                    Quaternion.AngleAxis(degree, Vector3.forward) * (Vector3.one * distance))
                    , PathFinded);
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance
                        (manager.Character.transform.position, manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase());

                if (pathFinded == true)
                {
                    if (IsometricUtility.ToIsometricDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            (nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                        manager.SetState(new AIIdel());
                }
                else if (pathFinded == null)
                    manager.SetState(new AIWandering());
            }
        }

        protected class AIChase : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("Chase Start");
                pathFinded = false;
                manager.Senser.FindPath(manager.player.transform, PathFinded);
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position,
                    manager.player.transform.position) > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel());

                if (pathFinded == true)
                {
                    if (IsometricUtility.ToIsometricDistance(manager.player.transform.position, manager.Character.transform.position) < manager.AISetting.AttackDistance
                        && manager.Character.CharacterData.BasicAttackTimer <= 0)
                    {
                        manager.SetState(new AIAttack());
                    }
                    else if (IsometricUtility.ToIsometricDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            (nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                    {
                        manager.SetState(new AIChase());
                    }
                }
                else if (pathFinded == null)
                {
                    manager.SetState(new AIIdel());
                }
            }

            public override void End()
            {
                manager.Senser.StopFindPath();
            }
        }

        protected class AIAttack : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("AttackStart");
                manager.Character.Move(
                    (manager.player.transform.position - manager.Character.transform.position).normalized);
                manager.Character.BasicAttack();
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position,
                    manager.player.transform.position) > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel());

                if (IsometricUtility.ToIsometricDistance(manager.player.transform.position,
                    manager.Character.transform.position) > manager.AISetting.AttackDistance)
                    manager.SetState(new AIChase());

                if (manager.Character.CharacterData.BasicAttackTimer <= 0)
                {
                    manager.Character.Move(
                        (manager.player.transform.position - manager.Character.transform.position).normalized);
                    manager.Character.BasicAttack();
                }
            }
        }
        #endregion
    }
}