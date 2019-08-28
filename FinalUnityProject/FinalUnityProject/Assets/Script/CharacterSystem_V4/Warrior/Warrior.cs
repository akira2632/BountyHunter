using UnityEngine;

namespace CharacterSystem_V4
{
    public class Warrior : ICharacterActionManager
    {
        public CharacterProperty Property;
        private CharacterRunTimeData RunTimeData;

        public Rigidbody2D MovementBody;
        public Animator CharacterAnimator;
        public bool AnimationEnd;

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
            if (RunTimeData.Health <= 0)
                SetAction(new WarriorDead());
            else
            {
                RunTimeData.AttackTimer += Time.deltaTime;

                RunTimeData.RegenTimer += Time.deltaTime;
                if (RunTimeData.RegenTimer > Property.CharacterRegenSpeed)
                {
                    RunTimeData.RegenTimer = 0;
                    RunTimeData.Health += Property.CharacterRegenHealth;
                }

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
            protected Vertical verticalBuffer;
            protected Horizontal horizontalBuffer;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                warrior = (Warrior)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Damage damage)
            {
                warrior.RunTimeData.Health -= damage.damage;
                warrior.RunTimeData.VertigoConter += damage.vertigo;
            }
        }

        /// <summary>
        /// 玩家待機
        /// </summary>
        private class WarriorIdel : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                warrior.CharacterAnimator.SetBool("IsFallDown", false);
                warrior.CharacterAnimator.SetBool("IsMove", false);
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

            public override void Move(Vertical direction)
            {
                warrior.RunTimeData.Vertical = direction;
                actionManager.SetAction(new WarriorMove());
            }

