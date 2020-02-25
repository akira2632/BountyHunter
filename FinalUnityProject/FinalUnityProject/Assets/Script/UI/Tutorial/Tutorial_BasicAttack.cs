using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_BasicAttack : TutorialBase
    {
        [Header("BasicAttackTutorialSetting")]
        [Min(1)]
        public int AttackTime = 3;
        private int counter;

        protected override void ChildrenStart()
        {
            counter = 0;
            warriorEventsManager.OnWarriorBasicAttack += tutorialUpdate;
        }

        private void tutorialUpdate()
        {
            if (!tutorialReady)
                return;

            counter++;

            if (counter >= AttackTime)
                EndAnimate();
        }
    }
}