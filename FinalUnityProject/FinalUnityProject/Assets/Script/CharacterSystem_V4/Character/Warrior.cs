using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    /// <summary>
    /// 戰士角色
    /// </summary>
    public class Warrior : ICharacterActionManager
    {
        public AudioSource MoveSound, DeffendSound, FallDownSound, LightAttackSound,
                HeavyAttack1Sound, HeavyAttackChargeSound, HeavyAttack2Sound;
        public HitEffect DefaultHitEffect, DefaultDeffendEffect;

        public void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property, transform);

            nowAction = new WarriorIdel();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is WarriorDead))
                SetAction(new WarriorDead());

            if (RunTimeData.Health > 0
                && RunTimeData.VertigoConter >= 4
                && !(nowAction is WarriorFall))
                SetAction(new WarriorFall());

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
                actionManager.RunTimeData.Health -= damage.Damage;
                actionManager.RunTimeData.VertigoConter += damage.Vertigo;
                warrior.DefaultHitEffect.PlayEffect(damage);
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
                actionManager.CharacterAnimator.SetBool("IsMove", false);

                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
            }
            #endregion

            #region 外部操作
            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(new WarriorDeffend());
            }

            public override void SpecialAttack() =>
               actionManager.SetAction(new WarriorHeavyAttack_Start());

            public override void BasicAttack() =>
               actionManager.SetAction(new WarriorLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionManager.RunTimeData.Direction = direction;
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
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
                actionManager.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionManager.MovementBody.MovePosition(actionManager.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionManager.RunTimeData.Direction)
                    * actionManager.Property.MoveSpeed * Time.deltaTime);
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
                    actionManager.RunTimeData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(new WarriorDeffend());
            }

            public override void SpecialAttack()
            {
                actionManager.SetAction(new WarriorHeavyAttack_Start());
            }

            public override void BasicAttack()
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
                actionManager.CharacterAnimator.SetBool("IsDeffend", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
            }

            public override void End()
            {
                actionManager.CharacterAnimator.SetBool("IsDeffend", false);
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    actionManager.RunTimeData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (!deffend)
                    actionManager.SetAction(new WarriorIdel());
            }

            public override void BasicAttack()
            {
                actionManager.SetAction(new WarriorLightAttack());
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.RunTimeData.Health -= (int)(damage.Damage * 0.1f);
                warrior.DeffendSound.Play();
                warrior.DefaultDeffendEffect.PlayEffect(damage);
            }
            #endregion
        }

        /// <summary>
        /// 戰士輕攻擊
        /// </summary>
        private class WarriorLightAttack : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionManager.RunTimeData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(new WarriorIdel());
                    return;
                }

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
                warrior.LightAttackSound.Play();
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.RunTimeData.BasicAttackTimer = actionManager.Property.BasicAttackSpeed;
                actionManager.SetAction(new WarriorIdel());
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
                actionManager.CharacterAnimator.SetBool("HeavyAttackStart", true);
            }
            #endregion

            #region 外部操作
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                    isCharge = false;
            }

            public override void OnAnimationEnd()
            {
                actionManager.SetAction(new WarriorHeavyAttack_Dodge(isCharge));
            }

            public override void OnHit(DamageData damage)
            {
                damage.Vertigo = 0;
                base.OnHit(damage);
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
                actionManager.MovementCollider.isTrigger = true;
                dodgeDistance = 0;
            }

            public override void Update()
            {
                Vector2 dodgeVector =
                    IsometricUtility.ToIsometricVector2(actionManager.RunTimeData.Direction)
                    * actionManager.Property.DodgeSpeed * Time.deltaTime;

                dodgeDistance += dodgeVector.magnitude;

                actionManager.MovementBody.MovePosition(
                    actionManager.MovementBody.position + dodgeVector);

                if (dodgeDistance >= targetDistance)
                    actionManager.SetAction(new WarriorHeavyAttack1(isCharge));
            }

            public override void End()
            {
                //GameManager.Instance.WarriorDodge(false);
                actionManager.MovementCollider.isTrigger = false;
            }
            #endregion

            #region 外部操作
            public override void SpecialAttack(bool hold)
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

                if (isCharge)
                    actionManager.CharacterAnimator.SetBool("HeavyAttackCharge", true);

                actionManager.CharacterAnimator.SetBool("HeavyAttackStart", false);
                warrior.HeavyAttack1Sound.Play();
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector =
                        IsometricUtility.ToIsometricVector2(actionManager.RunTimeData.Direction)
                        * actionManager.Property.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    actionManager.MovementBody.MovePosition(
                        actionManager.MovementBody.position + dodgeVector);
                }
            }
            #endregion

            #region 外部操作
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                {
                    isCharge = false;
                    actionManager.CharacterAnimator.SetBool("HeavyAttackCharge", false);
                }
            }

            public override void OnAnimationEnd()
            {
                if (isCharge)
                    actionManager.SetAction(new WarriorHeavyAttackCharge());
                else
                    actionManager.SetAction(new WarriorIdel());
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

                    IsometricUtility.GetVerticalAndHorizontal(
                        actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                    actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                    actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                    if (!IsCharge || ChargeTime > 2.1)
                        actionManager.SetAction(new WarriorHeavyAttack2());
                }
            }

            public override void End()
            {
                warrior.HeavyAttackChargeSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void SpecialAttack(bool hold)
            {
                if (!hold)
                    IsCharge = false;
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    actionManager.RunTimeData.Direction = direction;
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊第二段
        /// </summary>
        private class WarriorHeavyAttack2 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;

            #region 動作更新
            public override void Start()
            {
                dodgeDistance = 0;
                actionManager.CharacterAnimator.SetBool("HeavyAttackCharge", false);
                warrior.HeavyAttack2Sound.Play();
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector =
                        IsometricUtility.ToIsometricVector2(actionManager.RunTimeData.Direction)
                        * actionManager.Property.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    actionManager.MovementBody.MovePosition(
                        actionManager.MovementBody.position + dodgeVector);
                }
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.SetAction(new WarriorIdel());
            }

            public override void OnHit(DamageData damage) { }
            #endregion 
        }

        /// <summary>
        /// 戰士倒地
        /// </summary>
        private class WarriorFall : IWarriorAction
        {
            float fallDownTime;

            #region 動作更新
            public override void Start()
            {
                fallDownTime = 0;

                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
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
                actionManager.RunTimeData.VertigoConter = 0;
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion

            #region 外部操作
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

            public override void OnHit(DamageData damage) { }
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
                actionManager.MovementCollider.enabled = false;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                warrior.FallDownSound.Play();
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