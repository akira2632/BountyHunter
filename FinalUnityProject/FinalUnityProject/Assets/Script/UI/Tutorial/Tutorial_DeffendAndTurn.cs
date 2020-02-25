using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_DeffendAndTurn : TutorialBase
    {
        [Header("TutorialSetting")]
        [Min(0)]
        public float TurningTime = 3;
        private float timer;

        protected override void ChildrenStart()
        {
            timer = 0;
            warriorEventsManager.OnWarriorTurnInDeffend += TutorialUpdate;
        }

        protected void TutorialUpdate(float moveTime)
        {
            if (!tutorialReady)
                return;

            timer += moveTime;

            if (timer >= TurningTime)
            {
                EndAnimate();
                warriorEventsManager.OnWarriorTurnInDeffend -= TutorialUpdate;
            }
        }
    }
}
