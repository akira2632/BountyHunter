using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class OrcActionProvider : ICharacterActionProvider
    {
        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public HitEffect DefalutHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionManager manager)
        {
            var temp = new OrcIdle();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public override ICharacterAction GetDeadAction(CharacterActionManager manager)
        {
            var temp = new OrcDead();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public override ICharacterAction GetFallDownAction(CharacterActionManager manager)
        {
            var temp = new OrcFall();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetMoveAction(CharacterActionManager manager)
        {
            var temp = new OrcMove();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetBasicAttackAction(CharacterActionManager manager)
        {
            var temp = new OrcBasicAttack();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetKnockBackAction(CharacterActionManager manager, DamageData damage)
        {
            var temp = new OrcKnockBack(damage);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }
        #endregion

        #region OrkActions
        private class IOrcAction : ICharacterAction
        {
            protected OrcActionProvider actionProvider;

            public void SetProvider(OrcActionProvider actionProvider)
            {
                this.actionProvider = actionProvider;
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.CharacterData.Health -= damage.Damage;
                actionManager.CharacterData.VertigoConter += damage.Vertigo;

                actionProvider.DefalutHitEffect.PlayEffect(damage);
                if (damage.KnockBackDistance > 0)
                    actionManager.SetAction(actionProvider.GetKnockBackAction(actionManager, damage));
            }
        }

        private class OrcIdle : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.CharacterData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
                actionManager.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(actionProvider.GetBasicAttackAction(actionManager));

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionManager.CharacterData.Direction = direction;
                    actionManager.SetAction(actionProvider.GetMoveAction(actionManager));
                }
            }
            #endregion
        }

        private class OrcMove : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                actionProvider.MoveSound.Play();
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.CharacterData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
                actionManager.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.CharacterData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionManager.MovementBody.MovePosition(actionManager.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionManager.CharacterData.Direction)
                    * actionManager.CharacterData.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                actionProvider.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
               actionManager.SetAction(actionProvider.GetBasicAttackAction(actionManager));

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
                else
                    actionManager.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class OrcBasicAttack : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionManager.CharacterData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
                    return;
                }

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
                actionProvider.LightAttackSound.Play();
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.CharacterData.BasicAttackTimer = actionManager.CharacterData.BasicAttackSpeed;
                actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
            }
            #endregion
        }

        private class OrcFall : IOrcAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                actionProvider.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
            }

            public override void End()
            {
                actionManager.CharacterData.VertigoConter = 0;
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class OrcKnockBack : IOrcAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private DamageData damage;

            public OrcKnockBack(DamageData damage)
            {
                this.damage = damage;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = IsometricUtility.ToIsometricVector2(
                    actionManager.MovementBody.position - damage.HitFrom).normalized;
                actionManager.CharacterAnimator.SetBool("IsHurt", true);
                actionProvider.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    actionManager.MovementBody.MovePosition(actionManager.MovementBody.position + temp);
                }
                else
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
            }

            public override void End()
            {
                actionManager.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class OrcDead : IOrcAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                actionManager.MovementCollider.enabled = false;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                actionProvider.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    actionManager.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(actionManager.gameObject);
            }

            public override void End()
            {
                actionManager.MovementCollider.enabled = true;
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}