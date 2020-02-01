using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class GoblinActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, FallDownSound, BasicAttackSound, HurtSound;
        public HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionManager manager)
        {
            var temp = new GoblinIdle();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public override ICharacterAction GetDeadAction(CharacterActionManager manager)
        {
            var temp = new GoblinDead();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public override ICharacterAction GetFallDownAction(CharacterActionManager manager)
        {
            var temp = new GoblinFall();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetKnockBackAction(CharacterActionManager manager, DamageData damage)
        {
            var temp = new GoblinKnockBack(damage);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetMoveAction(CharacterActionManager manager)
        {
            var temp = new GoblinMove();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetBasicAttackAction(CharacterActionManager manager)
        {
            var temp = new GoblinBasicAttack();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetSpecailAttackAction(CharacterActionManager manager)
        {
            var temp = new GoblinSpacilAttack();
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }

        public ICharacterAction GetSpecailAttackAction(CharacterActionManager manager, Vector3 targetPosition)
        {
            var temp = new GoblinSpacilAttack(targetPosition);
            temp.SetManager(manager);
            temp.SetProvider(this);
            return temp;
        }
        #endregion

        #region GoblinActions
        private class IGoblinAction : ICharacterAction
        {
            protected GoblinActionProvider actionProvider;

            public void SetProvider(GoblinActionProvider actionProvider)
            {
                this.actionProvider = actionProvider;
            }

            public override void OnHit(DamageData damage)
            {
                actionManager.CharacterData.Health -= damage.Damage;
                actionManager.CharacterData.VertigoConter += damage.Vertigo;

                actionProvider.DefaultHitEffect.PlayEffect(damage);
                if (damage.KnockBackDistance > 0)
                    actionManager.SetAction(actionProvider.GetKnockBackAction(actionManager, damage));
            }
        }

        private class GoblinIdle : IGoblinAction
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

            public override void SpecialAttack() =>
                actionManager.SetAction(actionProvider.GetSpecailAttackAction(actionManager));

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                actionManager.CharacterData.TargetPosition = tartgetPosition;
                actionManager.SetAction(actionProvider.GetSpecailAttackAction(actionManager, tartgetPosition));
            }

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

        private class GoblinMove : IGoblinAction
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
            public override void BasicAttack() =>
                actionManager.SetAction(actionProvider.GetBasicAttackAction(actionManager));

            public override void SpecialAttack() =>
                actionManager.SetAction(actionProvider.GetSpecailAttackAction(actionManager));

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                actionManager.CharacterData.TargetPosition = tartgetPosition;
                actionManager.SetAction(actionProvider.GetSpecailAttackAction(actionManager, tartgetPosition));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
                else
                    actionManager.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class GoblinBasicAttack : IGoblinAction
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
                actionManager.AudioSource.clip = actionProvider.BasicAttackSound;
                actionManager.AudioSource.Play();
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
                if (actionManager.CharacterData.SpacilAttackTimer > 0)
                {
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
                    return;
                }

                if (hasTarget)
                {
                    IsometricUtility.GetVerticalAndHorizontal(
                        targetPosition - actionManager.transform.position, out var vertical, out var horizontal);
                    actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                    actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
                }
                actionManager.CharacterAnimator.SetTrigger("RangeAttack");
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.CharacterData.SpacilAttackTimer = actionManager.CharacterData.SpacilAttackSpeed;
                actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
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
                    actionManager.MovementBody.position - damage.HitFrom).normalized;

                actionManager.AudioSource.clip = actionProvider.HurtSound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetBool("IsHurt", true);
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    actionManager.MovementBody.MovePosition(actionManager.MovementBody.position
                        + temp);
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

        private class GoblinFall : IGoblinAction
        {
            float fallDownTimer;
            #region 動作更新
            public override void Start()
            {
                fallDownTimer = 2;
                actionManager.CharacterData.VertigoConter = 0;

                actionManager.AudioSource.clip = actionProvider.HurtSound;
                actionManager.AudioSource.Play();

                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionManager.SetAction(actionProvider.GetIdelAction(actionManager));
            }

            public override void End()
            {
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }

        private class GoblinDead : IGoblinAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 10;

                actionManager.AudioSource.clip = actionProvider.FallDownSound;
                actionManager.AudioSource.Play();

                actionManager.MovementCollider.enabled = false;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
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