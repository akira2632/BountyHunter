using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CharacterSystem_V4;

public class HitEffector : MonoBehaviour
{
    #region 輔助用結構
    public enum SpriteDirection { Left = -1, None, Right }

    [System.Serializable]
    public struct EffectPrafeb
    {
        public GameObject effectPrafeb;
        [Tooltip("效果的預設方向")]
        public SpriteDirection Direction;
    }
    #endregion

    #region 設定
    #region 測試設定
    [Header("TestSetting")]
    [Min(0)]
    public int test_Damage;
    public bool test_ShowDamage;
    public Vector2 test_HitAt;
    #endregion
    #region 預製物設定
    [Space(10)]
    [Header("PrafebSetting")]
    public GameObject[] NumberPrafebs;
    public EffectPrafeb[] HitEffectPrafebs, DeffendEffectPrafebs;
    [Tooltip("文字的間距")]
    public float NumberPitch;
    #endregion
    #region 傷害數字效果設定
    [Space(10)]
    [Header("NumberEffectSetting")]
    public float NumberEffectTime;
    public float NumberEndHight, NumberEndVertical;
    public Color NumberEndColor;
    public Ease NumberColorEase = Ease.InSine, 
        NumberHorizontalEase = Ease.InOutCubic,
        NumberVerticalEase = Ease.OutBack;
    public bool EnableHorizontal_Number = true;
    #endregion
    #region 擊中特效設定
    [Space(10)]
    [Header("HitEffectSetting")]
    public float HitEffectTime;
    public float HitEndHight, HitEndVertical;
    public Color HitEndColor;
    public Ease HitColorEase = Ease.InSine,
        HitHorizontalEase = Ease.InOutCubic,
        HitVerticalEase = Ease.OutBack;
    public bool EnableHorizontal_Hit = true;
    #endregion
    #endregion

    #region Inspector功能
    [ContextMenu("PlayHitEffect")]
    private void PlayHitEffect()
    {
        PlayHitEffect(new DamageData() 
        { 
            Damage = test_Damage,
            HitAt = test_HitAt,
            HitFrom = transform.position
        }, test_ShowDamage);
    }

    [ContextMenu("PlayDeffendEffect")]
    private void PlayDeffendEffect()
    {
        PlayDeffendEffect(new DamageData() 
        { 
            Damage = test_Damage,
            HitAt = test_HitAt,
            HitFrom = transform.position
        }, test_ShowDamage);
    }
    #endregion

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

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.DOColor(NumberEndColor, NumberEffectTime).SetEase(NumberColorEase);
        }
        
        temp.transform.DOBlendableMoveBy(
            transform.position + new Vector3(0, NumberEndHight), NumberEffectTime)
            .SetEase(NumberVerticalEase)
            .onComplete += () => Destroy(temp);

        if(EnableHorizontal_Number)
        {
            var horizontalOffset = new Vector3(damage.HitFrom.x - damage.HitAt.x, 0).normalized * NumberEndVertical;
            //Debug.Log(horizontalOffset);
            temp.transform.DOBlendableMoveBy(
                transform.position + horizontalOffset, NumberEffectTime)
                .SetEase(NumberHorizontalEase);
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
