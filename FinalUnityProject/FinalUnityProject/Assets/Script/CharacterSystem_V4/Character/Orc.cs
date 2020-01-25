using UnityEngine;
using CharacterSystem_V4.Skill;

namespace CharacterSystem_V4
{
    public class Orc : ICharacterActionManager
    {
        public Rigidbody2D MovementBody;
        public Collider2D MovementCollider;
        public Animator CharacterAnimator;
        public SpriteRenderer SpriteRenderer;

        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public SkillColliders LightAttackColliders;

        void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property, transform);

            nowAction = new OrcIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is OrcDead))
                SetAction(new OrcDead());
            else if (RunTimeData.Health > 0)
            {
                if (RunTimeData.BasicAttackTimer >= 0)
                    RunTimeData.BasicAttackTimer -= Time.deltaTime;

                RunTimeData.RegenTimer += Time.deltaTime;
                if (RunTimeData.Health < Property.MaxHealth &&
                    RunTimeData.RegenTimer >= Property.RegenSpeed)
                {
                    RunTimeData.Health += Property.RegenHealth;
                    RunTimeData.RegenTimer = 0;
                }

                if (RunTimeData.VertigoConter >= 4 && !(nowAction is OrcFall))
                    SetAction(new OrcFall());

                RunTimeData.VertigoConter -= Time.deltaTime / 10;
            }

            base.ActionUpdate();
        }

        #region OrkActions
        private class IOrcAction : ICharacterAction
        {
            protected Orc orc;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                orc = (Orc)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(DamageData damage)
            {
                orc.RunTimeData.Health -= damage.Damage;
                orc.RunTimeData.VertigoConter += damage.Vertigo;

                if (damage.KnockBackDistance > 0)
                    orc.SetAction(new OrcKnockBack(damage));
            }
        }

        private class OrcIdle : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    orc.RunTimeData.Direction, out var vertical, out var horizontal);
                orc.CharacterAnimator.SetFloat("Vertical", vertical);
                orc.CharacterAnimator.SetFloat("Horizontal", horizontal);

                orc.CharacterAnimator.SetBool("IsFallDown", false);
                orc.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(new OrcLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    orc.RunTimeData.Direction = direction;
                    actionManager.SetAction(new OrcMove());
                }
            }
            #endregion
        }

        private class OrcMove : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                orc.MoveSound.Play();
                IsometricUtility.GetVerticalAndHorizontal(
                    orc.RunTimeData.Direction, out var vertical, out var horizontal);
                orc.CharacterAnimator.SetFloat("Vertical", vertical);
                orc.CharacterAnimator.SetFloat("Horizontal", horizontal);
                orc.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    orc.RunTimeData.Direction, out var vertical, out var horizontal);
                orc.CharacterAnimator.SetFloat("Vertical", vertical);
                orc.CharacterAnimator.SetFloat("Horizontal", horizontal);

                orc.MovementBody.MovePosition(orc.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(orc.RunTimeData.Direction)
                    * orc.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                orc.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
               actionManager.SetAction(new OrcLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(new OrcIdle());
                else
                    orc.RunTimeData.Direction = direction;
            }
            #endregion
        }

        private class OrcLightAttack : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                if (orc.RunTimeData.BasicAttackTimer > 0)
                {
                    orc.SetAction(new OrcIdle());
                    return;
                }

                orc.animationEnd = false;

                orc.CharacterAnimator.SetTrigger("LightAttack");
                orc.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (orc.animationEnd)
                {
                    orc.RunTimeData.BasicAttackTimer = orc.Property.BasicAttackSpeed;
                    actionManager.SetAction(new OrcIdle());
                }
            }
            #endregion
        }

        private class OrcFall : IOrcAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                orc.CharacterAnimator.SetBool("IsFallDown", true);
                orc.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    orc.SetAction(new OrcIdle());
            }

            public override void End()
            {
                orc.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class OrcKnockBack : IOrcAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private DamageData damage;

            public OrcKnockBack(DamageData damage)
            {
                this.damage = damage;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = IsometricUtility.ToIsometricVector2(
                    orc.MovementBody.position - damage.HitFrom).normalized;
                orc.CharacterAnimator.SetBool("IsHurt", true);
                orc.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    orc.MovementBody.MovePosition(orc.MovementBody.position
                        + temp);
                }
                else
                    orc.SetAction(new OrcIdle());
            }

            public override void End()
            {
                orc.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class OrcDead : IOrcAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                orc.MovementCollider.enabled = false;
                orc.CharacterAnimator.SetBool("IsFallDown", true);
                orc.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    orc.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(orc.gameObject);
            }

            public override void End()
            {
                orc.MovementCollider.enabled = true;
                orc.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}