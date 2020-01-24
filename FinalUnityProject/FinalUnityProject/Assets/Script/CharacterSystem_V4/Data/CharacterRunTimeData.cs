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
        Vector2 _direction;

        public void SetData(ICharacterProperty characterProperty)
        {
            property = characterProperty;

            _health = property.MaxHealth;
            _regenTimer = 0;
            _attackTimer = 0;
            _vertigoConter = 0;
            _direction = new Vector2();
        }

        public Vector2 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
            }
        }
        public float AttackTimer
        {
            get => _attackTimer;
            set
            {
                if (value > property.AttackSpeed)
                    _attackTimer = property.AttackSpeed;
                else if (value < 0)
                    _attackTimer = 0;
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