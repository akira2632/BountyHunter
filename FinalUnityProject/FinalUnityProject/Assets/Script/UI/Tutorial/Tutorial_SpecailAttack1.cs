using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_SpecailAttack1 : TutorialBase
    {
        [Header("TutorialSetting")]
        [Min(1)]
        public int DeffendTime = 3;
        private int counter;

        protected override void ChildrenStart()
        {
            counter = 0;
            warriorEventsManager.OnWarriorSpecailAttack1 += TutorialUpdate;
        }

        private void TutorialUpdate()
        {
            if (!tutorialReady)
                return;

            counter++;

            if (counter >= DeffendTime)
            {
                EndAnimate();
                warriorEventsManager.OnWarriorSpecailAttack1 -= TutorialUpdate;
            }
        }
    }
}