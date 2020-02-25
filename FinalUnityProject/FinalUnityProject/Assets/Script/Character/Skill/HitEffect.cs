using DG.Tweening;
using UnityEngine;

namespace Character.Skill
{
    [CreateAssetMenu(fileName = "擊中特效設定", menuName = "賞金獵人/技能資料/擊中特效", order = 0)]
    public class HitEffect : ScriptableObject
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
        #region 預製物設定
        [Header("PrafebSetting"), SerializeField]
        private GameObject[] NumberPrafebs;
        [Tooltip("文字的間距"), SerializeField]
        private float NumberPitch;
        [SerializeField]
        private EffectPrafeb[] HitEffectPrafebs;
        #endregion
        #region 傷害數字效果設定
        [Space(10)]
        [Header("NumberEffectSetting")]
        [Min(0), SerializeField]
        private float NumberEffectTime;
        [Min(0), SerializeField]
        private float NumberDestroyDelayTime;
        [SerializeField]
        private float NumberEndHight, NumberEndHorizontal
            , NumberDestroyDelayHight, NumberDestroyDelayHorizontal;
        [SerializeField]
        private Color NumberStartColor, NumberEndColor;
        [SerializeField]
        private Vector3 NumberStartSize, NumberEndSize;
        [SerializeField]
        private Ease NumberColorEase = Ease.InSine,
            NumberSizeEase = Ease.OutBack,
            NumberHorizontalEase = Ease.InOutCubic,
            NumberVerticalEase = Ease.OutBack,
            NumberDestoryDelayMovementEase = Ease.InSine;
        [SerializeField]
        private bool NumberHorizontalMovement = true;
        #endregion
        #region 技能特效設定
        [Space(10)]
        [Header("HitEffectSetting")]
        [Min(0), SerializeField]
        private float HitEffectTime;
        [SerializeField]
        private float HitEndHight, HitEndHorizontal;
        [Range(0, 1), SerializeField]
        private float HitEndColorDelay;
        [SerializeField]
        private Vector3 HitStartSize, HitEndSize;
        [SerializeField]
        private Color HitEndColor;
        [SerializeField]
        private Ease HitColorEase = Ease.Linear,
            HitSizeEase = Ease.OutBack,
            HitMovementEase = Ease.OutBack;
        #endregion
        #endregion

        public void PlayEffect(DamageData damage, bool showNumber = true)
        {
            if (showNumber)
                ShowHitNumber(damage);

            if (HitEffectPrafebs.Length <= 0)
                return;

            ShowSprite(HitEffectPrafebs[Random.Range(0, HitEffectPrafebs.Length * 10)
                % HitEffectPrafebs.Length], damage);
        }

        private void ShowHitNumber(DamageData damage)
        {
            var temp = new GameObject($"{name}_Number_{damage.Damage}");

            var renderers = AssambellyNumber(damage.Damage, temp);
            temp.transform.position = damage.HitAt;
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.DOColor(NumberEndColor, NumberEffectTime + NumberDestroyDelayTime)
                    .ChangeStartValue(NumberStartColor)
                    .SetEase(NumberColorEase);
            }

            temp.transform.DOScale(NumberEndSize, NumberEffectTime)
                .ChangeStartValue(NumberStartSize)
                .SetEase(NumberSizeEase);

            temp.transform.DOBlendableMoveBy(
                new Vector3(0, NumberEndHight), NumberEffectTime)
                .SetEase(NumberVerticalEase)
                .onComplete += () =>
                {
                    temp.transform.DOBlendableMoveBy(
                        new Vector3(0, NumberDestroyDelayHight), NumberDestroyDelayTime)
                    .SetEase(NumberDestoryDelayMovementEase)
                    .onComplete += () => Destroy(temp);
                };

            if (NumberHorizontalMovement)
            {
                temp.transform.DOBlendableMoveBy(
                    new Vector3((damage.HitAt.x - damage.HitFrom.x) > 0 ?
                    NumberEndHorizontal : -NumberEndHorizontal, 0)
                    , NumberEffectTime)
                    .SetEase(NumberHorizontalEase)
                    .onComplete += () =>
                    {
                        temp.transform.DOBlendableMoveBy(
                            new Vector3((damage.HitAt.x - damage.HitFrom.x) > 0 ?
                            NumberDestroyDelayHorizontal : -NumberDestroyDelayHorizontal, 0)
                            , NumberDestroyDelayTime)
                            .SetEase(NumberDestoryDelayMovementEase);
                    };
            }

            #region 輔助函式
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
            #endregion
        }

        private void ShowSprite(EffectPrafeb effectPrafeb, DamageData damage)
        {
            var temp = Instantiate(effectPrafeb.Prafeb, damage.HitAt, Quaternion.identity);
            temp.name = $"{name}_Effect_{damage.Damage}";
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