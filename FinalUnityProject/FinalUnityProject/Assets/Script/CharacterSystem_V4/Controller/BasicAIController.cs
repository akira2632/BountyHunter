using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAIController : MonoBehaviour
    {
        public ICharacterActionManager Character;
        public BasicAISenser Senser;
        public AISetting AISetting;

        private GameObject player;

        #region StateControl
        private bool isInitial = false;
        private IBasicAIState nowState;

        private void Start()
        {
            SetState(new AIIdel());
            player = FindObjectOfType<PlayerController>().MyCharacter.gameObject;
        }

        private void OnEnable()
        {
            SetState(new AIIdel());
        }

        private void Update()
        {
            if (Character == null)
                Destroy(gameObject);

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
        private abstract class IBasicAIState
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

        private class AIIdel : IBasicAIState
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

                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position,
                    manager.player.transform.position) <= manager.AISetting.DetectedDistance)
                    manager.SetState(new AIChase());
            }
        }

        private class AIWandering : IBasicAIState
        {
            #region 流程控制
            public override void Initial()
            {
                //Debug.Log("Wandering Start");

                pathFinded = false;
                float distance = Random.Range
                    (manager.AISetting.WounderDistanceMin, manager.AISetting.WounderDistanceMax);
                float degree = Random.Range(0, 360);

                manager.Senser.FindPath(manager.Character.transform.position +
                    IsometricUtility.ToIsometricVector(
                    Quaternion.AngleAxis(degree, Vector3.forward) * (Vector3.one * distance))
                    , PathFinded);
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position,
                    manager.player.transform.position) <= manager.AISetting.DetectedDistance)
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
            #endregion
        }

        private class AIChase : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("Chase Start");
                pathFinded = false;
                manager.Senser.FindPathToPlayer(PathFinded);
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position,
                    manager.player.transform.position) > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel());

                if (pathFinded == true)
                {
                    if (IsometricUtility.ToIsometricDistance(manager.player.transform.position, manager.Character.transform.position) < manager.AISetting.AttackDistance
                        && manager.Character.RunTimeData.AttackTimer <= 0)
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
                manager.Senser.StopFindPathToPlayer();
            }
        }

        private class AIAttack : IBasicAIState
        {
            public override void Initial()
            {
                //Debug.Log("AttackStart");
                manager.Character.Move(
                    (manager.player.transform.position - manager.Character.transform.position).normalized);
                manager.Character.LightAttack();
            }

            public override void Update()
            {
                if (IsometricUtility.ToIsometricDistance(manager.Character.transform.position,
                    manager.player.transform.position) > manager.AISetting.DetectedDistance)
                    manager.SetState(new AIIdel());

                if (IsometricUtility.ToIsometricDistance(manager.player.transform.position, 
                    manager.Character.transform.position) > manager.AISetting.AttackDistance)
                    manager.SetState(new AIChase());

                if (manager.Character.RunTimeData.AttackTimer <= 0)
                {
                    manager.Character.Move(
                        (manager.player.transform.position - manager.Character.transform.position).normalized);
                    manager.Character.LightAttack();
                }
            }
        }
        #endregion
    }
}