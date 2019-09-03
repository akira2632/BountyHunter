using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAIController : AIStateManager
    {
        private void Start()
        {
            nowState = new AIIdel();
            nowState.SetManager(this);
            Senser.OnPlayerCloseBy += (bool data) => playerCloseBy = data;
        }

        protected override void ManagerUpdate()
        {
            if (!playerCloseBy)
                SetState(new AIIdel());
        }

        #region AIState
        private class IBasicAIState : AIState { }

        private class AIIdel : IBasicAIState
        {
            float idelTimer;

            public override void Initial()
            {
                Debug.Log("Idel Start");
                idelTimer = Random.Range
                    (manager.AISetting.IdelTimeMin, manager.AISetting.IdelTimeMax);

                manager.Character.Move(Vertical.None);
                manager.Character.Move(Horizontal.None);
            }

            public override void Update()
            {
                idelTimer -= Time.deltaTime;
                if (idelTimer < 0)
                    manager.SetState(new AIWandering());
            }

            public override void End()
            {
                Debug.Log("Idel End");
            }
        }

        private class AIWandering : IBasicAIState
        {
            int wanderingCount;
            bool? pathFinded;

            Vector3 nextPoint;

            public AIWandering(int wanderingCount = 0)
            {
                this.wanderingCount = wanderingCount + 1;
            }

            #region 流程控制
            public override void Initial()
            {
                Debug.Log("Wandering " + wanderingCount + " Start");

                pathFinded = false;
                float distance = Random.Range
                    (manager.AISetting.WounderDistanceMin, manager.AISetting.WounderDistanceMax);
                float degree = Random.Range(0, 360);

                manager.Senser.FindPath(manager.Character.transform.position,
                    manager.Character.transform.position +
                    Quaternion.AngleAxis(degree, Vector3.forward) * (Vector3.one * distance)
                    , PathFinded);
            }

            public override void Update()
            {
                if (pathFinded == true)
                {
                    if (Vector3.Distance(nextPoint, manager.Character.transform.position) > 0.3)
                    {
                        Vertical vertical;
                        Horizontal horizontal;

                        MoveDirection(
                            (nextPoint - manager.Character.transform.position).normalized, out vertical, out horizontal);
                        manager.Character.Move(vertical);
                        manager.Character.Move(horizontal);
                    }
                    else if (!manager.Senser.NextWayPoint(out nextPoint))
                    {
                        if (wanderingCount < manager.AISetting.WounderTurnMax)
                            manager.SetState(new AIWandering(wanderingCount));

                        manager.SetState(new AIIdel());
                    }
                }
                else if (pathFinded == null)
                    manager.SetState(new AIWandering(wanderingCount - 1));
            }

            public override void End()
            {
                Debug.Log("Wandering " + wanderingCount + " End");
                base.End();
            }
            #endregion

            private void PathFinded(bool? finded)
            {
                pathFinded = finded;
                manager.Senser.NextWayPoint(out nextPoint);
            }

            private void MoveDirection(Vector3 direction, out Vertical vertical, out Horizontal horizontal)
            {
                if (direction.y < -0.5)
                    vertical = Vertical.Down;
                else if (direction.y > 0.5)
                    vertical = Vertical.Top;
                else
                    vertical = Vertical.None;

                if (direction.x < -0.5)
                    horizontal = Horizontal.Left;
                else if (direction.x > 0.5)
                    horizontal = Horizontal.Right;
                else
                    horizontal = Horizontal.None;
            }
        }
        #endregion
    }
}