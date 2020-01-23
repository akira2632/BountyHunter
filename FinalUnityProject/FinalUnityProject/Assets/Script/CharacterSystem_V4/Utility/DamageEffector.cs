using DG.Tweening;
using UnityEngine;

namespace CharacterSystem_V4
{
    public class DamageEffector : MonoBehaviour
    {
        #region 輔助用結構
        public enum EffectDirection { Left = -1, None, Right }

        [System.Serializable]
        public struct EffectPrafeb
        {
            public GameObject Prafeb;
            [Tooltip("效果的預設方向")]
            public EffectDirection DefaultDirection;
            public bool HorizontalMovement, VerticalMovement;
        }
        #endregion

        #region 設定
        #region 測試設定
        [Header("TestSetting")]
        [Min(0)]
        public int test_Damage;
        public bool test_ShowDamage;
        public Vector2 test_HitFrom;
        #endregion
        #region 預製物設定
        [Space(10)]
        [Header("PrafebSetting")]
        public GameObject[] NumberPrafebs;
        public float NumberPitch;
        public EffectPrafeb[] HitEffectPrafebs, DeffendEffectPrafebs;
        [Tooltip("文字的間距")]
        #endregion
        #region 傷害數字效果設定
        [Space(10)]
        [Header("NumberEffectSetting")]
        public float NumberEffectTime;
        public float NumberEndHight, NumberEndHorizontal;
        public Color NumberEndColor;
        public Ease NumberColorEase = Ease.InSine,
            NumberHorizontalEase = Ease.InOutCubic,
            NumberVerticalEase = Ease.OutBack;
        public bool NumberHorizontalMovement = true;
        #endregion
        #region 擊中特效設定
        [Space(10)]
        [Header("HitEffectSetting")]
        public float HitEffectTime;
        public float HitEndHight, HitEndHorizontal;
        [Range(0, 1)]
        public float HitEndColorDelay;
        public Vector3 HitStartSize, HitEndSize;
        public Color HitEndColor;
        public Ease HitColorEase = Ease.Linear,
            HitSizeEase = Ease.OutBack,
            HitMovementEase = Ease.OutBack;
        #endregion
        #endregion

        #region Inspector功能
        [ContextMenu("PlayHitEffect")]
        private void PlayHitEffect()
        {
            PlayHitEffect(new DamageData()
            {
                Damage = test_Damage,
                HitAt = transform.position,
                HitFrom = test_HitFrom
            }, test_ShowDamage);
        }

        [ContextMenu("PlayDeffendEffect")]
        private void PlayDeffendEffect()
        {
            PlayDeffendEffect(new DamageData()
            {
                Damage = test_Damage,
                HitAt = transform.position,
                HitFrom = test_HitFrom
            }, test_ShowDamage);
        }
        #endregion

        public void PlayHitEffect(DamageData damage, bool showNumber = true)
        {
            if (showNumber)
                ShowHitNumber(damage);
            ShowSprite(HitEffectPrafebs[Random.Range(0, HitEffectPrafebs.Length * 10)
                % HitEffectPrafebs.Length], damage);
        }

        public void PlayDeffendEffect(DamageData damage, bool showNumber = true)
        {
            if (showNumber)
                ShowHitNumber(damage);
            ShowSprite(DeffendEffectPrafebs[Random.Range(0, DeffendEffectPrafebs.Length * 10)
                % DeffendEffectPrafebs.Length], damage);
        }

        private void ShowHitNumber(DamageData damage)
        {
            var temp = new GameObject($"HitNumber_{damage.Damage}");

            var renderers = AssambellyNumber(damage.Damage, temp);
            temp.transform.position = damage.HitAt;
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.DOColor(NumberEndColor, NumberEffectTime).SetEase(NumberColorEase);
            }

            temp.transform.DOBlendableMoveBy(
                new Vector3(0, NumberEndHight), NumberEffectTime)
                .SetEase(NumberVerticalEase)
                .onComplete += () => Destroy(temp);

            if (NumberHorizontalMovement)
            {
                temp.transform.DOBlendableMoveBy(
                    new Vector3((damage.HitAt.x - damage.HitFrom.x) > 0 ?
                    NumberEndHorizontal : -NumberEndHorizontal, 0)
                    , NumberEffectTime)
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

        private void ShowSprite(EffectPrafeb effectPrafeb, DamageData damage)
        {
            var temp = Instantiate(effectPrafeb.Prafeb, damage.HitAt, Quaternion.identity);
            temp.GetComponent<SpriteRenderer>().flipX =
                (damage.HitAt.x - damage.HitFrom.x) * (int)effectPrafeb.DefaultDirection > 0;

            temp.transform.DOScale(HitEndSize, HitEffectTime)
                .SetEase(HitSizeEase)
                .ChangeStartValue(HitStartSize)
                .onComplete += () => Destroy(temp);

            temp.GetComponent<SpriteRenderer>()
                .DOColor(HitEndColor, HitEffectTime * (1 - HitEndColorDelay))
                .SetDelay(HitEffectTime * HitEndColorDelay);

            if (!effectPrafeb.VerticalMovement && !effectPrafeb.HorizontalMovement)
                return;

            Vector3 endPosition = new Vector3(
                damage.HitAt.x + (!effectPrafeb.HorizontalMovement ? 0 :
                    (damage.HitAt.x - damage.HitFrom.x) < 0 ? HitEndHorizontal : -HitEndHorizontal),
                damage.HitAt.y + (effectPrafeb.VerticalMovement ? HitEndHight : 0));

            temp.transform.DOMove(endPosition, HitEffectTime);
        }
    }
}