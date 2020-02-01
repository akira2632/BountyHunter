using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    /// <summary>
    /// 戰士角色
    /// </summary>
    public class WarriorActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, DeffendSound, FallDownSound, BasicAttackSound,
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

        private ICharacterAction GetSpacilAttackStartAction(CharacterActionManager manager)
        {
            var temp = new WarriorSpecailAttackStart();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttackDodgeAction(CharacterActionManager manager, bool isCharge)
        {
            var temp = new WarriorSpecailAttackDodge(isCharge);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttack1Action(CharacterActionManager manager, bool isCharge)
        {
            var temp = new WarriorSpacilAttack1(isCharge);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttackChargeAction(CharacterActionManager manager)
        {
            var temp = new WarriorSpecailAttackCharge();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        private ICharacterAction GetSpecailAttack2Action(CharacterActionManager manager)
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
            protected WarriorActionProvider actionProvider;

            public void SetProvider(WarriorActionProvider actionProvider)
            {
                this.actionProvider = actionProvider;
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.CharacterData.Health -= damage.Damage;
                actionManager.CharacterData.VertigoConter += damage.Vertigo;
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
                    actionManager.SetAction(actionProvider.GetDeffendAction(actionManager));
            }

            public override void SpecialAttack() =>
               actionManager.SetAction(actionProvider.GetSpacilAttackStartAction(actionManager));

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

        /// <summary>
        /// 戰士移動
        /// </summary>
        private class WarriorMove : IWarriorAction
        {
            #region 動作更新
            public override void Start()
            {
                actionManager.AudioSource.clip = actionProvider.MoveSound;
                actionManager.AudioSource.loop = true;
                actionManager.AudioSource.Play();

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
                actionManager.AudioSource.Stop();
                actionManager.AudioSource.loop = false;
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
                else
                    actionManager.CharacterData.Direction = direction;
            }

            public override void Deffend(bool deffend)
            {
                if (deffend)
                    actionManager.SetAction(actionProvider.GetDeffendAction(actionManager));
            }

            public override void SpecialAttack()
            {
                actionManager.SetAction(actionProvider.GetSpacilAttackStartAction(actionManager));
            }

            public override void BasicAttack()
            {
                actionManager.SetAction(actionProvider.GetBasicAttackAction(actionManager));
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
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
            }

            public override void BasicAttack()
            {
                actionManager.SetAction(actionProvider.GetBasicAttackAction(actionManager));
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.CharacterData.Health -= (int)(damage.Damage * 0.1f);

                actionManager.AudioSource.clip = actionProvider.DeffendSound;
                actionManager.AudioSource.Play();

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
                if (actionManager.CharacterData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
                    return;
                }

                actionManager.AudioSource.clip = actionProvider.BasicAttackSound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
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
                actionManager.SetAction(actionProvider.GetSpecailAttackDodgeAction(actionManager, isCharge));
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
                    actionManager.SetAction(actionProvider.GetSpecailAttack1Action(actionManager, isCharge));
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

                actionManager.AudioSource.clip = actionProvider.HeavyAttack1Sound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetBool("HeavyAttackStart", false);
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
                    actionManager.SetAction(actionProvider.GetSpecailAttackChargeAction(actionManager));
                else
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
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

                actionManager.AudioSource.clip = actionProvider.HeavyAttackChargeSound;
                actionManager.AudioSource.loop = true;
                actionManager.AudioSource.Play();
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
                        actionManager.SetAction(actionProvider.GetSpecailAttack2Action(actionManager));
                }
            }

            public override void End()
            {
                actionManager.AudioSource.Stop();
                actionManager.AudioSource.loop = false;
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

                actionManager.AudioSource.clip = actionProvider.HeavyAttack2Sound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetBool("HeavyAttackCharge", false);
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
                actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
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

                actionManager.AudioSource.clip = actionProvider.FallDownSound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                fallDownTime += Time.deltaTime;
                if (fallDownTime > 5)
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
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

                actionManager.AudioSource.clip = actionProvider.FallDownSound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
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