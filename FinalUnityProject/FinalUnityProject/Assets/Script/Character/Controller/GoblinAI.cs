using UnityEngine;

namespace Character.Controller
{
    public class GoblinAI : MonoBehaviour
    {
        public MemberType MemberType;

        public CharacterActionController Character;
        public BasicAISenser Senser;
        public GoblinAISetting AISetting;

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
            SetState(new AIIdel(this));
            MemberType = MemberType.None;
            GoblinAITeam.Instance.AddToTeam(this);
        }

        private void Update()
        {
            if (Character.CharacterData.Health <= 0)
            {
                GoblinAITeam.Instance.RemoveFromTeam(this);
                return;
            }

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
        }
        #endregion

        #region AIState
        protected abstract class IBasicAIState
        {
            protected GoblinAI manager;
            protected Vector3 nextPoint;

            public IBasicAIState(GoblinAI manager)
            {
                this.manager = manager;
            }

            public virtual void Initial() { }
            public virtual void Update() { }
            public virtual void End() { }
        }

        protected class AIIdel : IBasicAIState
        {
            float idelTimer;

            public AIIdel(GoblinAI manager) : base(manager)
            { }

            public override void Initial()
            {
                //Debug.Log("Idel Start");

                idelTimer = Random.Range(manager.AISetting.IdelTimeMin, manager.AISetting.IdelTimeMax);

                manager.Character.Move(Vector2.zero);
            }

            public override void Update()
            {
                idelTimer -= Time.deltaTime;
                if (idelTimer < 0)
                    manager.SetState(new AIWandering(manager));

                if (manager.Character.transform.position.IsoDistance(manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase(manager));
            }
        }

        protected class AIWandering : IBasicAIState
        {
            public AIWandering(GoblinAI manager) : base(manager)
            {
            }

            public override void Initial()
            {
                //Debug.Log("Wandering Start");

                float distance = Random.Range
                    (manager.AISetting.WounderDistanceMin, manager.AISetting.WounderDistanceMax);
                float degree = Random.Range(0, 360);

                manager.Senser.FindPath(manager.Character.transform.position
                    + (Quaternion.AngleAxis(degree, Vector3.forward) * Vector3.right).IsoNormalized()
                    * distance,
                    (Vector3 nextPoint) => this.nextPoint = nextPoint);
            }

            public override void Update()
            {
                if (manager.Character.transform.position.IsoDistance(manager.player.transform.position)
                    <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase(manager));

                if (manager.Senser.PathFinded)
                {
                    if (nextPoint.IsoDistance(manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move((nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                        manager.SetState(new AIIdel(manager));
                }
            }
        }

        protected class AIChase : IBasicAIState
        {
            public AIChase(GoblinAI manager) : base(manager)
            {
            }

            public override void Initial()
            {
                //Debug.Log("Chase Start");
                manager.Senser.FindPath(manager.player.transform,
                    (Vector3 nextPoint) => this.nextPoint = nextPoint);
            }

            public override void Update()
            {
                if (manager.Character.transform.position.IsoDistance(manager.player.transform.position)
                    > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel(manager));

                if (manager.Senser.PathFinded)
                {
                    if (manager.MemberType == MemberType.BasicAttacker
                        && manager.player.transform.position.IsoDistance(manager.Character.transform.position)
                        < manager.AISetting.BasicAttackDistance)
                    {
                        manager.SetState(new AIBasicAttack(manager));
                        return;
                    }

                    if (manager.MemberType == MemberType.SpacilAttacker
                        && manager.player.transform.position.IsoDistance(manager.Character.transform.position)
                        < manager.AISetting.SpacilAttackDistance)
                    {
                        manager.SetState(new AISpacilAttack(manager));
                        return;
                    }

                    if (nextPoint.IsoDistance(manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move((nextPoint - manager.Character.transform.position).normalized);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                    {
                        manager.SetState(new AIChase(manager));
                    }
                }
            }

            public override void End()
            {
                manager.Character.Move(Vector2.zero);
                manager.Senser.StopFindPath();
            }
        }

        protected class AIAround : IBasicAIState
        {
            private Vector3 targetPoint;
            private float angle;
            private int roundTurnCount;

            private AIAround(GoblinAI manager, float angle, int roundTurnCount) : base(manager)
            {
                targetPoint = manager.player.transform.position
                    + (Quaternion.AngleAxis(angle, Vector3.forward)
                    * (manager.Character.transform.position - manager.player.transform.position)).IsoNormalized()
                    * manager.AISetting.AroundRadius;

                this.angle = angle;
                this.roundTurnCount = roundTurnCount - 1;
                manager.Senser.FindPath(targetPoint,
                    (Vector3 nextPoint) => this.nextPoint = nextPoint);
                //Debug.Log($"AroundStart turnCount = {roundTurnCount}");
            }

            public AIAround(GoblinAI manager) : base(manager)
            {
                targetPoint = manager.player.transform.position
                    + (manager.Character.transform.position - manager.player.transform.position).IsoNormalized()
                    * manager.AISetting.AroundRadius;

                angle = Random.Range(1, 10) > 5 ? -manager.AISetting.AroundDegree : manager.AISetting.AroundDegree;
                roundTurnCount = manager.AISetting.RoundTurn;
                manager.Senser.FindPath(targetPoint,
                    (Vector3 nextPoint) => this.nextPoint = nextPoint);
                //Debug.Log($"AroundStart turnCount = {roundTurnCount}");
            }

            public override void Update()
            {
                if (manager.Senser.PathFinded)
                {
                    if (nextPoint.IsoDistance(manager.Character.transform.position)
                        > manager.AISetting.StopDistance)
                    {
                        manager.Character.Move((nextPoint - manager.Character.transform.position).normalized);
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

            public override void End()
            {
                manager.Character.Move(Vector2.zero);
            }
        }

        protected class AIBasicAttack : IBasicAIState
        {
            public AIBasicAttack(GoblinAI manager) : base(manager)
            {
            }

            public override void Update()
            {
                if (manager.Character.transform.position.IsoDistance(manager.player.transform.position)
                    > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel(manager));

                if (manager.player.transform.position.IsoDistance(manager.Character.transform.position)
                    > manager.AISetting.BasicAttackDistance)
                    manager.SetState(new AIChase(manager));

                if (manager.Character.CharacterData.BasicAttackTimer <= 0)
                {
                    manager.Character.Move((manager.player.transform.position - manager.Character.transform.position).normalized);
                    manager.Character.BasicAttack();
                }
            }
        }

        protected class AISpacilAttack : IBasicAIState
        {
            public AISpacilAttack(GoblinAI manager) : base(manager)
            {
            }

            public override void Update()
            {
                if (manager.Character.transform.position.IsoDistance(manager.player.transform.position)
                    > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel(manager));

                if (manager.player.transform.position.IsoDistance(manager.Character.transform.position)
                    > manager.AISetting.SpacilAttackDistance)
                    manager.SetState(new AIChase(manager));

                if (manager.Character.CharacterData.SpacilAttackTimer <= 0)
                {
                    manager.Character.Move(manager.player.transform.position - manager.Character.transform.position);
                    manager.Character.SpecialAttack(manager.player.transform.position + new Vector3(0, 0.8f));
                    manager.SetState(new AIAround(manager));
                }
            }
        }
        #endregion
    }
}