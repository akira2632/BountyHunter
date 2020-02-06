using UnityEngine;

namespace CharacterSystem.Controller
{
    public class SpiderAIController : MonoBehaviour
    {
        public CharacterActionController Character;
        public BasicAISenser Senser;
        public SpiderAISetting AISetting;

        private GameObject player;

        #region StateControl
        private bool isInitial = false;
        private ISpiderAIState nowState;

        private void Start()
        {
            player = FindObjectOfType<PlayerController>().MyCharacter.gameObject;
        }

        private void OnEnable()
        {
            SetState(new AIIdel(this));
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

        private void SetState(ISpiderAIState nextState)
        {
            nowState?.End();
            isInitial = false;
            nowState = nextState;
        }
        #endregion

        #region AIState
        protected abstract class ISpiderAIState
        {
            protected SpiderAIController manager;
            protected Vector3 nextPoint;

            public ISpiderAIState(SpiderAIController manager)
            {
                this.manager = manager;
            }

            public virtual void Initial() { }
            public virtual void Update() { }
            public virtual void End() { }
        }

        protected class AIIdel : ISpiderAIState
        {
            float idelTimer;

            public AIIdel(SpiderAIController manager) : base(manager) { }

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
                    manager.SetState(new AIWandering(manager));

                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position, manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase(manager));
            }
        }

        protected class AIWandering : ISpiderAIState
        {
            public AIWandering(SpiderAIController manager) : base(manager) { }

            public override void Initial()
            {
                //Debug.Log("Wandering Start");

                float distance = Random.Range
                    (manager.AISetting.WounderDistanceMin, manager.AISetting.WounderDistanceMax);
                float degree = Random.Range(0, 360);

                manager.Senser.FindPath(manager.Character.transform.position +
                    IsometricUtility.ToIsometricVector3(
                    Quaternion.AngleAxis(degree, Vector3.forward) * (Vector3.one * distance)));
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position, manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase(manager));

                if (manager.Senser.PathFinded)
                {
                    if (IsometricUtility.ToIsometricDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            (nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                        manager.SetState(new AIIdel(manager));
                }
            }
        }

        protected class AIChase : ISpiderAIState
        {
            public AIChase(SpiderAIController manager) : base(manager) { }

            public override void Initial()
            {
                //Debug.Log("Chase Start");

                manager.Senser.FindPath(manager.player.transform);
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position, manager.player.transform.position)
                    > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel(manager));

                if (manager.Senser.PathFinded)
                {
                    if (IsometricUtility.ToIsometricDistance(manager.player.transform.position, manager.Character.transform.position) < manager.AISetting.AttackDistance
                        && manager.Character.CharacterData.BasicAttackTimer <= 0)
                    {
                        manager.SetState(new AIAttack(manager));
                    }
                    else if (IsometricUtility.ToIsometricDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            (nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                    {
                        manager.SetState(new AIChase(manager));
                    }
                }
            }

            public override void End()
            {
                manager.Senser.StopFindPath();
            }
        }

        protected class AIAttack : ISpiderAIState
        {
            public AIAttack(SpiderAIController manager) : base(manager) { }

            public override void Initial()
            {
                //Debug.Log("AttackStart");
                manager.Character.Move(
                    (manager.player.transform.position - manager.Character.transform.position).normalized);
                manager.Character.BasicAttack();
                manager.SetState(new AIAround(manager));
            }
        }

        protected class AIAround : ISpiderAIState
        {
            private Vector3 targetPoint;
            private float angle;
            private int roundTurnCount;

            private AIAround(SpiderAIController manager, float angle, int roundTurnCount) : base(manager)
            {
                var orignalDirection = IsometricUtility.ToIsometricVector3(manager.Character.transform.position - manager.player.transform.position).normalized
                    * manager.AISetting.AroundRadius;
                var rotateDirection = Quaternion.AngleAxis(angle, Vector3.forward)
                    * orignalDirection;
                targetPoint = manager.player.transform.position + rotateDirection;
                Debug.Log($"Orignal{orignalDirection}\nRotate{rotateDirection}\nTarget{targetPoint}");

                this.angle = angle;
                this.roundTurnCount = roundTurnCount - 1;
                manager.Senser.FindPath(targetPoint);
            }

            public AIAround(SpiderAIController manager) : base(manager)
            {
                targetPoint = manager.player.transform.position
                    + IsometricUtility.ToIsometricVector3(manager.Character.transform.position - manager.player.transform.position).normalized
                    * manager.AISetting.AroundRadius;

                angle = Random.Range(1, 10) > 5 ? -manager.AISetting.AroundDegree : manager.AISetting.AroundDegree;
                roundTurnCount = manager.AISetting.RoundTurn;
                manager.Senser.FindPath(targetPoint);
            }

            public override void Update()
            {
                if (manager.Senser.PathFinded)
                {
                    if (IsometricUtility.ToIsometricDistance(nextPoint, manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move(
                            (nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                    {
                        if (roundTurnCount > 0)
                            manager.SetState(new AIAround(manager, angle, roundTurnCount));
                        else
                            manager.SetState(new AIChase(manager));
                    }
                }
            }
        }
        #endregion
    }
}