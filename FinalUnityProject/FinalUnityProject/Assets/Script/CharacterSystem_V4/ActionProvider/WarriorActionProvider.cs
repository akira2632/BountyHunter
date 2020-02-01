using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    /// <summary>
    /// 戰士角色
    /// </summary>
    public class WarriorActionProvider : ICharacterActionProvider
    {
        public AudioClip clip;
        public AudioSource MoveSound, DeffendSound, FallDownSound, LightAttackSound,
                HeavyAttack1Sound, HeavyAttackChargeSound, HeavyAttack2Sound;
        public HitEffect DefaultHitEffect, DefaultDeffendEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionManager manager)
        {
            var temp = new WarriorIdel();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public override ICharacterAction GetDeadAction(CharacterActionManager manager)
        {
            var temp = new WarriorDead();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public override ICharacterAction GetFallDownAction(CharacterActionManager manager)
        {
            var temp = new WarriorFall();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetMoveAction(CharacterActionManager manager)
        {
            var temp = new WarriorMove();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetDeffendAction(CharacterActionManager manager)
        {
            var temp = new WarriorDeffend();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetBasicAttackAction(CharacterActionManager manager)
        {
            var temp = new WarriorBasicAttack();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpacilAttackStart(CharacterActionManager manager)
        {
            var temp = new WarriorSpecailAttackStart();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttackDodge(CharacterActionManager manager, bool isCharge)
        {
            var temp = new WarriorSpecailAttackDodge(isCharge);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttack1(CharacterActionManager manager, bool isCharge)
        {
            var temp = new WarriorSpacilAttack1(isCharge);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttackCharge(CharacterActionManager manager)
        {
            var temp = new WarriorSpecailAttackCharge();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttack2(CharacterActionManager manager)
        {
            var temp = new WarriorSpacilAttack2();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }
        #endregion

        #region WarriorActions
        /// <summary>
        /// 戰士角色動作基底類別
        /// </summary>
        private class IWarriorAction : ICharacterAction
        {
            protected WarriorActionProvider provider;

            public void SetProvider(WarriorActionProvider provider)
            {
                this.provider = provider;
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.CharacterData.Health -= damage.Damage;
                actionManager.CharacterData.VertigoConter += damage.Vertigo;
                provider.DefaultHitEffect.PlayEffect(damage);
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
                    actionManager.CharacterData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
            }
            #endregion

            #region 外部操作
            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(provider.GetDeffendAction(actionManager));
            }

            public override void SpecialAttack() =>
               actionManager.SetAction(provider.GetSpacilAttackStart(actionManager));

            public override void BasicAttack() =>
               actionManager.SetAction(provider.GetBasicAttackAction(actionManager));

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionManager.CharacterData.Direction = direction;
                    actionManager.SetAction(provider.GetMoveAction(actionManager));
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
                provider.MoveSound.Play();
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
                provider.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(provider.GetIdelAction(actionManager));
                else
                    actionManager.CharacterData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(provider.GetDeffendAction(actionManager));
            }

            public override void SpecialAttack()
            {
                actionManager.SetAction(provider.GetSpacilAttackStart(actionManager));
            }

            public override void BasicAttack()
            {
                actionManager.SetAction(provider.GetBasicAttackAction(actionManager));
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
                    actionManager.CharacterData.Direction, out var vertical, out var horizontal);
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
                    actionManager.CharacterData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (!deffend)
                    actionManager.SetAction(provider.GetIdelAction(actionManager));
            }

            public override void BasicAttack()
            {
                actionManager.SetAction(provider.GetBasicAttackAction(actionManager));
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.CharacterData.Health -= (int)(damage.Damage * 0.1f);
                provider.DeffendSound.Play();
                provider.DefaultDeffendEffect.PlayEffect(damage);
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
                if (actionManager.CharacterData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(provider.GetIdelAction(actionManager));
                    return;
                }

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
                provider.LightAttackSound.Play();
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.CharacterData.BasicAttackTimer = actionManager.CharacterData.BasicAttackSpeed;
                actionManager.SetAction(provider.GetIdelAction(actionManager));
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊預備
        /// </summary>
        private class WarriorSpecailAttackStart : IWarriorAction
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
                actionManager.SetAction(provider.GetSpecailAttackDodge(actionManager, isCharge));
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
        private class WarriorSpecailAttackDodge : IWarriorAction
        {
            float dodgeDistance, targetDistance = 2f;
            bool isCharge;

            public WarriorSpecailAttackDodge(bool isCharge)
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
                    IsometricUtility.ToIsometricVector2(actionManager.CharacterData.Direction)
                    * actionManager.CharacterData.DodgeSpeed * Time.deltaTime;

                dodgeDistance += dodgeVector.magnitude;

                actionManager.MovementBody.MovePosition(
                    actionManager.MovementBody.position + dodgeVector);

                if (dodgeDistance >= targetDistance)
                    actionManager.SetAction(provider.GetSpecailAttack1(actionManager, isCharge));
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
        private class WarriorSpacilAttack1 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;
            bool isCharge;

            public WarriorSpacilAttack1(bool isCharge)
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
                provider.HeavyAttack1Sound.Play();
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector =
                        IsometricUtility.ToIsometricVector2(actionManager.CharacterData.Direction)
                        * actionManager.CharacterData.DodgeSpeed * Time.deltaTime;

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
                    actionManager.SetAction(provider.GetSpecailAttackCharge(actionManager));
                else
                    actionManager.SetAction(provider.GetIdelAction(actionManager));
            }

            public override void OnHit(DamageData damage) { }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊蓄力
        /// </summary>
        private class WarriorSpecailAttackCharge : IWarriorAction
        {
            bool IsCharge, ChargeEnd;
            float ChargeTime;

            #region 動作更新
            public override void Start()
            {
                IsCharge = true;
                ChargeEnd = false;
                ChargeTime = 0;

                provider.HeavyAttackChargeSound.Play();
            }

            public override void Update()
            {
                if (!ChargeEnd)
                {
                    ChargeTime += Time.deltaTime;

                    IsometricUtility.GetVerticalAndHorizontal(
                        actionManager.CharacterData.Direction, out var vertical, out var horizontal);
                    actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                    actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                    if (!IsCharge || ChargeTime > 2.1)
                        actionManager.SetAction(provider.GetSpecailAttack2(actionManager));
                }
            }

            public override void End()
            {
                provider.HeavyAttackChargeSound.Stop();
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
                    actionManager.CharacterData.Direction = direction;
            }
            #endregion
        }

        /// <summary>
        /// 戰士重攻擊第二段
        /// </summary>
        private class WarriorSpacilAttack2 : IWarriorAction
        {
            float dodgeDistance, targetDistance = 0.4f;

            #region 動作更新
            public override void Start()
            {
                dodgeDistance = 0;
                actionManager.CharacterAnimator.SetBool("HeavyAttackCharge", false);
                provider.HeavyAttack2Sound.Play();
            }

            public override void Update()
            {
                if (dodgeDistance < targetDistance)
                {
                    Vector2 dodgeVector =
                        IsometricUtility.ToIsometricVector2(actionManager.CharacterData.Direction)
                        * actionManager.CharacterData.DodgeSpeed * Time.deltaTime;

                    dodgeDistance += dodgeVector.magnitude;

                    actionManager.MovementBody.MovePosition(
                        actionManager.MovementBody.position + dodgeVector);
                }
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.SetAction(provider.GetIdelAction(actionManager));
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
                actionManager.CharacterData.VertigoConter = 0;

                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                provider.FallDownSound.Play();
            }

            public override void Update()
            {
                fallDownTime += Time.deltaTime;
                if (fallDownTime > 5)
                    actionManager.SetAction(provider.GetIdelAction(actionManager));
            }

            public override void End()
            {
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
                provider.FallDownSound.Play();
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