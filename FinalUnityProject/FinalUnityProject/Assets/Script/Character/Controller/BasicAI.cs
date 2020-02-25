using UnityEngine;

namespace Character.Controller
{
    public class BasicAI : MonoBehaviour
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
            protected BasicAI manager;
            protected Vector3 nextPoint;

            public void SetManager(BasicAI manager)
                => this.manager = manager;

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

                if (IsometricUtility.ToDistance(manager.Character.transform.position, manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase());
            }
        }

        protected class AIWandering : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("Wandering Start");
                float distance = Random.Range
                    (manager.AISetting.WounderDistanceMin, manager.AISetting.WounderDistanceMax);
                float degree = Random.Range(0, 360);

                manager.Senser.FindPath(manager.Character.transform.position +
                    IsometricUtility.ToVector3(
                    Quaternion.AngleAxis(degree, Vector3.forward) * Vector3.right)
                    * distance,
                    (Vector3 nextPoint) => this.nextPoint = nextPoint);
            }

            public override void Update()
            {
                if (IsometricUtility.ToDistance(manager.Character.transform.position, manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase());

                if (manager.Senser.PathFinded)
                {
                    if (IsometricUtility.ToDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            IsometricUtility.ToVector2(manager.Character.transform.position, nextPoint));
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                        manager.SetState(new AIIdel());
                }
            }
        }

        protected class AIChase : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("Chase Start");
                manager.Senser.FindPath(manager.player.transform,
                    (Vector3 nextPoint) => this.nextPoint = nextPoint);
            }

            public override void Update()
            {
                if (IsometricUtility.ToDistance(manager.Character.transform.position, manager.player.transform.position)
                    > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel());

                if (manager.Senser.PathFinded)
                {
                    if (IsometricUtility.ToDistance(manager.player.transform.position, manager.Character.transform.position) < manager.AISetting.AttackDistance
                        && manager.Character.CharacterData.BasicAttackTimer <= 0)
                    {
                        manager.SetState(new AIAttack());
                        return;
                    }

                    if (IsometricUtility.ToDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            IsometricUtility.ToVector2(manager.Character.transform.position, nextPoint));
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                    {
                        manager.SetState(new AIChase());
                    }
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
                manager.Character.Move(IsometricUtility.ToVector2(
                    manager.Character.transform.position, manager.player.transform.position));
                manager.Character.BasicAttack();
            }

            public override void Update()
            {
                if (IsometricUtility.ToDistance(manager.Character.transform.position,
                    manager.player.transform.position) > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel());

                if (IsometricUtility.ToDistance(manager.player.transform.position,
                    manager.Character.transform.position) > manager.AISetting.AttackDistance)
                    manager.SetState(new AIChase());

                if (manager.Character.CharacterData.BasicAttackTimer <= 0)
                {
                    manager.Character.Move(IsometricUtility.ToVector2(
                        manager.Character.transform.position, manager.player.transform.position));
                    manager.Character.BasicAttack();
                }
            }
        }
        #endregion
    }
}