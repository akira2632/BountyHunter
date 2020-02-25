using UnityEngine;

namespace Character.Skill
{
    public class SkillEffectTester : MonoBehaviour
    {
        [Header("TestSetting")]
        [Tooltip("要測試的效果")]
        public HitEffect Effect;
        public GameObject HitFrom, HitAt;
        [Space(10)]
        [Min(0)]
        public int test_Damage;
        public bool test_ShowDamage;

        [ContextMenu("PlayEffect")]
        private void PlayEffect()
        {
            Effect.PlayEffect(new DamageData()
            {
                Damage = test_Damage,
                HitAt = HitAt.transform.position,
                HitFrom = HitFrom.transform.position
            }, test_ShowDamage);
        }
    }
}