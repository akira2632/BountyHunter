using UnityEngine;

namespace CharacterSystem_V4
{
    /// <summary>
    /// 戰士角色
    /// </summary>
    public class Warrior : ICharacterActionManager
    {
        public Rigidbody2D MovementBody;
        public Collider2D MovementCollider;
        public Animator CharacterAnimator;

        public AudioSource MoveSound, DeffendSound, FallDownSound, LightAttackSound,
                HeavyAttack1Sound, HeavyAttackChargeSound, HeavyAttack2Sound;
        public AttackColliders LightAttackColliders, HeavyAttack1Colliders, HeavyAttack2Colliders;

        public void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property);

            nowAction = new WarriorIdel();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is WarriorDead))
                SetAction(new WarriorDead());
            else if (RunTimeData.Health > 0)
            {
                if (RunTimeData.AttackTimer >= 0)
                    RunTimeData.AttackTimer -= Time.deltaTime;

                RunTimeData.RegenTimer += Time.deltaTime;
                if (RunTimeData.Health < Property.MaxHealth &&
                    RunTimeData.RegenTimer >= Property.RegenSpeed)
                {
                    RunTimeData.Health += Property.RegenHealth;
                    RunTimeData.RegenTimer = 0;
                }

                if (RunTimeData.VertigoConter >= 4 && !(nowAction is WarriorFall))
                    SetAction(new WarriorFall(false));

                RunTimeData.VertigoConter -= Time.deltaTime / 10;
            }

            base.ActionUpdate();
        }

        #region WarriorActions
        /// <summary>
        /// 戰士角色動作基底類別
        /// </summary>
        private class IWarriorAction : ICharacterAction
        {
            protected Warrior warrior;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                warrior = (Warrior)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(DamageData damage)
            {
                warrior.RunTimeData.Health -= damage.Damage;
                warrior.RunTimeData.VertigoConter += damage.Vertigo;
                actionManager.DamageEffector.PlayHitEffect(damage);
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
                warrior.CharacterAnimator.SetBool("IsFallDown", false);
                warrior.CharacterAnimator.SetBool("IsMove", false);

                warrior.CharacterAnimator.SetFloat("Vertical", warrior.RunTimeData.Vertical);
                warrior.CharacterAnimator.SetFloat("Horizontal", warrior.RunTimeData.Horizontal);
            }
            #endregion

            #region 外部操作
            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(new WarriorDeffend());
            }

            public override void HeavyAttack() =>
               actionManager.SetAction(new WarriorHeavyAttack_Start());

            public override void LightAttack() =>
               actionManager.SetAction(new WarriorLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    warrior.RunTimeData.Direction = direction;
                    actionManager.SetAction(new WarriorMove());
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
                warrior.MoveSound.Play();
                warrior.CharacterAnimator.SetFloat("Vertical", warrior.RunTimeData.Vertical);
                warrior.CharacterAnimator.SetFloat("Horizontal", warrior.RunTimeData.Horizontal);
                warrior.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                warrior.CharacterAnimator.SetFloat("Vertical", warrior.RunTimeData.Vertical);
                warrior.CharacterAnimator.SetFloat("Horizontal", warrior.RunTimeData.Horizontal);

                warrior.MovementBody.MovePosition(warrior.MovementBody.position +
                    IsometricUtility.ToIsometricDirection(warrior.RunTimeData.Direction)
                    * warrior.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                warrior.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(new WarriorIdel());
                else
                    warrior.RunTimeData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(new WarriorDeffend());
            }

            public override void HeavyAttack()
            {
                actionManager.SetAction(new WarriorHeavyAttack_Start());
            }

            public override void LightAttack()
            {
                actionManager.SetAction(new WarriorLightAttack());
            }
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
                warrior.CharacterAnimator.SetBool("IsDeffend", true);
            }

            public override void Update()
            {
                warrior.CharacterAnimator.SetFloat("Vertical", warrior.RunTimeData.Vertical);
                warrior.CharacterAnimator.SetFloat("Horizontal", warrior.RunTimeData.Horizontal);
            }

            public override void End()
            {
                warrior.CharacterAnimator.SetBool("IsDeffend", false);
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    warrior.RunTimeData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (!deffend)
                    actionManager.SetAction(new WarriorIdel());
            }

            public override void LightAttack()
            {
                actionManager.SetAction(new WarriorLightAttack());
            }

            public override void OnHit(DamageData damage)
            {
                warrior.DeffendSound.Play();
                actionManager.DamageEffector.PlayDeffendEffect(damage);
                base.OnHit(new DamageData
                {
                    Damage = (int)(damage.Damage * 0.1f)
                });
            }
            #endregion
        }

        /// <summary>
        /// 戰士輕攻擊
        /// </summary>
        private class WarriorLightAttack : IWarriorAction
        {
            bool resetTimer;
            #region 動作更新
            public override void Start()
            {
                if (warrior.RunTimeData.AttackTimer > 0)
                {
                    resetTimer = false;
                    warrior.SetAction(new WarriorIdel());
                }
                else
                {
                    resetTimer = true;
                    warrior.animationEnd = false;

                    warrior.LightAttackColliders.MyDamage
                        = new DamageData { Damage = warrior.Property.Damage, Vertigo = 1 };

                    warrior.CharacterAnimator.SetTrigger("LightAttack");
                    warrior.LightAttackSound.Play();
                }
            }

            public override void Update()
            {
                if (warrior.animationEnd)
                    actionManager.SetAction(new WarriorIdel());
            }

            public override void End()
            {
                if (resetTimer)
                    warrior.RunTimeData.AttackTimer = warrior.Property.AttackSpeed;
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊預備
        /// </summary>
        private class WarriorHeavyAttack_Start : IWarriorAction
        {
            bool isCharge;

            #region 動作更新
            public override void Start()
            {
                isCharge = true;
                warrior.animationEnd = false;
                warrior.CharacterAnimator.SetBool("HeavyAttackStart", true);
            }

            public override void Update()
            {
                if (warrior.animationEnd)
                    actionManager.SetAction(new WarriorHeavyAttack_Dodge(isCharge));
            }
            #endregion

            #region 外部操作
            public override void HeavyAttack(bool hold)
            {
                if (!hold)
                    isCharge = false;
            }

            public override void OnHit(DamageData damage)
            {
                base.OnHit(new DamageData
                {
                    Damage = damage.Damage
                });
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊衝刺
        /// </summary>
        private class WarriorHeavyAttack_Dodge : IWarriorAction
        {
            float dodgeDistance, targetDistance = 2f;
            bool isCharge;

            public WarriorHeavyAttack_Dodge(bool isCharge)
            {
                this.isCharge = isCharge;
            }

            #region 動作更新
            public override void Start()
            {
                //GameManager.Instance.WarriorDodge(true);
                warrior.MovementCollider.isTrigger = true;
                dodgeDistance = 0;
            }

            public override void Update()
            {
                Vector2 dodgeVector = 
                    IsometricUtility.ToIsometricDirection(warrior.RunTimeData.Direction)
                    * warrior.Property.DodgeSpeed * Time.deltaTime;

                dodgeDistance += dodgeVector.magnitude;

                warrior.MovementBody.MovePosition(
                    warrior.MovementBody.position + dodgeVector);

                if (dodgeDistance >= targetDistance)
                    actionManager.SetAction(new WarriorHeavyAttack1(isCharge));
            }

            public override void End()
            {
                //GameManager.Instance.WarriorDodge(false);
                warrior.MovementCollider.isTrigger = false;
            }
            #endregion

            #region 外部操作
            public override void HeavyAttack(bool hold)
            {
                if (!hold)
                    isCharge = false;
            }

            public override void OnHit(DamageData damage) { }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊第一段
        /// </summary>
        private class WarriorHeavyAttack1 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;
            bool isCharge;

            public WarriorHeavyAttack1(bool isCharge)
            {
                this.isCharge = isCharge;
            }

            #region 動作更新
            public override void Start()
            {
                dodgeDistance = 0;
                warrior.animationEnd = false;
                warrior.HeavyAttack1Colliders.MyDamage
                    = new DamageData { Damage = warrior.Property.Damage * 2, Vertigo = 3 };

                if (isCharge)
                    warrior.CharacterAnimator.SetBool("HeavyAttackCharge", true);

                warrior.CharacterAnimator.SetBool("HeavyAttackStart", false);
                warrior.HeavyAttack1Sound.Play();
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector = 
                        IsometricUtility.ToIsometricDirection(warrior.RunTimeData.Direction)
                        * warrior.Property.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    warrior.MovementBody.MovePosition(
                        warrior.MovementBody.position + dodgeVector);
                }

                if (warrior.animationEnd)
                {
                    if (isCharge)
                        actionManager.SetAction(new WarriorHeavyAttackCharge());
                    else
                        actionManager.SetAction(new WarriorIdel());
                }
            }
            #endregion

            #region 外部操作
            public override void HeavyAttack(bool hold)
            {
                if (!hold)
                {
                    isCharge = false;
                    warrior.CharacterAnimator.SetBool("HeavyAttackCharge", false);
                }
            }

            public override void OnHit(DamageData damage) { }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊蓄力
        /// </summary>
        private class WarriorHeavyAttackCharge : IWarriorAction
        {
            bool IsCharge, ChargeEnd;
            float ChargeTime;

            #region 動作更新
            public override void Start()
            {
                IsCharge = true;
                ChargeEnd = false;
                ChargeTime = 0;

                warrior.HeavyAttackChargeSound.Play();
            }

            public override void Update()
            {
                if (!ChargeEnd)
                {
                    ChargeTime += Time.deltaTime;

                    warrior.CharacterAnimator.SetFloat("Vertical", Mathf.Floor(warrior.RunTimeData.Direction.y));
                    warrior.CharacterAnimator.SetFloat("Horizontal", Mathf.Floor(warrior.RunTimeData.Direction.x));

                    if (!IsCharge || ChargeTime > 2.1)
                    {
                        if (ChargeTime < 0.7)
                            actionManager.SetAction(new WarriorHeavyAttack2(0));
                        else if (ChargeTime < 1.4)
                            actionManager.SetAction(new WarriorHeavyAttack2(1));
                        else
                            actionManager.SetAction(new WarriorHeavyAttack2(2));
                    }
                }
            }

            public override void End()
            {
                warrior.HeavyAttackChargeSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void HeavyAttack(bool hold)
            {
                if (!hold)
                    IsCharge = false;
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    warrior.RunTimeData.Direction = direction;
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊第二段
        /// </summary>
        private class WarriorHeavyAttack2 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;
            int chargeState;

            public WarriorHeavyAttack2(int chargeState)
            {
                this.chargeState = chargeState;
            }

            #region 動作更新
            public override void Start()
            {
                dodgeDistance = 0;
                warrior.animationEnd = false;
                warrior.HeavyAttack2Colliders.MyDamage
                    = new DamageData { Damage = warrior.Property.Damage * 5, Vertigo = 3 };

                warrior.CharacterAnimator.SetBool("HeavyAttackCharge", false);
                warrior.HeavyAttack2Sound.Play();
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector = 
                        IsometricUtility.ToIsometricDirection(warrior.RunTimeData.Direction)
                        * warrior.Property.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    warrior.MovementBody.MovePosition(
                        warrior.MovementBody.position + dodgeVector);
                }

                if (warrior.animationEnd)
                {
                    if (chargeState == 2)
                        actionManager.SetAction(new WarriorHeavyAttackRecovery());
                    else
                        actionManager.SetAction(new WarriorIdel());
                }
            }
            #endregion

            #region 外部操作
            public override void OnHit(DamageData damage) { }
            #endregion 
        }

        /// <summary>
        /// 戰士重攻擊硬直
        /// </summary>
        private class WarriorHeavyAttackRecovery : IWarriorAction
        {
            float recoveryTime;

            #region 動作更新
            public override void Start()
            {
                recoveryTime = 0;
                warrior.CharacterAnimator.SetBool("IsMove", false);
            }

            public override void Update()
            {
                recoveryTime += Time.deltaTime;
                if (recoveryTime > 0.5f)
                    actionManager.SetAction(new WarriorIdel());
            }
            #endregion

            #region 外部操作
            public override void OnHit(DamageData damage)
            {
                base.OnHit(new DamageData
                {
                    Damage = (int)(damage.Damage * 2.5)
                });
                actionManager.SetAction(new WarriorFall(true));
            }
            #endregion
        }

        /// <summary>
        /// 戰士倒地
        /// </summary>
        private class WarriorFall : IWarriorAction
        {
            bool hitable;
            float fallDownTime;

            public WarriorFall(bool hitable)
            {
                this.hitable = hitable;
            }

            #region 動作更新
            public override void Start()
            {
                fallDownTime = 0;

                warrior.CharacterAnimator.SetBool("IsFallDown", true);
                warrior.FallDownSound.Play();
            }

            public override void Update()
            {
                fallDownTime += Time.deltaTime;
                if (fallDownTime > 5)
                    actionManager.SetAction(new WarriorIdel());
            }

            public override void End()
            {
                warrior.RunTimeData.VertigoConter = 0;
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if(direction.magnitude > 0)
                    TryToRecurve();
            }

            public override void Dodge()
            {
                TryToRecurve();
            }

            public override void LightAttack()
            {
                TryToRecurve();
            }

            public override void HeavyAttack()
            {
                TryToRecurve();
            }

            public override void Deffend(bool deffend)
            {
                TryToRecurve();
            }

            public override void OnHit(DamageData damage)
            {
                if (hitable)
                    base.OnHit(new DamageData
                    {
                        Damage = (int)(damage.Damage * 0.5),
                        Vertigo = damage.Vertigo * 0.5f
                    });
            }
            #endregion

            private void TryToRecurve()
            {
                if (hitable)
                    fallDownTime += 0.1f;
                else
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
                warrior.MovementCollider.enabled = false;
                warrior.CharacterAnimator.SetBool("IsFallDown", true);
                warrior.FallDownSound.Play();
            }

            public override void End()
            {
                warrior.MovementCollider.enabled = true;
                warrior.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}