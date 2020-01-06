using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAIController : AIStateManager
    {
        private void Start()
        {
            nowState = new AIIdel();
            nowState.SetManager(this);
            Senser.OnPlayerCloseBy += (bool data) => PlayerCloseby(data);
        }

        private void PlayerCloseby(bool playerCloseby)
        {
            findPlayer = playerCloseby;

            if (playerCloseby)
            {
                if (Vector3.Distance(Character.transform.position, Senser.PlayerPosition) > AISetting.AttackDistance)
                    SetState(new AIChase());
                else
                    SetState(new AIAttack());
            }
            else
                SetState(new AIIdel());
        }

        #region AIState
        private class IBasicAIState : AIState
        {
            protected bool? pathFinded;
            protected Vector3 nextPoint;

            protected void PathFinded(bool? finded)
            {
                pathFinded = finded;
                manager.Senser.NextWayPoint(out nextPoint);
            }
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
                    Quaternion.AngleAxis(degree, Vector3.forward) * (Vector3.one * distance)
                    , PathFinded);
            }

            public override void Update()
            {
                if (pathFinded == true)
                {
                    if (Vector3.Distance(nextPoint, manager.Character.transform.position)
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
                if (pathFinded == true)
                {
                    if (Vector3.Distance(manager.Senser.PlayerPosition, manager.Character.transform.position) < manager.AISetting.AttackDistance
                        && manager.Character.RunTimeData.AttackTimer <= 0)
                    {
                        manager.SetState(new AIAttack());
                    }
                    else if (Vector3.Distance(nextPoint, manager.Character.transform.position)
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
                    (manager.Senser.PlayerPosition - manager.Character.transform.position).normalized);
                manager.Character.LightAttack();
            }

            public override void Update()
            {
                if (Vector3.Distance(manager.Senser.PlayerPosition, manager.Character.transform.position) > manager.AISetting.AttackDistance)
                    manager.SetState(new AIChase());

                if (manager.Character.RunTimeData.AttackTimer <= 0)
                {
                    manager.Character.Move(
                        (manager.Senser.PlayerPosition - manager.Character.transform.position).normalized);
                    manager.Character.LightAttack();
                }
            }
        }
        #endregion
    }
}