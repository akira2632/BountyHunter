using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    public class OrcCaptainActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, FallDownSound, BasicAttackSound, SpecailAttackSound;
        public Skill.HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetDeadAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }

        public override ICharacterAction GetIdelAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region OrcCaptainAction
        private class IOrcCaptainAction : ICharacterAction
        {
            protected OrcCaptainActionProvider actionProvider;
            public CharacterActionController actionController;

            public void SetProvider(OrcCaptainActionProvider actionProvider)
            {
                this.actionProvider = actionProvider;
            }

            public void Hit(DamageData damage)
            {
                actionController.CharacterData.Health -= damage.Damage;
                actionController.CharacterData.VertigoConter += damage.Vertigo;
                actionProvider.DefaultHitEffect.PlayEffect(damage);
            }

            public void Start()
            {
                throw new System.NotImplementedException();
            }

            public void Update()
            {
                throw new System.NotImplementedException();
            }

            public void End()
            {
                throw new System.NotImplementedException();
            }

            public void Move(Vector2 direction)
            {
                throw new System.NotImplementedException();
            }

            public void Dodge()
            {
                throw new System.NotImplementedException();
            }

            public void BasicAttack()
            {
                throw new System.NotImplementedException();
            }

            public void SpecialAttack()
            {
                throw new System.NotImplementedException();
            }

            public void SpecialAttack(bool hold)
            {
                throw new System.NotImplementedException();
            }

            public void SpecialAttack(Vector3 tartgetPosition)
            {
                throw new System.NotImplementedException();
            }

            public void Deffend(bool deffend)
            {
                throw new System.NotImplementedException();
            }

            public void OnAnimationStart()
            {
                throw new System.NotImplementedException();
            }

            public void OnAnimationEnd()
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
    }
}
