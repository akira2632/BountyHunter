using UnityEngine;
using Character;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialBase : MonoBehaviour
{
    public GameObject NextTutorial;
    [Header("AnimateSetting")]
    public float StartTime;
    [Range(0, 1)]
    public float HightLightTimeOffset;
    public float EndTime;
    [ColorUsage(true)]
    public Color StartColor, HightLightColor, EndColor;

    protected WarriorEventsManager warriorEventsManager;
    protected Image tutorialImage;
    protected bool tutorialReady;

    private void Start()
    {
        tutorialReady = false;
        tutorialImage = GetComponent<Image>();
        warriorEventsManager = GetComponentInParent<WarriorEventsManager>();

        ChildrenStart();
        StartAnimate();
    }

    protected virtual void ChildrenStart() { }

    protected void EndAnimate()
    {
        tutorialImage.DOColor(EndColor, EndTime).onComplete += () =>
        {
            if(NextTutorial != null)
                NextTutorial.SetActive(true);

            gameObject.SetActive(false);
        };
    }

    protected void StartAnimate()
    {
        DOTween.Sequence()
            .Append(tutorialImage.DOColor(HightLightColor, StartTime * HightLightTimeOffset).ChangeStartValue(StartColor))
            .Append(tutorialImage.DOColor(Color.white, StartTime * (1 - HightLightTimeOffset)))
            .onComplete += () => tutorialReady = true;
    }
}
