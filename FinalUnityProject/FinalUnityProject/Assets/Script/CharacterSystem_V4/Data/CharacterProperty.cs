using UnityEngine;

namespace CharacterSystem_V4
{
    [CreateAssetMenu(fileName = "角色能力", menuName = "賞金獵人_角色系統V4/角色能力/一般能力")]
    public class CharacterProperty : IScriptableCharacterProperty
    {
        [Header("自然恢復速度"), Tooltip("角色自然恢復速度(秒)"), Min(0)]
        public float CharacterRegenSpeed;
        [Header("自然恢復量"), Min(0)]
        public int CharacterRegenHealth;
        [Header("最大生命值"), Min(0)]
        public int CharacterMaxHealth;
        [Header("移動速度"), Tooltip("角色移動速度、每秒幾格(格/秒)"), Min(0)]
        public float CharacterMoveSpeed;
        [Header("迴避速度"), Tooltip("角色迴避動作移動速度、影響包括重攻擊的衝刺速度、每秒幾格(格/秒)"), Min(0)]
        public float CharacterDodgeSpeed;
        [Header("攻擊力"), Tooltip("角色基本攻擊力"), Min(0)]
        public int CharacterAttack;
        [Header("攻擊速度"), Tooltip("角色攻擊速度、每秒幾次(次/秒)"), Min(0)]
        public float CharacterAttackSpeed;

        public override float RegenSpeed => CharacterRegenSpeed;
        public override int RegenHealth => CharacterRegenHealth;
        public override int MaxHealth => CharacterMaxHealth;
        public override float MoveSpeed => CharacterMoveSpeed;
        public override float DodgeSpeed => CharacterDodgeSpeed;
        public override int Damage
        {
            get
            {
                if (Random.Range(0, 100) <= CriticalRate)
                    return Damage + CriticalDamage;
                else
                    return Damage;
            }
        }
        public override float AttackSpeed => CharacterAttackSpeed;
        public override int CriticalDamage => throw new System.NotImplementedException();
        public override float CriticalRate => throw new System.NotImplementedException();
    }
}
