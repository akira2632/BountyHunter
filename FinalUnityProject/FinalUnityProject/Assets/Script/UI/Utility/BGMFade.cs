using DG.Tweening;
using UnityEngine;

public class BGMFade : MonoBehaviour
{
    public AudioClip FadeInClip, FadeOutClip;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void FadeIn()
    {
        audio.DOKill();
        DOTween.Sequence()
            .Append(audio.DOFade(0, 1).OnComplete(() => 
                { 
                    audio.clip = FadeInClip;
                    audio.Play();
                }))
            .Append(audio.DOFade(1, 1));
    }

    public void FadeOut()
    {
        audio.DOKill();
        DOTween.Sequence()
            .Append(audio.DOFade(0, 1).OnComplete(() =>
                {
                    audio.clip = FadeOutClip;
                    audio.Play();
                }))
            .Append(audio.DOFade(1, 1));
    }
}
