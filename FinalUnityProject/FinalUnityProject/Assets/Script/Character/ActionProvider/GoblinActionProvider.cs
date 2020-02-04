using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    [CreateAssetMenu(fileName = "哥布林動作提供者", menuName = "賞金獵人_角色系統/動作提供者/哥布林動作提供者")]
    public class GoblinActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, FallDownSound, BasicAttackSound, HurtSound;
        public Skill.HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController comtroller)
        {
            return new GoblinIdle()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetDeadAction(CharacterActionController comtroller)
        {
            return new GoblinDead()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController comtroller)
        {
            return new GoblinFall()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetKnockBackAction(CharacterActionController comtroller, DamageData damage)
        {
            return new GoblinKnockBack(damage)
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetMoveAction(CharacterActionController comtroller)
        {
            return new GoblinMove()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetBasicAttackAction(CharacterActionController comtroller)
        {
            return new GoblinBasicAttack()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecailAttackAction(CharacterActionController comtroller)
        {
            return new GoblinSpacilAttack()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecailAttackAction(CharacterActionController comtroller, Vector3 targetPosition)
        {
            return new GoblinSpacilAttack(targetPosition)
            {
                actionController = comtroller,
                actionProvider = this
            };
        }
        #endregion

        #region GoblinActions
        private class IGoblinAction : ICharacterAction
        {
            public GoblinActionProvider actionProvider;
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
                if (actionController.CharacterData.Health > 0
                    && damage.KnockBackDistance > 0)
                    actionController.SetAction(actionProvider.GetKnockBackAction(actionController, damage));
            }
        }

        private class GoblinIdle : IGoblinAction
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

            public override void SpecialAttack() =>
                actionController.SetAction(actionProvider.GetSpecailAttackAction(actionController));

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                actionController.CharacterData.TargetPosition = tartgetPosition;
                actionController.SetAction(actionProvider.GetSpecailAttackAction(actionController, tartgetPosition));
            }

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

        private class GoblinMove : IGoblinAction
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

            public override void SpecialAttack() =>
                actionController.SetAction(actionProvider.GetSpecailAttackAction(actionController));

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                actionController.CharacterData.TargetPosition = tartgetPosition;
                actionController.SetAction(actionProvider.GetSpecailAttackAction(actionController, tartgetPosition));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                else
                    actionController.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class GoblinBasicAttack : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionController.CharacterData.BasicAttackTimer > 0)
                {
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                    return;
                }

                actionController.CharacterAnimator.SetTrigger("LightAttack");
                actionController.AudioSource.clip = actionProvider.BasicAttackSound;
                actionController.AudioSource.Play();
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

        private class GoblinSpacilAttack : IGoblinAction
        {
            private Vector3 targetPosition;
            private bool hasTarget;

            public GoblinSpacilAttack()
            {
                hasTarget = false;
            }

            public GoblinSpacilAttack(Vector3 targetPosition)
            {
                this.targetPosition = targetPosition;
                hasTarget = true;
            }

            #region 動作更新
            public override void Start()
            {
                if (actionController.CharacterData.SpacilAttackTimer > 0)
                {
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                    return;
                }

                if (hasTarget)
                {
                    IsometricUtility.GetVerticalAndHorizontal(
                        targetPosition - actionController.transform.position, out var vertical, out var horizontal);
                    actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                    actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);
                }
                actionController.CharacterAnimator.SetTrigger("RangeAttack");
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionController.CharacterData.SpacilAttackTimer = actionController.CharacterData.SpacilAttackSpeed;
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
            #endregion
        }

        private class GoblinKnockBack : IGoblinAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private DamageData damage;

            public GoblinKnockBack(DamageData damage)
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

                    actionController.MovementBody.MovePosition(actionController.MovementBody.position
                        + temp);
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

        private class GoblinFall : IGoblinAction
        {
            float fallDownTimer;
            #region 動作更新
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
            #endregion
        }

        private class GoblinDead : IGoblinAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                actionController.AudioSource.clip = actionProvider.FallDownSound;
                actionController.AudioSource.Play();

                actionController.MovementCollider.enabled = false;
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