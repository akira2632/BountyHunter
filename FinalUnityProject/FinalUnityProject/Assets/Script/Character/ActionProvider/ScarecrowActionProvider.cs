using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    [CreateAssetMenu(fileName = "稻草人動作提供者", menuName = "賞金獵人/動作提供者/稻草人動作提供者")]
    public class ScarecrowActionProvider : ICharacterActionProvider
    {
        public AudioClip AttackSound, HurtSound;
        public Skill.HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }

        public override ICharacterAction GetDeadAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region ScarecrowAction
        private class IScarecrowAction
        {
            public ScarecrowActionProvider actionProvider;
            public CharacterActionController actionController;

            public virtual void Start() { }
            public virtual void Update() { }
            public virtual void End() { }

            public virtual void OnAnimationStart() { }
            public virtual void OnAnimationEnd() { }

            public virtual void Move(Vector2 direction) { }
            public virtual void Dodge() { }
            public virtual void Deffend(bool deffend) { }

            public virtual void BasicAttack() { }
            public virtual void SpecialAttack() { }
            public virtual void SpecialAttack(bool hold) { }
            public virtual void SpecialAttack(Vector3 tartgetPosition) { }

            public virtual void Hit(DamageData damage) =>
                actionProvider.DefaultHitEffect.PlayEffect(damage);
        }

        private class ScarecrowIdel : IScarecrowAction
        {
            public override void BasicAttack()
            {
                base.BasicAttack();
            }

            public override void Hit(DamageData damage)
            {
                base.Hit(damage);
            }
        }
        #endregion
    }
}
