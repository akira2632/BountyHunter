using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_BasicAttack : TutorialBase
    {
        [Header("TutorialSetting")]
        [Min(1)]
        public int AttackTime = 3;
        private int counter;

        protected override void ChildrenStart()
        {
            counter = 0;
            warriorEventsManager.OnWarriorBasicAttack += TutorialUpdate;
        }

        private void TutorialUpdate()
        {
            if (!tutorialReady)
                return;

            counter++;

            if (counter >= AttackTime)
            {
                EndAnimate();
                warriorEventsManager.OnWarriorBasicAttack -= TutorialUpdate;
            }
        }
    }
}