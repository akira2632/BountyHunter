using UnityEngine;

namespace CharacterSystem
{
    //[CreateAssetMenu(fileName = "戰士屬性", menuName = "賞金獵人_角色系統/角色能力/戰士能力(屬性計算)")]
    public class WarriorProperty : IScriptableCharacterProperty
    {
        public int LvExpRequire;
        [Header("當前等級")]
        public int Lv;

        [SerializeField]
        private int _exp;
        public int Exp
        {
            get => _exp;
            set
            {
                if (value < 0)
                    _exp = 0;
                else if (value > Lv * LvExpRequire)
                    _exp = Lv * LvExpRequire;
                else
                    _exp = value;
            }
        }

        [Header("力量")]
        public int Str;

        [Header("敏捷")]
        public int Agi;

        [Header("智慧")]
        public int Wis;

        [Header("靈敏")]
        public int Dex;

        [Header("生命力")]
        public int Vit;

        [Header("基礎移動速度")]
        public float MoveSpeedBase;

        [Header("基礎迴避速度")]
        public float DodgeSpeedBase;

        [Header("武器傷害")]
        public int WeaponDamage;


        // 移動速度
        public override float MoveSpeed => MoveSpeedBase + (Agi * 0.1f);
        // 迴避速度
        public override float DodgeSpeed => DodgeSpeedBase + (Agi * 0.1f);
        // 生命值
        public override int MaxHealth => 100 + Vit * 10;
        // 物理攻擊
        public override int Attack
        {
            get
            {
                if (Random.Range(0, 100) <= CriticalRate)
                    return (int)((Attack) * CriticalMagnifiction);
                else
                    return (Attack);
            }
        }
        // 攻擊速度
        public override float BasicAttackSpeed => 0;
        // 回復速度
        public override float RegenSpeed => 20;
        // 回復生命
        public override int RegenHealth => (Vit + MaxHealth) / 30;
        // 暴擊傷害
        public override float CriticalMagnifiction => ((Str * 2) * Lv) / 15;
        // 暴擊機率
        public override float CriticalRate => ((Lv + Dex + Agi) / 8);

        public override float SpacilAttackSpeed => throw new System.NotImplementedException();

        public override int AttackFloating => throw new System.NotImplementedException();
    }
}
