using DG.Tweening;
using UnityEngine;

namespace CharacterSystem_V4.Skill
{
    [CreateAssetMenu(fileName ="技能特效設定", menuName = "賞金獵人_角色系統V4/技能資料/技能效果", order = 0)]
    public class SkillEffector : ScriptableObject
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
        [Header("PrafebSetting")]
        public GameObject[] NumberPrafebs;
        [Tooltip("文字的間距")]
        public float NumberPitch;
        public EffectPrafeb[] SkillEffectPrafebs;
        #endregion
        #region 傷害數字效果設定
        [Space(10)]
        [Header("NumberEffectSetting")]
        public float NumberEffectTime;
        public float NumberEndHight, NumberEndHorizontal;
        public Color NumberStartColor, NumberEndColor;
        public Ease NumberColorEase = Ease.InSine,
            NumberHorizontalEase = Ease.InOutCubic,
            NumberVerticalEase = Ease.OutBack;
        public bool NumberHorizontalMovement = true;
        #endregion
        #region 技能特效設定
        [Space(10)]
        [Header("SkillEffectSetting")]
        public float SkillEffectTime;
        public float SkillEndHight, SkillEndHorizontal;
        [Range(0, 1)]
        public float SkillEndColorDelay;
        public Vector3 SkillStartSize, SkillEndSize;
        public Color SkillEndColor;
        public Ease SkillColorEase = Ease.Linear,
            SkillSizeEase = Ease.OutBack,
            SkillMovementEase = Ease.OutBack;
        #endregion
        #endregion

        public void PlayEffect(DamageData damage, bool showNumber = true)
        {
            if (showNumber)
                ShowHitNumber(damage);

            if (SkillEffectPrafebs.Length <= 0)
                return;

            ShowSprite(SkillEffectPrafebs[Random.Range(0, SkillEffectPrafebs.Length * 10)
                % SkillEffectPrafebs.Length], damage);
        }

        private void ShowHitNumber(DamageData damage)
        {
            var temp = new GameObject($"HitNumber_{damage.Damage}");

            var renderers = AssambellyNumber(damage.Damage, temp);
            temp.transform.position = damage.HitAt;
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.DOColor(NumberEndColor, NumberEffectTime)
                    .ChangeStartValue(NumberStartColor)
                    .SetEase(NumberColorEase);
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

            temp.transform.DOScale(SkillEndSize, SkillEffectTime)
                .SetEase(SkillSizeEase)
                .ChangeStartValue(SkillStartSize)
                .onComplete += () => Destroy(temp);

            temp.GetComponent<SpriteRenderer>()
                .DOColor(SkillEndColor, SkillEffectTime * (1 - SkillEndColorDelay))
                .SetDelay(SkillEffectTime * SkillEndColorDelay);

            if (!effectPrafeb.VerticalMovement && !effectPrafeb.HorizontalMovement)
                return;

            Vector3 endPosition = new Vector3(
                damage.HitAt.x + (!effectPrafeb.HorizontalMovement ? 0 :
                    (damage.HitAt.x - damage.HitFrom.x) < 0 ? SkillEndHorizontal : -SkillEndHorizontal),
                damage.HitAt.y + (effectPrafeb.VerticalMovement ? SkillEndHight : 0));

            temp.transform.DOMove(endPosition, SkillEffectTime);
        }
    }
}