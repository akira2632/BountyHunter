using UnityEngine;

namespace CharacterSystem_V2.InterfaceAndData
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
    /// 回乎函式定義
    /// </summary>
    public delegate void CallBackFuction();

    /// <summary>
    /// 角色基底介面
    /// </summary>
    public interface ICharacter : ICharacterControll, ICharacterData, ICharacterAttribute { }

    /// <summary>
    /// 角色操作介面
    /// </summary>
    public interface ICharacterControll
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
    /// 角色執行期參數介面
    /// </summary>
    public interface ICharacterData
    {
        Vertical Vertiacl { get; set; }
        Horizontal Horizontal { get; set; }
        float RegenTime { get; set; }
        float Recovery { get; set; }
        int Health { get; set; }
    }

    /// <summary>
    /// 角色屬性介面
    /// </summary>
    public interface ICharacterAttribute
    {
        float RegenSpeed { get; }
        int RegenHealth { get; }

        float MoveSpeed { get; }
        float DodgeSpeed { get; }
        int MaxHealth { get; }
    }
}