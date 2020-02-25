using UnityEngine;

namespace UI.Tutorial
{
    public class Tutorial_Deffend : TutorialBase
    {
        [Header("TutorialSetting")]
        [Min(1)]
        public int DeffendTime = 3;
        private int counter;

        protected override void ChildrenStart()
        {
            counter = 0;
            warriorEventsManager.OnWarriorDeffend += TutorialUpdate;
        }

        private void TutorialUpdate()
        {
            if (!tutorialReady)
                return;

            counter++;

            if (counter >= DeffendTime)
            {
                EndAnimate();
                warriorEventsManager.OnWarriorDeffend -= TutorialUpdate;
            }
        }
    }
}
