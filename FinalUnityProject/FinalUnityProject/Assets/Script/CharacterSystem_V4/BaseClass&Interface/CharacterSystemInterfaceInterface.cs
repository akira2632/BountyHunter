using UnityEngine;

namespace CharacterSystem_V4
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

        int Damage { get; }
        float BasicAttackSpeed { get; }
        float SpacilAttackSpeed { get; }
        int CriticalDamage { get; }
        float CriticalRate { get; }

        float RegenSpeed { get; }
        int RegenHealth { get; }
    }

    public abstract class IScriptableCharacterProperty : ScriptableObject, ICharacterProperty
    {
        public abstract float MoveSpeed { get; }
        public abstract float DodgeSpeed { get; }
        public abstract int MaxHealth { get; }
        public abstract int Damage { get; }
        public abstract float BasicAttackSpeed { get; }
        public abstract float SpacilAttackSpeed { get; }
        public abstract float RegenSpeed { get; }
        public abstract int RegenHealth { get; }
        public abstract int CriticalDamage { get; }
        public abstract float CriticalRate { get; }
    }
}