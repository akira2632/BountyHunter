using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_SpecailAttack2 : TutorialBase
    {
        [Header("TutorialSetting")]
        [Min(1)]
        public int DeffendTime = 3;
        private int counter;

        protected override void ChildrenStart()
        {
            counter = 0;
            warriorEventsManager.OnWarriorSpecailAttack2 += TutorialUpdate;
        }

        private void TutorialUpdate()
        {
            if (!tutorialReady)
                return;

            counter++;

            if (counter >= DeffendTime)
            {
                EndAnimate();
                warriorEventsManager.OnWarriorSpecailAttack2 -= TutorialUpdate;
            }
        }
    }
}