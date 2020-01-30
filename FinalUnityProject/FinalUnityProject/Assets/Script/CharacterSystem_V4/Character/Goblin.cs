using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class Goblin : ICharacterActionManager
    {
        public Rigidbody2D MovementBody;
        public Collider2D MovementCollider;
        public Animator CharacterAnimator;
        public SpriteRenderer SpriteRenderer;

        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public HitEffect DefaultHitEffect;

        void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property, transform);

            nowAction = new GoblinIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is GoblinDead))
                SetAction(new GoblinDead());

            if (RunTimeData.Health > 0
                && RunTimeData.VertigoConter >= 4
                && !(nowAction is GoblinFall))
                SetAction(new GoblinFall());

            base.ActionUpdate();
        }

        #region GoblinActions
        private class IGoblinAction : ICharacterAction
        {
            protected Goblin goblin;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                goblin = (Goblin)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(DamageData damage)
            {
                goblin.RunTimeData.Health -= damage.Damage;
                goblin.RunTimeData.VertigoConter += damage.Vertigo;

                goblin.DefaultHitEffect.PlayEffect(damage);
                if (damage.KnockBackDistance > 0)
                    goblin.SetAction(new GoblinKnockBack(damage));
            }
        }

        private class GoblinIdle : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    goblin.RunTimeData.Direction, out var vertical, out var horizontal);
                goblin.CharacterAnimator.SetFloat("Vertical", vertical);
                goblin.CharacterAnimator.SetFloat("Horizontal", horizontal);

                goblin.CharacterAnimator.SetBool("IsFallDown", false);
                goblin.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(new GoblinBasicAttack());

            public override void SpecialAttack() =>
                actionManager.SetAction(new GoblinSpacilAttack());

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                goblin.RunTimeData.TargetPosition = tartgetPosition;
                actionManager.SetAction(new GoblinSpacilAttack(tartgetPosition));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    goblin.RunTimeData.Direction = direction;
                    actionManager.SetAction(new GoblinMove());
                }
            }
            #endregion
        }

        private class GoblinMove : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                goblin.MoveSound.Play();
                IsometricUtility.GetVerticalAndHorizontal(
                    goblin.RunTimeData.Direction, out var vertical, out var horizontal);
                goblin.CharacterAnimator.SetFloat("Vertical", vertical);
                goblin.CharacterAnimator.SetFloat("Horizontal", horizontal);
                goblin.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    goblin.RunTimeData.Direction, out var vertical, out var horizontal);
                goblin.CharacterAnimator.SetFloat("Vertical", vertical);
                goblin.CharacterAnimator.SetFloat("Horizontal", horizontal);

                goblin.MovementBody.MovePosition(goblin.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(goblin.RunTimeData.Direction)
                    * goblin.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                goblin.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(new GoblinBasicAttack());

            public override void SpecialAttack() =>
                actionManager.SetAction(new GoblinSpacilAttack());

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                goblin.RunTimeData.TargetPosition = tartgetPosition;
                actionManager.SetAction(new GoblinSpacilAttack(tartgetPosition));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(new GoblinIdle());
                else
                    goblin.RunTimeData.Direction = direction;
            }
            #endregion
        }

        private class GoblinBasicAttack : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                if (goblin.RunTimeData.BasicAttackTimer > 0)
                {
                    goblin.SetAction(new GoblinIdle());
                    return;
                }

                goblin.animationEnd = false;

                goblin.CharacterAnimator.SetTrigger("LightAttack");
                goblin.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (goblin.animationEnd)
                {
                    goblin.RunTimeData.BasicAttackTimer = goblin.Property.BasicAttackSpeed;
                    actionManager.SetAction(new GoblinIdle());
                }
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

            public override void Start()
            {
                if (goblin.RunTimeData.SpacilAttackTimer > 0)
                {
                    goblin.SetAction(new GoblinIdle());
                    return;
                }

                goblin.animationEnd = false;

                if (hasTarget)
                {
                    IsometricUtility.GetVerticalAndHorizontal(
                        targetPosition - goblin.transform.position, out var vertical, out var horizontal);
                    goblin.CharacterAnimator.SetFloat("Vertical", vertical);
                    goblin.CharacterAnimator.SetFloat("Horizontal", horizontal);
                }
                goblin.CharacterAnimator.SetTrigger("RangeAttack");
            }

            public override void Update()
            {
                if (goblin.animationEnd)
                {
                    goblin.RunTimeData.SpacilAttackTimer = goblin.Property.SpacilAttackSpeed;
                    actionManager.SetAction(new GoblinIdle());
                }
            }
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
                    goblin.MovementBody.position - damage.HitFrom).normalized;
                goblin.CharacterAnimator.SetBool("IsHurt", true);
                goblin.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    goblin.MovementBody.MovePosition(goblin.MovementBody.position
                        + temp);
                }
                else
                    goblin.SetAction(new GoblinIdle());
            }

            public override void End()
            {
                goblin.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class GoblinFall : IGoblinAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                goblin.CharacterAnimator.SetBool("IsFallDown", true);
                goblin.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    goblin.SetAction(new GoblinIdle());
            }

            public override void End()
            {
                goblin.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class GoblinDead : IGoblinAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 10;

                goblin.MovementCollider.enabled = false;
                goblin.CharacterAnimator.SetBool("IsFallDown", true);
                goblin.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    goblin.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(goblin.gameObject);
            }

            public override void End()
            {
                goblin.MovementCollider.enabled = true;
                goblin.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}