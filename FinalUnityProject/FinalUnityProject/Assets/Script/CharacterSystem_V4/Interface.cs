using UnityEngine;

namespace CharacterSystem_V4
{
    public struct Damage
    {
        public float vertigo;
        public int damage;
    }

    /// <summary>
    /// 垂直方向定義
    /// </summary>
    public enum Vertical { Top = 1, None = 0, Down = -1 }

    /// <summary>
    /// 水平方向定義
    /// </summary>
    public enum Horizontal { Left = -1, None = 0, Right = 1 }

    /// <summary>
    /// 角色操作介面
    /// </summary>
    public interface ICharacterActionControll
    {
        void Move(Vertical direction);
        void Move(Horizontal direction);

        void Dodge();
        void LightAttack();
        void HeavyAttack();
        void HeavyAttack(bool hold);
        void Deffend(bool deffend);

        void OnHit(Damage damage);
    }

    /// <summary>
    /// 角色能力介面
    /// </summary>
    public interface ICharacterProperty
    {
        float MoveSpeed { get; }
        float DodgeSpeed { get; }
        int MaxHealth { get; }

        int Attack { get; }
        float AttackSpeed { get; }

        float RegenSpeed { get; }
        int RegenHealth { get; }
    }
}