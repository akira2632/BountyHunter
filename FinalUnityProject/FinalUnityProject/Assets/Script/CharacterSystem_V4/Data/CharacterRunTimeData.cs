using UnityEngine;

namespace CharacterSystem_V4
{
    /// <summary>
    /// 角色執行期參數介面
    /// </summary>
    public class CharacterRunTimeData
    {
        ICharacterProperty property;
        int _health;
        float _attackTimer, _regenTimer, _vertigoConter;

        public void SetData(ICharacterProperty characterProperty)
        {
            property = characterProperty;

            _health = property.MaxHealth;
            Vertical = Vertical.Down;
            Horizontal = Horizontal.None;
            _regenTimer = 0;
            _attackTimer = 0;
            _vertigoConter = 0;
        }

        public Vertical Vertical { get; set; }
        public Horizontal Horizontal { get; set; }
        public float AttackTimer
        {
            get => _attackTimer;
            set
            {
                if (value <= property.AttackSpeed)
                    _attackTimer = value;
            }
        }
        public float RegenTimer
        {
            get => _regenTimer;
            set
            {
                if (value <= property.RegenSpeed)
                    _regenTimer = value;
            }
        }
        public float VertigoConter
        {
            get => _vertigoConter;
            set
            {
                if (value > 0)
                    _vertigoConter = value;
            }
        }
        public int Health
        {
            get => _health;
            set
            {
                if (value >= 0 && value <= property.MaxHealth)
                    _health = value;
            }
        }
    }

}
