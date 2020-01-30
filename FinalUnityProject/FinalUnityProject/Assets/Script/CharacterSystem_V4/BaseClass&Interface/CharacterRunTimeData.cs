using UnityEngine;

namespace CharacterSystem
{
    /// <summary>
    /// 角色執行期參數介面
    /// </summary>
    [System.Serializable]
    public class CharacterRunTimeData
    {
        ICharacterProperty property;
        Transform transform;
        [SerializeField]
        int _health;
        [SerializeField]
        float _basicAttackTimer, _spacilAttackTimer , _regenTimer, _vertigoConter;
        [SerializeField]
        Vector2 _direction;
        [SerializeField]
        Vector3 _target;

        public void SetData(ICharacterProperty characterProperty, Transform characterTransform)
        {
            property = characterProperty;
            transform = characterTransform;

            _health = property.MaxHealth;
            _regenTimer = 0;
            _basicAttackTimer = 0;
            _spacilAttackTimer = 0;
            _vertigoConter = 0;
            _direction = new Vector2();
        }

        public void Update()
        {
            if (Health > 0)
            {
                if (BasicAttackTimer >= 0)
                    BasicAttackTimer -= Time.deltaTime;
                if (SpacilAttackTimer >= 0)
                    SpacilAttackTimer -= Time.deltaTime;

                RegenTimer += Time.deltaTime;
                if (Health < property.MaxHealth &&
                    RegenTimer >= property.RegenSpeed)
                {
                    Health += property.RegenHealth;
                    RegenTimer = 0;
                }

                VertigoConter -= Time.deltaTime / 10;
            }
        }

        public Vector2 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _target = IsometricUtility.ToIsometricVector3(_direction) * 10 + transform.position;
            }
        }
        public Vector3 TargetPosition
        {
            get => _target;
            set => _target = value;
        }
        public float BasicAttackTimer
        {
            get => _basicAttackTimer;
            set
            {
                if (value > property.BasicAttackSpeed)
                    _basicAttackTimer = property.BasicAttackSpeed;
                else if (value < 0)
                    _basicAttackTimer = 0;
                else
                    _basicAttackTimer = value;
            }
        }
        public float SpacilAttackTimer
        {
            get => _spacilAttackTimer;
            set
            {
                if (value > property.SpacilAttackSpeed)
                    _spacilAttackTimer = property.SpacilAttackSpeed;
                else if (value < 0)
                    _spacilAttackTimer = 0;
                else
                    _spacilAttackTimer = value;
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
                else
                    _vertigoConter = 0;
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