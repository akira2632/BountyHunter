using UnityEngine;

namespace CharacterSystem
{
    /// <summary>
    /// 角色傷害物件
    /// </summary>
    public struct DamageData
    {
        public float Vertigo;
        public int Damage;

        public float KnockBackDistance, KnockBackSpeed;
        public Vector2 HitFrom, HitAt;
    }

    /// <summary>
    /// 角色操作介面
    /// </summary>
    public interface ICharacterActionControll
    {
        void Move(Vector2 direction);

        void Dodge();
        void BasicAttack();
        void SpecialAttack();
        void SpecialAttack(bool hold);
        void SpecialAttack(Vector3 tartgetPosition);
        void Deffend(bool deffend);

        void OnHit(DamageData damage);
    }

    /// <summary>
    /// 角色能力介面
    /// </summary>
    public interface ICharacterProperty
    {
        float MoveSpeed { get; }
        float DodgeSpeed { get; }
        int MaxHealth { get; }

        float RegenSpeed { get; }
        int RegenHealth { get; }

        float BasicAttackSpeed { get; }
        float SpacilAttackSpeed { get; }

        int Attack { get; }
        int AttackFloating { get; }
        float CriticalMagnifiction { get; }
        float CriticalRate { get; }
    }

    /// <summary>
    /// 角色執行期參數介面
    /// </summary>
    public interface ICharacterData
    {
        void SetData(ICharacterProperty characterProperty, Transform characterTransform);
        void Update();

        Vector2 Direction { get; set; }
        Vector3 TargetPosition { get; set; }

        float BasicAttackTimer { get; set; }
        float SpacilAttackTimer { get; set; }

        float RegenTimer { get; set; }
        int Health { get; set; }
        float VertigoConter { get; set; }
    }

    public abstract class IScriptableCharacterProperty : ScriptableObject, ICharacterProperty
    {
        public abstract float MoveSpeed { get; }
        public abstract float DodgeSpeed { get; }
        public abstract int MaxHealth { get; }

        public abstract float RegenSpeed { get; }
        public abstract int RegenHealth { get; }

        public abstract float BasicAttackSpeed { get; }
        public abstract float SpacilAttackSpeed { get; }

        public abstract int Attack { get; }
        public abstract int AttackFloating { get; }
        public abstract float CriticalMagnifiction { get; }
        public abstract float CriticalRate { get; }
    }
}