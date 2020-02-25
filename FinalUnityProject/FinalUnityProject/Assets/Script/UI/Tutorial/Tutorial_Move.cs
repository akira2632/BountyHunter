using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_Move : TutorialBase
    {
        [Header("TutorialSetting")]
        [Min(0)]
        public float MoveTime = 3;
        private float timer;

        protected override void ChildrenStart()
        {
            timer = 0;
            warriorEventsManager.OnWarriorMove += TutorialUpdate;
        }

        protected void TutorialUpdate(float moveTime)
        {
            if (!tutorialReady)
                return;

            timer += moveTime;

            if (timer >= MoveTime)
            {
                EndAnimate();
                warriorEventsManager.OnWarriorMove -= TutorialUpdate;
            }
        }
    }
}
