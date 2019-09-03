using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class BasicAIController : AIStateManager
    {


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
                    manager.SetState(new AIWandering());
            }
        }

        private class AIWandering : IBasicAIState
        {
            int wanderingCount;
            float distance;
            Vector2 degree;

            public override void Initial()
            {
                wanderingCount = 0;
                distance = Random.Range
                    (manager.setting.WounderDistanceMin, manager.setting.WounderDistanceMax);
            }

            public override void Update()
            {
                base.Update();
            }

            public override void End()
            {
                base.End();
            }
        }
    }
}