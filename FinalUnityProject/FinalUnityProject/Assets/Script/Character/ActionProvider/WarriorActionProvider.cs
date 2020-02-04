using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    /// <summary>
    /// 戰士角色
    /// </summary>
    [CreateAssetMenu(fileName = "戰士動作提供者", menuName = "賞金獵人/動作提供者/戰士動作提供者")]
    public class WarriorActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, DeffendSound, FallDownSound, BasicAttackSound,
                SpecialAttack1Sound, HeavyAttackChargeSound, SpecialAttack2Sound;
        public Skill.HitEffect DefaultHitEffect, DefaultDeffendEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController comtroller)
        {
            return new WarriorIdel()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetDeadAction(CharacterActionController comtroller)
        {
            return new WarriorDead()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController comtroller)
        {
            return new WarriorFallDown()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetMoveAction(CharacterActionController comtroller)
        {
            return new WarriorMove()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetDeffendAction(CharacterActionController comtroller)
        {
            return new WarriorDeffend()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetBasicAttackAction(CharacterActionController comtroller)
        {
            return new WarriorBasicAttack()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttackStartAction(CharacterActionController comtroller)
        {
            return new WarriorSpecialAttackStart()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttackDodgeAction(CharacterActionController comtroller, bool isCharge)
        {
            return new WarriorSpecialAttackDodge(isCharge)
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttack1Action(CharacterActionController comtroller, bool isCharge)
        {
            return new WarriorSpecialAttack1(isCharge)
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttackChargeAction(CharacterActionController comtroller)
        {
            return new WarriorSpecialAttackCharge()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttack2Action(CharacterActionController comtroller)
        {
            return new WarriorSpecialAttack2()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }
        #endregion

        #region WarriorActions
        /// <summary>
        /// 戰士角色動作基底類別
        /// </summary>
        private class IWarriorAction : ICharacterAction
        {
            public WarriorActionProvider actionProvider;
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

        /// <summary>
        /// 戰士待機
        /// </summary>
        private class WarriorIdel : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.CharacterAnimator.SetBool("IsMove", false);

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);
            }
            #endregion

            #region 外部事件
            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionController.SetAction(actionProvider.GetDeffendAction(actionController));
            }

            public override void BasicAttack() =>
               actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));

            public override void SpecialAttack() =>
               actionController.SetAction(actionProvider.GetSpecialAttackStartAction(actionController));

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

        /// <summary>
        /// 戰士移動
        /// </summary>
        private class WarriorMove : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.AudioSource.clip = actionProvider.MoveSound;
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

                actionController.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部事件
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                else
                    actionController.CharacterData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionController.SetAction(actionProvider.GetDeffendAction(actionController));
            }

            public override void SpecialAttack() =>
                actionController.SetAction(actionProvider.GetSpecialAttackStartAction(actionController));

            public override void BasicAttack() =>
                actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));
            #endregion
        }

        /// <summary>
        /// 戰士防禦
        /// </summary>
        private class WarriorDeffend : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.CharacterAnimator.SetBool("IsDeffend", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);
            }

            public override void End()
            {
                actionController.CharacterAnimator.SetBool("IsDeffend", false);
            }
            #endregion

            #region 外部事件
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    actionController.CharacterData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (!deffend)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void BasicAttack()
            {
                actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));
            }

            public override void Hit(DamageData damage)
            {
                actionController.CharacterData.Health -= (int)(damage.Damage * 0.1f);

                actionController.AudioSource.PlayOneShot(actionProvider.DeffendSound);

                actionProvider.DefaultDeffendEffect.PlayEffect(damage);
            }
            #endregion
        }

        /// <summary>
        /// 戰士輕攻擊
        /// </summary>
        private class WarriorBasicAttack : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionController.CharacterData.BasicAttackTimer > 0)
                {
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                    return;
                }

                actionController.AudioSource.PlayOneShot(actionProvider.BasicAttackSound);

                actionController.CharacterAnimator.SetTrigger("BasicAttack");
            }
            #endregion

            #region 外部事件
            public override void OnAnimationEnd()
            {
                actionController.CharacterData.BasicAttackTimer = actionController.CharacterData.BasicAttackSpeed;
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊預備
        /// </summary>
        private class WarriorSpecialAttackStart : IWarriorAction
        {
            bool isCharge;

            #region 動作更新
            public override void Start()
            {
                isCharge = true;
                actionController.CharacterAnimator.SetBool("SpecialAttackStart", true);
            }
            #endregion

            #region 外部事件
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                    isCharge = false;
            }

            public override void OnAnimationEnd() =>
                actionController.SetAction(actionProvider.GetSpecialAttackDodgeAction(actionController, isCharge));

            public override void Hit(DamageData damage)
            {
                damage.Vertigo = 0;
                base.Hit(damage);
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊衝刺
        /// </summary>
        private class WarriorSpecialAttackDodge : IWarriorAction
        {
            float dodgeDistance, targetDistance = 2f;
            bool isCharge;

            public WarriorSpecialAttackDodge(bool isCharge)
            {
                this.isCharge = isCharge;
            }

            #region 動作更新
            public override void Start()
            {
                //GameManager.Instance.WarriorDodge(true);
                actionController.MovementCollider.isTrigger = true;
                dodgeDistance = 0;
            }

            public override void Update()
            {
                Vector2 dodgeVector =
                    IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                    * actionController.CharacterData.DodgeSpeed * Time.deltaTime;

                dodgeDistance += dodgeVector.magnitude;

                actionController.MovementBody.MovePosition(
                    actionController.MovementBody.position + dodgeVector);

                if (dodgeDistance >= targetDistance)
                    actionController.SetAction(actionProvider.GetSpecialAttack1Action(actionController, isCharge));
            }

            public override void End()
            {
                //GameManager.Instance.WarriorDodge(false);
                actionController.MovementCollider.isTrigger = false;
            }
            #endregion

            #region 外部事件
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                    isCharge = false;
            }

            public override void Hit(DamageData damage) { }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊第一段
        /// </summary>
        private class WarriorSpecialAttack1 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;
            bool isCharge;

            public WarriorSpecialAttack1(bool isCharge)
            {
                this.isCharge = isCharge;
            }

            #region 動作更新
            public override void Start()
            {
                dodgeDistance = 0;

                if (isCharge)
                    actionController.CharacterAnimator.SetBool("SpecialAttackCharge", true);

                actionController.AudioSource.PlayOneShot(actionProvider.SpecialAttack1Sound);

                actionController.CharacterAnimator.SetBool("SpecialAttackStart", false);
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector =
                        IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                        * actionController.CharacterData.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    actionController.MovementBody.MovePosition(
                        actionController.MovementBody.position + dodgeVector);
                }
            }
            #endregion

            #region 外部事件
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                {
                    isCharge = false;
                    actionController.CharacterAnimator.SetBool("SpacilAttackCharge", false);
                }
            }

            public override void OnAnimationEnd()
            {
                if (isCharge)
                    actionController.SetAction(actionProvider.GetSpecialAttackChargeAction(actionController));
                else
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void Hit(DamageData damage) { }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊蓄力
        /// </summary>
        private class WarriorSpecialAttackCharge : IWarriorAction
        {
            bool IsCharge, ChargeEnd;
            float ChargeTime;

            #region 動作更新
            public override void Start()
            {
                IsCharge = true;
                ChargeEnd = false;
                ChargeTime = 0;

                actionController.AudioSource.clip = actionProvider.HeavyAttackChargeSound;
                actionController.AudioSource.Play();
            }

            public override void Update()
            {
                if (!ChargeEnd)
                {
                    ChargeTime += Time.deltaTime;

                    IsometricUtility.GetVerticalAndHorizontal(
                        actionController.CharacterData.Direction, out var vertical, out var horizontal);
                    actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                    actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);

                    if (!IsCharge || ChargeTime > 2.1)
                        actionController.SetAction(actionProvider.GetSpecialAttack2Action(actionController));
                }
            }

            public override void End()
            {
                actionController.AudioSource.Stop();
            }
            #endregion

            #region 外部事件
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                    IsCharge = false;
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    actionController.CharacterData.Direction = direction;
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊第二段
        /// </summary>
        private class WarriorSpecialAttack2 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;

            #region 動作更新
            public override void Start()
            {
                dodgeDistance = 0;

                actionController.AudioSource.PlayOneShot(actionProvider.SpecialAttack2Sound);

                actionController.CharacterAnimator.SetBool("SpecialAttackCharge", false);
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector =
                        IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                        * actionController.CharacterData.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    actionController.MovementBody.MovePosition(
                        actionController.MovementBody.position + dodgeVector);
                }
            }
            #endregion

            #region 外部事件
            public override void OnAnimationEnd()
            {
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void Hit(DamageData damage) { }
            #endregion 
        }

        /// <summary>
        /// 戰士倒地
        /// </summary>
        private class WarriorFallDown : IWarriorAction
        {
            float fallDownTime;

            #region 動作更新
            public override void Start()
            {
                fallDownTime = 0;
                actionController.CharacterData.VertigoConter = 0;

                actionController.AudioSource.PlayOneShot(actionProvider.FallDownSound);

                actionController.CharacterAnimator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                fallDownTime += Time.deltaTime;
                if (fallDownTime > 5)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void End()
            {
                actionController.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion

            #region 外部事件
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    TryToRecurve();
            }

            public override void Dodge()
            {
                TryToRecurve();
            }

            public override void BasicAttack()
            {
                TryToRecurve();
            }

            public override void SpecialAttack()
            {
                TryToRecurve();
            }

            public override void Deffend(bool deffend)
            {
                TryToRecurve();
            }

            public override void Hit(DamageData damage) { }
            #endregion

            private void TryToRecurve()
            {
                fallDownTime += 0.2f;
            }
        }

        /// <summary>
        /// 戰士死亡
        /// </summary>
        private class WarriorDead : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.MovementCollider.enabled = false;

                actionController.AudioSource.PlayOneShot(actionProvider.FallDownSound);

                actionController.CharacterAnimator.SetBool("IsFallDown", true);
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