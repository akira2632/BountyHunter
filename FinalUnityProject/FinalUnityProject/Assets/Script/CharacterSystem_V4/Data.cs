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
            Vertiacl = Vertical.Down;
            Horizontal = Horizontal.None;
            _regenTimer = 0;
            _attackTimer = 0;
            VertigoConter = 0;
        }

        public Vertical Vertiacl { get; set; }
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
            set => _vertigoConter = value;
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

    [CreateAssetMenu(fileName = "按鍵設定", menuName = "賞金獵人_角色系統V4/按鍵設定", order = 0)]
    public class KeySetting : ScriptableObject
    {
        public KeyCode UpKey, DownKey, LeftKey, RightKey, LightAttack, HeavyAttack, Deffend;
    }

    [CreateAssetMenu(fileName = "角色能力", menuName = "賞金獵人_角色系統V4/角色能力", order = 1)]
    public class CharacterProperty : ScriptableObject, ICharacterProperty
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

        public float RegenSpeed => CharacterRegenSpeed;
        public int RegenHealth => CharacterRegenHealth;
        public int MaxHealth => CharacterMaxHealth;
        public float MoveSpeed => CharacterMoveSpeed;
        public float DodgeSpeed => CharacterDodgeSpeed;
        public int Attack => CharacterAttack;
        public float AttackSpeed => CharacterAttackSpeed;
    }
}
