using System;
using UnityEngine;

namespace CharacterSystem
{
    public abstract class ICharacterActionManager : MonoBehaviour,
        ICharacterActionControll, IAnimationStateHandler
    {
        public CharacterRunTimeData RunTimeData;
        public IScriptableCharacterProperty Property;
        public Rigidbody2D MovementBody;
        public Collider2D MovementCollider;
        public Animator CharacterAnimator;
        public SpriteRenderer SpriteRenderer;

        public event Action OnCharacterDead;
        private bool hasInvoke;

        #region MyRegion
        protected abstract ICharacterAction IdelAction { get; }
        #endregion

        #region 流程控制
        protected ICharacterAction nowAction;
        private bool IsStart;

        public void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property, transform);

            nowAction = IdelAction;
            nowAction.SetManager(this);
        }

        public void FixedUpdate()
        {
            if (RunTimeData.Health <= 0 && !hasInvoke)
            {
                OnCharacterDead?.Invoke();
                hasInvoke = true;
            }

            RunTimeData.Update();

            ActionUpdate();
        }

        public virtual void ActionUpdate()
        {
            if(!IsStart)
            {
                nowAction.Start();
                IsStart = true;
            }

            nowAction.Update();
        }

        public void SetAction(ICharacterAction nextAction)
        {
            nowAction.End();
            nextAction.SetManager(this);
            nowAction = nextAction;
            IsStart = false;
        }
        #endregion

        #region IAnimationStateHandler委派
        public void OnAnimationStart() => nowAction.OnAnimationStart();
        public void OnAnimationEnd() => nowAction.OnAnimationEnd();
        #endregion

        #region ICharacterActionControll委派
        public void Move(Vector2 direction) => nowAction.Move(direction);

        public void Dodge() => nowAction.Dodge();
        public void Deffend(bool deffend) => nowAction.Deffend(deffend);
        public void BasicAttack() => nowAction.BasicAttack();
        public void SpecialAttack() => nowAction.SpecialAttack();
        public void SpecialAttack(bool hold) => nowAction.SpecialAttack(hold);
        public void SpecialAttack(Vector3 tartgetPosition) => nowAction.SpecialAttack(tartgetPosition);

        public void OnHit(DamageData wound) => nowAction.OnHit(wound);
        #endregion
    }

    public abstract class ICharacterAction : ICharacterActionControll, IAnimationStateHandler
    {
        protected ICharacterActionManager actionManager;

        public virtual void SetManager(ICharacterActionManager actionManager)
        {
            this.actionManager = actionManager;
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void End() { }

        #region IAnimationStateHandler抽象實作
        public virtual void OnAnimationStart() { }
        public virtual void OnAnimationEnd() { }
        #endregion

        #region ICharacterActionControll抽象實作
        public virtual void Move(Vector2 direction) { }

        public virtual void Deffend(bool deffend) { }
        public virtual void Dodge() { }
        public virtual void SpecialAttack() { }
        public virtual void SpecialAttack(bool hold) { }
        public virtual void SpecialAttack(Vector3 tartgetPosition){ }
        public virtual void BasicAttack() { }

        public virtual void OnHit(DamageData damage) { }
        #endregion
    }
}