            public override void Move(Horizontal direction)
            {
                warrior.RunTimeData.Horizontal = direction;
                actionManager.SetAction(new WarriorMove());
            }
            #endregion
        }

        /// <summary>
        /// 玩家移動
        /// </summary>
        private class WarriorMove : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                horizontalBuffer = warrior.RunTimeData.Horizontal;
                verticalBuffer = warrior.RunTimeData.Vertical;

                warrior.MoveSound.Play();
                warrior.CharacterAnimator.SetFloat("Vertical", (float)warrior.RunTimeData.Vertical);
                warrior.CharacterAnimator.SetFloat("Horizontal", (float)warrior.RunTimeData.Horizontal);
                warrior.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                if (verticalBuffer == Vertical.None && horizontalBuffer == Horizontal.None)
                {
                    actionManager.SetAction(new WarriorIdel());
                }
                else
                {
                    warrior.RunTimeData.Vertical = verticalBuffer;
                    warrior.RunTimeData.Horizontal = horizontalBuffer;

                    warrior.CharacterAnimator.SetFloat("Vertical", (float)warrior.RunTimeData.Vertical);
                    warrior.CharacterAnimator.SetFloat("Horizontal", (float)warrior.RunTimeData.Horizontal);

                    warrior.MovementBody.MovePosition(
                        warrior.MovementBody.position +
                        new Vector2((float)warrior.RunTimeData.Horizontal, (float)warrior.RunTimeData.Vertical * 0.6f).normalized
                        * warrior.Property.MoveSpeed * Time.deltaTime);
                }
            }

            public override void End()
            {
                warrior.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void Move(Vertical direction)
            {
                verticalBuffer = direction;
            }

            public override void Move(Horizontal direction)
            {
                horizontalBuffer = direction;
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
        /// 玩家防禦
        /// </summary>
        private class WarriorDeffend : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                verticalBuffer = warrior.RunTimeData.Vertical;
                horizontalBuffer = warrior.RunTimeData.Horizontal;

                warrior.CharacterAnimator.SetBool("IsDeffend", true);
            }

            public override void Update()
            {
                if (!(verticalBuffer == Vertical.None && horizontalBuffer == Horizontal.None))
                {
                    warrior.RunTimeData.Vertical = verticalBuffer;
                    warrior.RunTimeData.Horizontal = horizontalBuffer;

                    warrior.CharacterAnimator.SetFloat("Vertical", (float)warrior.RunTimeData.Vertical);
                    warrior.CharacterAnimator.SetFloat("Horizontal", (float)warrior.RunTimeData.Horizontal);
                }
            }

            public override void End()
            {
                warrior.CharacterAnimator.SetBool("IsDeffend", false);
            }
            #endregion

            #region 外部操作
            public override void Move(Vertical direction)
            {
                verticalBuffer = direction;
            }

            public override void Move(Horizontal direction)
            {
                horizontalBuffer = direction;
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

            public override void OnHit(Damage damage)
            {
                warrior.DeffendSound.Play();
                base.OnHit(new Damage
                {
                    damage = (int)(damage.damage * 0.1f)
                });
            }
            #endregion
        }

        /// <summary>
        /// 玩家輕攻擊
        /// </summary>
        private class WarriorLightAttack : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                warrior.AnimationEnd = false;

                warrior.LightAttackColliders.MyDamage
                    = new Damage { damage = warrior.Property.Attack, vertigo = 1 };

                warrior.CharacterAnimator.SetTrigger("LightAttack");
                warrior.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (warrior.AnimationEnd)
                    actionManager.SetAction(new WarriorIdel());
            }
            #endregion
        }

        /// <summary>
        /// 玩家重攻擊預備
        /// </summary>
        private class WarriorHeavyAttack_Start : IWarriorAction
        {
            bool isCharge;

            #region 動作更新
            public override void Start()
            {
                isCharge = true;
                warrior.AnimationEnd = false;
                warrior.CharacterAnimator.SetBool("HeavyAttackStart", true);
            }

            public override void Update()
            {
                if (warrior.AnimationEnd)
                    actionManager.SetAction(new WarriorHeavyAttack_Dodge(isCharge));
            }
            #endregion

            #region 外部操作
            public override void HeavyAttack(bool hold)
            {
                if (!hold)
                {
                    Debug.Log("not hold");
                    isCharge = false;
                }
                else
                    Debug.Log("hold");
            }

            public override void OnHit(Damage damage)
            {
                base.OnHit(new Damage
                {
                    damage = damage.damage
                });
            }
            #endregion
        }

        /// <summary>
        /// 玩家重攻擊衝刺
        /// </summary>
        private class WarriorHeavyAttack_Dodge : IWarriorAction
        {
            float DodgeDistance, TargetDistance = 2f;
            bool isCharge;

            public WarriorHeavyAttack_Dodge(bool isCharge)
            {
                this.isCharge = isCharge;
            }

            #region 動作更新
            public override void Start()
            {
                //GameManager.Instance. WarriorDodge();
                DodgeDistance = 0;
            }

            public override void Update()
            {
                Vector2 temp =
                    new Vector2((float)warrior.RunTimeData.Horizontal, (float)warrior.RunTimeData.Vertical * 0.6f).normalized
                    * warrior.Property.DodgeSpeed * Time.deltaTime;

                DodgeDistance += temp.magnitude;

                warrior.MovementBody.MovePosition(
                    warrior.MovementBody.position + temp);

                if (DodgeDistance >= TargetDistance)
                    actionManager.SetAction(new WarriorHeavyAttack1(isCharge));
            }
            #endregion

            #region 外部操作
            public override void HeavyAttack(bool hold)
            {
                if (!hold)
                    isCharge = false;
            }

            public override void OnHit(Damage damage) { }
            #endregion
        }

        /// <summary>
        /// 玩家重攻擊第一段
        /// </summary>
        private class WarriorHeavyAttack1 : IWarriorAction
        {
            bool isCharge;

            public WarriorHeavyAttack1(bool isCharge)
            {
                this.isCharge = isCharge;
            }

            #region 動作更新
            public override void Start()
            {
                warrior.AnimationEnd = false;
                warrior.HeavyAttack1Colliders.MyDamage
                    = new Damage { damage = warrior.Property.Attack * 2, vertigo = 3 };

                warrior.CharacterAnimator.SetBool("HeavyAttackStart", false);
                warrior.HeavyAttack1Sound.Play();
            }

            public override void Update()
            {
                if(warrior.AnimationEnd)
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
                    isCharge = false;
            }

            public override void OnHit(Damage damage) { }
            #endregion
        }

        /// <summary>
        /// 玩家重攻擊蓄力
        /// </summary>
        private class WarriorHeavyAttackCharge : IWarriorAction
        {
            bool IsCharge, ChargeEnd;
            float ChargeTime;
            Vertical vertical;
            Horizontal horizontal;

            #region 動作更新
            public override void Start()
            {
                IsCharge = true;
                ChargeEnd = false;
                ChargeTime = 0;

                vertical = warrior.RunTimeData.Vertical;
                horizontal = warrior.RunTimeData.Horizontal;

                warrior.CharacterAnimator.SetBool("HeavyAttackCharge", true);
                warrior.HeavyAttackChargeSound.Play();
            }

            public override void Update()
            {
                if (!ChargeEnd)
                {
                    ChargeTime += Time.deltaTime;

                    if (!(vertical == Vertical.None && horizontal == Horizontal.None))
                    {
                        warrior.RunTimeData.Vertical = vertical;
                        warrior.RunTimeData.Horizontal = horizontal;

                        warrior.CharacterAnimator.SetFloat("Vertical", (float)warrior.RunTimeData.Vertical);
                        warrior.CharacterAnimator.SetFloat("Horizontal", (float)warrior.RunTimeData.Horizontal);
                    }

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

            public override void Move(Vertical direction)
            {
                if (vertical != direction)
                    vertical = direction;
            }

            public override void Move(Horizontal direction)
            {
                if (horizontal != direction)
                    horizontal = direction;
            }
            #endregion
        }

        /// <summary>
        /// 玩家重攻擊第二段
        /// </summary>
        private class WarriorHeavyAttack2 : IWarriorAction
        {
            int chargeState;

            public WarriorHeavyAttack2(int chargeState)
            {
                this.chargeState = chargeState;
            }

            #region 動作更新
            public override void Start()
            {
                warrior.AnimationEnd = false;
                warrior.HeavyAttack2Colliders.MyDamage
                    = new Damage { damage = warrior.Property.Attack * 5, vertigo = 3 };

                warrior.CharacterAnimator.SetBool("HeavyAttackCharge", false);
                warrior.HeavyAttack2Sound.Play();
            }

            public override void Update()
            {
                if (warrior.AnimationEnd)
                {
                    if (chargeState == 2)
                        actionManager.SetAction(new WarriorHeavyAttackRecovery());
                    else
                        actionManager.SetAction(new WarriorIdel());
                }
            }
            #endregion

            #region 外部操作
            public override void OnHit(Damage damage) { }
            #endregion
        }

        /// <summary>
        /// 玩家重攻擊硬直
        /// </summary>
        private class WarriorHeavyAttackRecovery : IWarriorAction
        {
            float recoveryTime;

            #region 動作更新
            public override void Start()
            {
                recoveryTime = 0;
            }

            public override void Update()
            {
                recoveryTime += Time.deltaTime;
                if (recoveryTime > 2)
                    actionManager.SetAction(new WarriorIdel());
            }
            #endregion

            #region 外部操作
            public override void OnHit(Damage damage)
            {
                base.OnHit(new Damage
                {
                    damage = (int)(damage.damage * 2.5)
                });
                actionManager.SetAction(new WarriorFall(true));
            }
            #endregion
        }

        /// <summary>
        /// 玩家倒地
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
            public override void Move(Vertical direction)
            {
                TryToRecurve(hitable);
            }

            public override void Move(Horizontal direction)
            {
                TryToRecurve(hitable);
            }

            public override void Dodge()
            {
                TryToRecurve(hitable);
            }

            public override void LightAttack()
            {
                TryToRecurve(hitable);
            }

            public override void HeavyAttack()
            {
                TryToRecurve(hitable);
            }

            public override void Deffend(bool deffend)
            {
                TryToRecurve(hitable);
            }

            public override void OnHit(Damage damage)
            {
                if (hitable)
                    base.OnHit(new Damage
                    {
                        damage = (int)(damage.damage * 0.5),
                        vertigo = damage.vertigo * 0.5f
                    });
            }
            #endregion

            private void TryToRecurve(bool IsHitable)
            {
                if (IsHitable)
                    fallDownTime += 0.1f;
                else
                    fallDownTime += 0.2f;
            }
        }

        /// <summary>
        /// 玩家死亡
        /// </summary>
        private class WarriorDead : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                warrior.CharacterAnimator.SetBool("IsFallDown", true);
                warrior.FallDownSound.Play();
            }
            #endregion
        }
        #endregion
    }
}