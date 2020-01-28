using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem
{
    [CreateAssetMenu(fileName = "技能傷害", menuName = "賞金獵人_角色系統V4/技能資料/技能傷害", order = 1)]
    public class SkillDamage : ScriptableObject
    {
        [SerializeField, Header("額外傷害"), Tooltip("技能的額外傷害"), Min(0)]
        private int BonusDamage;
        [SerializeField, Header("額外會心率"), Tooltip("技能的額外會心率"), Range(-100, 100)]
        private float BonusCriticalRate;
        [SerializeField, Header("無會心"), Tooltip("使技能無法觸發會心")]
        private bool NoCritical;
        [SerializeField, Header("傷害倍率"), Tooltip("技能的傷害倍率"), Min(0)]
        private float DamageMagnifiction = 1;

        [Space(10)]
        [SerializeField, Header("暈眩值"), Tooltip("技能造成的暈眩值"), Min(0)]
        private float Vertigo;
        [SerializeField, Header("擊退距離"), Tooltip("使技將敵人擊退的距離"), Min(0)]
        private float KnockBackDistance;
        [SerializeField, Header("擊退速度"), Tooltip("使技將敵人擊退的速度、若有將敵人擊退時請不要設0"), Min(0)]
        private float KnockBackSpeed;

        public DamageData GetDamageData(IScriptableCharacterProperty property)
        {
            float damage = (BonusDamage + DamageMagnifiction
                * (property.Attack + Random.Range(-property.AttackFloating, property.AttackFloating)));

            if(!NoCritical
                && Random.Range(0, 100) < (BonusCriticalRate + property.CriticalRate))
                damage *= property.CriticalMagnifiction;

            return new DamageData()
            {
                Damage = (int)damage,
                Vertigo = Vertigo,
                KnockBackDistance = KnockBackDistance,
                KnockBackSpeed = KnockBackSpeed
            };
        }
    }
}