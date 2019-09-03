using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAIController : AIStateManager
    {
        private void Start()
        {
            nowState = new AIIdel();
        }

        private class IBasicAIState : AIState
        {

        }

        private class AIIdel : IBasicAIState
        {
            float idelTimer;

            public override void Initial()
            {
                idelTimer = Random.Range
                    (manager.setting.IdelTimeMin, manager.setting.IdelTimeMax);
            }

            public override void Update()
            {
                idelTimer -= Time.deltaTime;
                if (idelTimer < 0)
                    manager.SetState(new AIWandering(0));
            }
        }

        private class AIWandering : IBasicAIState
        {
            int wanderingCount;
            bool? pathFinded;
            float distance, degree;
            Vector3 nextPoint;

            public AIWandering(int wanderingCount)
            {
                this.wanderingCount = wanderingCount + 1;
            }

            #region 流程控制
            public override void Initial()
            {
                if (wanderingCount <= manager.setting.WounderTurnMax)
                {
                    pathFinded = false;
                    distance = Random.Range
                        (manager.setting.WounderDistanceMin, manager.setting.WounderDistanceMax);
                    degree = Random.Range(0, 360);

                    manager.senser.FindPath(manager.character.transform.position,
                        manager.character.transform.position +
                        Quaternion.AngleAxis(degree, Vector3.up) *
                        new Vector3(distance, 0), PathFinded);
                }
                else
                    manager.SetState(new AIIdel());
            }


            public override void Update()
            {
                if (pathFinded == true)
                {
                    if (Vector3.Distance(nextPoint, manager.character.transform.position) > 0.3f)
                    {
                        manager.character.Move(VerticalDirection
                            (manager.character.transform.position.x, nextPoint.x));
                        manager.character.Move(HorizontalDirection
                            (manager.character.transform.position.y, nextPoint.y));
                    }
                    else if (manager.senser.NextWayPoint(out nextPoint))
                        manager.SetState(new AIIdel());
                }
                else if (pathFinded == null)
                    manager.SetState(new AIWandering(wanderingCount - 1));
            }

            public override void End()
            {
                base.End();
            }
            #endregion

            private void PathFinded(bool? finded)
            {
                    pathFinded = finded;
                    manager.senser.NextWayPoint(out nextPoint);
            }

            private Vertical VerticalDirection(float from, float to)
            {
                float dirction = from - to;
                if (dirction < -0.1)
                    return Vertical.Down;
                else if (dirction > 0.1)
                    return Vertical.Top;
                else
                    return Vertical.None;
            }

            private Horizontal HorizontalDirection(float from, float to)
            {
                float dirction = from - to;
                if (dirction < -0.1)
                    return Horizontal.Left;
                else if (dirction > 0.1)
                    return Horizontal.Right;
                else
                    return Horizontal.None;
            }
        }
    }
}