using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    [CreateAssetMenu(fileName = "稻草人動作提供者", menuName = "賞金獵人/動作提供者/稻草人動作提供者")]
    public class ScarecrowActionProvider : ICharacterActionProvider
    {
        public AudioClip AttackSound, HurtSound;
        public Skill.HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController controller)
        {
            return new ScarecrowIdel()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController controller)
        {
            return new ScarecrowIdel()
            {
                actionController = controller,
                actionProvider = this
            };
        }
        
        public override ICharacterAction GetDeadAction(CharacterActionController controller)
        {
            return new ScarecrowIdel()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetScarecrowHurtAction(CharacterActionController controller, DamageData damage)
        {
            return new ScarecrowHurt(damage)
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetScarecrowBascicAttackAction(CharacterActionController controller)
        {
            return new ScarecrowBasicAttack()
            {
                actionController = controller,
                actionProvider = this
            };
        }
        #endregion

        #region ScarecrowAction
        private class IScarecrowAction : ICharacterAction
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

            public virtual void Hit(DamageData damage) { }
        }

        private class ScarecrowIdel : IScarecrowAction
        {
            public override void BasicAttack() =>
                actionController.SetAction(actionProvider.GetScarecrowBascicAttackAction(actionController));

            public override void Hit(DamageData damage)=>
                actionController.SetAction(actionProvider.GetScarecrowHurtAction(actionController, damage));
        }

        private class ScarecrowBasicAttack : IScarecrowAction
        {
            public override void Start()
            {
                actionController.AudioSource.PlayOneShot(actionProvider.AttackSound);

                actionController.CharacterAnimator.SetTrigger("Attack");
            }

            public override void OnAnimationEnd()
            {
                actionController.CharacterData.BasicAttackTimer = actionController.CharacterData.BasicAttackSpeed;
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
        }

        private class ScarecrowHurt : IScarecrowAction
        {
            private bool flipX;
            private DamageData damage;

            public ScarecrowHurt(DamageData damage)
            {
                this.damage = damage;
                flipX = (damage.HitAt.x - damage.HitFrom.x) < 0;
            }

            public override void Start()
            {
                actionController.SpriteRenderer.flipX = flipX;
                actionController.CharacterAnimator.SetTrigger("Hurt");

                actionController.AudioSource.PlayOneShot(actionProvider.HurtSound);

                actionProvider.DefaultHitEffect.PlayEffect(damage);

                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
        }
        #endregion
    }
}
