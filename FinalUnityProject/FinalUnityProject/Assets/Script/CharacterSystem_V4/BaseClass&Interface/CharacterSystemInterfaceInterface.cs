using UnityEngine;

namespace CharacterSystem_V4
{
    /// <summary>
    /// 角色傷害物件
    /// </summary>
    public struct Wound
    {
        public float Vertigo;
        public int Damage;

        public float KnockBackDistance, KnockBackSpeed;
        public Vector2 KnockBackFrom;
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

        void OnHit(Wound wound);
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
        float AttackSpeed { get; }
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
        public abstract float AttackSpeed { get; }
        public abstract float RegenSpeed { get; }
        public abstract int RegenHealth { get; }
        public abstract int CriticalDamage { get; }
        public abstract float CriticalRate { get; }
    }
}

namespace CharacterSystem_V4.Controller
{
    public abstract class AIStateManager : MonoBehaviour
    {
        public ICharacterActionManager character;
        public BasicAISenser senser;
        public AISetting setting;

        private bool isInitial = false, playerCloseBy;
        private AIState nowState;

        private void Start()
        {
            senser.OnPlayerCloseBy += (bool data) => playerCloseBy = data;
        }

        private void FixedUpdate()
        {
            if (playerCloseBy)
            {
                if (!isInitial)
                {
                    nowState.Initial();
                    isInitial = true;
                }

                nowState.Update();
            }
        }

        public void SetState(AIState nextState)
        {
            nowState.End();
            isInitial = false;
            nowState = nextState;
        }
    }

    public abstract class AIState
    {
        protected AIStateManager manager;

        public void SetManager(AIStateManager manager)
            => this.manager = manager;

        public virtual void Initial() { }
        public virtual void Update() { }
        public virtual void End() { }
    }

}