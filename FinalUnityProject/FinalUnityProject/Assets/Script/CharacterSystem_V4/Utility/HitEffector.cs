using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CharacterSystem_V4;

public class HitEffector : MonoBehaviour
{
    public enum SpriteDirection { Left = -1, None, Right }

    [System.Serializable]
    public struct EffectPrafeb
    {
        public GameObject effectPrafeb;
        [Tooltip("效果的預設方向")]
        public SpriteDirection Direction;
    }

    [Header("TestSetting")]
    [Min(0)]
    public int test_Damage;
    public bool test_ShowDamage;
    public Vector2 test_HitAt;

    [Space(10)]
    [Header("PrafebSetting")]
    public GameObject[] NumberPrafebs;
    public EffectPrafeb[] HitEffectPrafebs, DeffendEffectPrafebs;
    [Tooltip("文字的間距")]
    public float NumberPitch;

    [Space(10)]
    [Header("EffectSetting")]
    public float HitEffectTime; 
    public float NumberEffectTime;
    public float EndHight, EndVertical;
    public bool EnableHorizontal = true;

    [ContextMenu("PlayHitEffect")]
    private void PlayHitEffect()
    {
        PlayDeffendEffect(new DamageData() 
        { 
            Damage = test_Damage,
            HitAt = test_HitAt,
            HitFrom = transform.position
        }, test_ShowDamage);
    }

    public void PlayHitEffect(DamageData damage, bool showNumber = true)
    {
        if(showNumber)
            ShowHitNumber(damage);
        ShowSprite(HitEffectPrafebs[Random.Range(0, HitEffectPrafebs.Length - 1)], damage);
    }

    public void PlayDeffendEffect(DamageData damage, bool showNumber = true)
    {
        if (showNumber)
            ShowHitNumber(damage);
        ShowSprite(DeffendEffectPrafebs[Random.Range(0, DeffendEffectPrafebs.Length - 1)], damage);
    }

    private void ShowHitNumber(DamageData damage)
    {
        var temp = new GameObject($"HitNumber_{damage.Damage}");
        temp.transform.position = damage.HitAt;
        var renderers = AssambellyNumber(damage.Damage, temp);

        var endColor = new Color(1, 1, 1, 0);
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.DOColor(endColor, NumberEffectTime).SetEase(Ease.InExpo);
        }
        
        temp.transform.DOBlendableMoveBy(
            transform.position + new Vector3(0, EndHight), NumberEffectTime)
            .SetEase(Ease.OutExpo)
            .onComplete += () => Destroy(temp);

        if(EnableHorizontal)
        {
            //temp.transform.DOBlendableMoveBy();
        }

        SpriteRenderer[] AssambellyNumber(int value, GameObject parent)
        {
            var numbers = GetNumbers(value);
            SpriteRenderer[] spriteRenderers = new SpriteRenderer[numbers.Length];
            float numberOffset = (numbers.Length - 1) * NumberPitch * 0.5f;
            for (int i = 0; i < numbers.Length; i++)
            {
                spriteRenderers[i] = 
                    Instantiate(NumberPrafebs[numbers[i]],
                        new Vector3(numberOffset, 0),
                        Quaternion.identity,
                        parent.transform)
                    .GetComponent<SpriteRenderer>();

                numberOffset -= NumberPitch;
            }
            return spriteRenderers;
        }
        int[] GetNumbers(int orignalNumber)
        {
            if (orignalNumber > 0)
            {
                int[] numbers = new int[(int)Mathf.Log10(orignalNumber) + 1];

                for (int i = 0; i < numbers.Length; i++)
                {
                    numbers[i] = orignalNumber % 10;
                    orignalNumber /= 10;
                }

                return numbers;
            }
            else
                return new int[] { 0 };
        }
    }

    private void ShowSprite(EffectPrafeb effectSprite, DamageData damage)
    {

    }
}
