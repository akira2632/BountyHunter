using UnityEngine;

namespace CharacterSystem_V2.InterfaceAndData
{
    public class CharacterData : ICharacterData
    {
        readonly ICharacter myCharacter;
        Vertical _vertical;
        Horizontal _horizontal;
        float _regenTime, _recovery;
        int _health;

        public CharacterData(ICharacter character)
        {
            myCharacter = character;
            _vertical = Vertical.Down;
            _horizontal = Horizontal.Right;
            _regenTime = 0;
            _recovery = 0;
            _health = character.MaxHealth;
        }

        public Vertical Vertiacl { get => _vertical; set => _vertical = value; }
        public Horizontal Horizontal { get => _horizontal; set => _horizontal = value; }
        public float RegenTime { get => _regenTime; set => _regenTime = value; }
        public float Recovery { get => _recovery; set => _recovery = value >= 0 ? value : 0; }

        public int Health
        {
            get => _health;
            set
            {
                if (value >= 0 && value <= myCharacter.MaxHealth)
                    _health = value;
            }
        }
    }

    public class TempAttribute : ICharacterAttribute
    {
        public float RegenSpeed => 20;

        public int RegenHealth => 10;

        public float MoveSpeed => 2;

        public float DodgeSpeed => 4;

        public int MaxHealth => 200;
    }

    [CreateAssetMenu(fileName = "角色屬性", menuName = "賞金獵人_角色系統V2/角色屬性", order = 0)]
    public class CharacterAttribute : ScriptableObject, ICharacterAttribute
    {
        [Header("自然恢復速度"), Tooltip("角色自然恢復速度(秒)"), Min(0)]
        public float CharacterRegenSpeed;
        [Header("自然恢復量"), Min(0)]
        public int CharacterRegenHealth;
        [Header("最大生命值"), Min(0)]
        public int CharacterMaxHealth;
        [Header("移動速度"), Tooltip("角色移動速度、每秒幾格(格/秒)"), Min(0)]
        public float CharacterMoveSpeed;
        [Header("迴避速度"), Tooltip("角色迴避動作速度、影響包括重攻擊的衝刺速度、每秒幾格(格/秒)"), Min(0)]
        public float CharacterDodgeSpeed;

        public float RegenSpeed { get => CharacterRegenSpeed; }
        public int RegenHealth { get => CharacterRegenHealth; }
        public int MaxHealth { get => CharacterMaxHealth; }
        public float MoveSpeed { get => CharacterMoveSpeed; }
        public float DodgeSpeed { get => CharacterDodgeSpeed; }
    }
}
