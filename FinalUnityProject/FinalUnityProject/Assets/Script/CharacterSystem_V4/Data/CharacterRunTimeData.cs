using UnityEngine;

namespace CharacterSystem_V4
{
    /// <summary>
    /// 角色執行期參數介面
    /// </summary>
    [System.Serializable]
    public class CharacterRunTimeData
    {
        ICharacterProperty property;
        [SerializeField]
        int _health;
        [SerializeField]
        float _attackTimer, _regenTimer, _vertigoConter;
        [SerializeField]
        Vertical _vertical;
        [SerializeField]
        Horizontal _horizontal;

        public void SetData(ICharacterProperty characterProperty)
        {
            property = characterProperty;

            _health = property.MaxHealth;
            _vertical = Vertical.Down;
            _horizontal = Horizontal.None;
            _regenTimer = 0;
            _attackTimer = 0;
            _vertigoConter = 0;
        }

        public Vertical Vertical { get => _vertical; set => _vertical = value; }
        public Horizontal Horizontal { get => _horizontal; set => _horizontal = value; }
        public float AttackTimer
        {
            get => _attackTimer;
            set
            {
                if (value > property.AttackSpeed)
                    _attackTimer = property.AttackSpeed;
                else
                    _attackTimer = value;
            }
        }
        public float RegenTimer
        {
            get => _regenTimer;
            set
            {
                if (value > property.RegenSpeed)
                    _regenTimer = property.RegenSpeed;
                else
                    _regenTimer = value;
            }
        }
        public float VertigoConter
        {
            get => _vertigoConter;
            set
            {
                if (value >= 0)
                    _vertigoConter = value;
            }
        }
        public int Health
        {
            get => _health;
            set
            {
                if (value < 0)
                    _health = 0;
                else if (value > property.MaxHealth)
                    _health = property.MaxHealth;
                else
                    _health = value;
            }
        }
    }

}
