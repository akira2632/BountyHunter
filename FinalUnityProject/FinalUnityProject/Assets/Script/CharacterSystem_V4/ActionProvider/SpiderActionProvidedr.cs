using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem.ActionProvider
{
    public class SpiderActionProvidedr : ICharacterActionProvider
    {
        public AudioClip MoveSound, FallDownSound, BasicAttackSound, HurtSound;
        public HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController comtroller)
        {
            return new SpiderIdle()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetDeadAction(CharacterActionController comtroller)
        {
            return new SpiderDead()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController comtroller)
        {
            return new SpiderFall()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetMoveAction(CharacterActionController comtroller)
        {
            return  new SpiderMove()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetBasicAttackAction(CharacterActionController comtroller)
        {
            return new SpiderBasicAttack()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetKnockBackAction(CharacterActionController comtroller, DamageData damage)
        {
            return new SpiderKnockBack(damage)
            {
                actionController = comtroller,
                actionProvider = this
            };
        }
        #endregion

        #region SpiderActions
        private class ISpiderAction : ICharacterAction
        {
            public SpiderActionProvidedr actionProvider;
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
                if (damage.KnockBackDistance > 0)
                    actionController.SetAction(actionProvider.GetKnockBackAction(actionController, damage));
            }
        }

        private class SpiderIdle : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionController.CharacterAnimator.SetBool("IsFallDown", false);
                actionController.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionController.CharacterData.Direction = direction;
                    actionController.SetAction(actionProvider.GetMoveAction(actionController));
                }
            }
            #endregion
        }

        private class SpiderMove : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.AudioSource.clip = actionProvider.MoveSound;
                actionController.AudioSource.loop = true;
                actionController.AudioSource.Play();

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);
                actionController.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionController.MovementBody.MovePosition(actionController.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                    * actionController.CharacterData.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                actionController.AudioSource.Stop();
                actionController.AudioSource.loop = false;
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
               actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                else
                    actionController.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class SpiderBasicAttack : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionController.CharacterData.BasicAttackTimer > 0)
                {
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                    return;
                }

                actionController.AudioSource.clip = actionProvider.BasicAttackSound;
                actionController.AudioSource.Play();

                actionController.CharacterAnimator.SetTrigger("LightAttack");
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionController.CharacterData.BasicAttackTimer = actionController.CharacterData.BasicAttackSpeed;
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
            #endregion
        }

        private class SpiderKnockBack : ISpiderAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private DamageData damage;

            public SpiderKnockBack(DamageData damage)
            {
                this.damage = damage;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = IsometricUtility.ToIsometricVector2(
                    actionController.MovementBody.position - damage.HitFrom).normalized;

                actionController.AudioSource.clip = actionProvider.HurtSound;
                actionController.AudioSource.Play();

                actionController.CharacterAnimator.SetBool("IsHurt", true);
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    actionController.MovementBody.MovePosition(actionController.MovementBody.position + temp);
                }
                else
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void End()
            {
                actionController.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class SpiderFall : ISpiderAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                actionController.CharacterData.VertigoConter = 0;

                actionController.AudioSource.clip = actionProvider.HurtSound;
                actionController.AudioSource.Play();

                actionController.CharacterAnimator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void End()
            {
                actionController.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class SpiderDead : ISpiderAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                actionController.MovementCollider.enabled = false;

                actionController.AudioSource.clip = actionProvider.FallDownSound;
                actionController.AudioSource.Play();

                actionController.CharacterAnimator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    actionController.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(actionController.gameObject);
            }

            public override void End()
            {
                actionController.MovementCollider.enabled = true;
                actionController.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}