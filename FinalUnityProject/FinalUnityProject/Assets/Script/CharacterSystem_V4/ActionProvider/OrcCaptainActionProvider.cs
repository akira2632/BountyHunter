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
            public OrcCaptainActionProvider actionProvider;
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

            public virtual void Hit(DamageData damage)
            {
                actionController.CharacterData.Health -= damage.Damage;
                actionController.CharacterData.VertigoConter += damage.Vertigo;
                actionProvider.DefaultHitEffect.PlayEffect(damage);
            }
        }
        #endregion
    }
}
