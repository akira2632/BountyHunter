using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4
{
    public abstract class ICharacterActionManager : MonoBehaviour, ICharacterActionControll
    {
        public CharacterRunTimeData RunTimeData;
        public IScriptableCharacterProperty Property;

        #region AnimationControll
        public void AnimationEnd()
        {
            animationEnd = true;
        }
        public void AnimationStart()
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

        #region ICharacterActionControll委派
        public void Move(Vertical direction) => nowAction.Move(direction);
        public void Move(Horizontal direction) => nowAction.Move(direction);

        public void Dodge() => nowAction.Dodge();
        public void Deffend(bool deffend) => nowAction.Deffend(deffend);
        public void LightAttack() => nowAction.LightAttack();
        public void HeavyAttack() => nowAction.HeavyAttack();
        public void HeavyAttack(bool hold) => nowAction.HeavyAttack(hold);

        public void OnHit(Wound wound) => nowAction.OnHit(wound);
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
        public virtual void Move(Vertical direction) { }
        public virtual void Move(Horizontal direction) { }

        public virtual void Deffend(bool deffend) { }
        public virtual void Dodge() { }
        public virtual void HeavyAttack() { }
        public virtual void HeavyAttack(bool hold) { }
        public virtual void LightAttack() { }

        public virtual void OnHit(Wound damage) { }
        #endregion
    }
}
