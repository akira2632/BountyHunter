using UnityEngine;

namespace CharacterSystem_V4
{
    [CreateAssetMenu(fileName = "戰士屬性", menuName = "賞金獵人_角色系統V4/角色能力/戰士能力(屬性計算)")]
    public class WarriorProperty : IScriptableCharacterProperty
    {
        public int LvExpRequire;
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
        
        public override float MoveSpeed => throw new System.NotImplementedException();
        public override float DodgeSpeed => throw new System.NotImplementedException();
        public override int MaxHealth => throw new System.NotImplementedException();
        public override int Damage
        {
            get
            {
                if (Random.Range(0, 100) <= CriticalRate)
                    return (Damage) + CriticalDamage;
                else
                    return (Damage);
            }
        }
        public override float AttackSpeed => throw new System.NotImplementedException();
        public override float RegenSpeed => throw new System.NotImplementedException();
        public override int RegenHealth => throw new System.NotImplementedException();
        public override int CriticalDamage => throw new System.NotImplementedException();
        public override float CriticalRate => throw new System.NotImplementedException();
    }
}