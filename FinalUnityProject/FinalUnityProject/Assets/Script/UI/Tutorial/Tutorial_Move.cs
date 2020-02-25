using Character;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tutorial
{
    public class Tutorial_Move : MonoBehaviour
    {
        public GameObject NextTutorial;
        [Min(0)]
        public float MoveTime = 3;
        [Header("AnimateSetting")]
        public float StartTime;
        [Range(0, 1)]
        public float HightLightTimeOffset;
        public float EndTime;
        [ColorUsage(true)]
        public Color StartColor, HightLightColor, EndColor;

        private Image tutorialImage;
        private float timer;
        private bool tutorialReady;

        void Start()
        {
            timer = 0;
            tutorialReady = false;
            GetComponentInParent<WarriorEventsManager>().OnWarriorMove += TutorialUpdate;
            tutorialImage = GetComponent<Image>();

            StartAnimate();
        }


        private void TutorialUpdate(float moveTime)
        {
            if (!tutorialReady)
                return;

            timer += moveTime;
            if (timer >= MoveTime)
            {
                EndAnimate();
            }
        }

        private void EndAnimate()
        {
            tutorialImage.DOColor(EndColor, EndTime).onComplete += () =>
            {
                NextTutorial?.SetActive(true);
                gameObject.SetActive(false);
            };
        }

        private void StartAnimate()
        {
            DOTween.Sequence()
                .Append(tutorialImage.DOColor(HightLightColor, StartTime * HightLightTimeOffset).ChangeStartValue(StartColor))
                .Append(tutorialImage.DOColor(Color.white, StartTime * (1 - HightLightTimeOffset)))
                .onComplete += () => tutorialReady = true;
        }
    }
}
