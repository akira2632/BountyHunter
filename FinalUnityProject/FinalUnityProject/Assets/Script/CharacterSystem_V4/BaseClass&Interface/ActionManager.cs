using System;
using UnityEngine;

namespace CharacterSystem
{
    public abstract class ICharacterActionManager : MonoBehaviour,
        ICharacterActionControll, IAnimateStateInvokeTarget
    {
        public CharacterRunTimeData RunTimeData;
        public IScriptableCharacterProperty Property;

        public event Action OnCharacterDead;
        private bool hasInvoke;

        #region AnimationControll
        public void OnAnimationEnd()
        {
            animationEnd = true;
        }

        public void OnAnimationStart()
        {
            animationStart = true;
        }
        protected bool animationStart, animationEnd;
        #endregion

        #region 流程控制
        protected ICharacterAction nowAction;
        private bool IsStart;

        public void FixedUpdate()
        {
            ActionUpdate();
        }

        public virtual void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !hasInvoke)
            {
                OnCharacterDead?.Invoke();
                hasInvoke = true;
            }

            if(!IsStart)
            {
                nowAction.Start();
                IsStart = true;
            }

            if (RunTimeData.Health > 0)
            {
                if (RunTimeData.BasicAttackTimer >= 0)
                    RunTimeData.BasicAttackTimer -= Time.deltaTime;
                if (RunTimeData.SpacilAttackTimer >= 0)
                    RunTimeData.SpacilAttackTimer -= Time.deltaTime;

                RunTimeData.RegenTimer += Time.deltaTime;
                if (RunTimeData.Health < Property.MaxHealth &&
                    RunTimeData.RegenTimer >= Property.RegenSpeed)
                {
                    RunTimeData.Health += Property.RegenHealth;
                    RunTimeData.RegenTimer = 0;
                }

                RunTimeData.VertigoConter -= Time.deltaTime / 10;
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

    public abstract class ICharacterAction : ICharacterActionControll
    {
        protected ICharacterActionManager actionManager;

        public virtual void SetManager(ICharacterActionManager actionManager)
        {
            this.actionManager = actionManager;
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void End() { }

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
