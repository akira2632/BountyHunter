using UnityEngine;

namespace CharacterSystem_V4
{
    public class OrcCaptain : ICharacterActionManager
    {
        public Rigidbody2D MovementBody;
        public Collider2D MovementCollider;
        public Animator CharacterAnimator;
        public SpriteRenderer SpriteRenderer;

        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public AttackColliders LightAttackColliders;

        void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property);

            nowAction = new OrcCaptainIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is OrcCaptainDead))
                SetAction(new OrcCaptainDead());
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

                RunTimeData.VertigoConter -= Time.deltaTime / 10;
            }

            base.ActionUpdate();
        }

        #region OrkCaptainActions
        private class IOrcCaptainAction : ICharacterAction
        {
            protected OrcCaptain orcCaptain;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                orcCaptain = (OrcCaptain)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Wound wound)
            {
                orcCaptain.RunTimeData.Health -= wound.Damage;

                if (wound.KnockBackDistance > 0)
                    orcCaptain.SetAction(new OrcCaptainHurt(new Wound
                    {
                        Damage = wound.Damage
                    }));
            }
        }

        private class OrcCaptainIdle : IOrcCaptainAction
        {
            #region 動作更新
            public override void Start()
            {
                orcCaptain.CharacterAnimator.SetFloat("Vertical", orcCaptain.RunTimeData.Vertical);
                orcCaptain.CharacterAnimator.SetFloat("Horizontal", orcCaptain.RunTimeData.Horizontal);

                orcCaptain.CharacterAnimator.SetBool("IsFallDown", false);
                orcCaptain.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
                actionManager.SetAction(new OrcCaptainLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    orcCaptain.RunTimeData.Direction = direction;
                    actionManager.SetAction(new OrcCaptainMove());
                }
            }
            #endregion
        }

        private class OrcCaptainMove : IOrcCaptainAction
        {
            #region 動作更新
            public override void Start()
            {
                orcCaptain.MoveSound.Play();
                orcCaptain.CharacterAnimator.SetFloat("Vertical", orcCaptain.RunTimeData.Vertical);
                orcCaptain.CharacterAnimator.SetFloat("Horizontal", orcCaptain.RunTimeData.Horizontal);
                orcCaptain.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                orcCaptain.CharacterAnimator.SetFloat("Vertical", orcCaptain.RunTimeData.Vertical);
                orcCaptain.CharacterAnimator.SetFloat("Horizontal", orcCaptain.RunTimeData.Horizontal);

                orcCaptain.MovementBody.MovePosition(orcCaptain.MovementBody.position +
                    IsometricUtility.ToIsometricDirection(orcCaptain.RunTimeData.Direction)
                    * orcCaptain.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                orcCaptain.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
               actionManager.SetAction(new OrcCaptainLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(new OrcCaptainIdle());
                else
                    orcCaptain.RunTimeData.Direction = direction;
            }
            #endregion
        }

        private class OrcCaptainLightAttack : IOrcCaptainAction
        {
            bool resetTimer;
            #region 動作更新
            public override void Start()
            {
                if (orcCaptain.RunTimeData.AttackTimer > 0)
                {
                    resetTimer = false;
                    orcCaptain.SetAction(new OrcCaptainIdle());
                }
                else
                {
                    resetTimer = true;
                    orcCaptain.animationEnd = false;

                    orcCaptain.LightAttackColliders.MyDamage
                        = new Wound { Damage = orcCaptain.Property.Damage, Vertigo = 0.4f };

                    orcCaptain.CharacterAnimator.SetTrigger("LightAttack");
                    orcCaptain.LightAttackSound.Play();
                }
            }

            public override void Update()
            {
                if (orcCaptain.animationEnd)
                    actionManager.SetAction(new OrcCaptainIdle());
            }

            public override void End()
            {
                if (resetTimer)
                    orcCaptain.RunTimeData.AttackTimer = orcCaptain.Property.AttackSpeed;
            }
            #endregion
        }

        private class OrcCaptainHurt : IOrcCaptainAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private Wound wound;

            public OrcCaptainHurt(Wound wound)
            {
                this.wound = wound;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = IsometricUtility.ToIsometricDirection(
                        wound.HitFrom - orcCaptain.MovementBody.position).normalized;
                orcCaptain.CharacterAnimator.SetBool("IsHurt", true);
                orcCaptain.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < wound.KnockBackDistance)
                {
                    Vector2 temp = wound.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    orcCaptain.MovementBody.MovePosition(orcCaptain.MovementBody.position
                        + temp);
                }
                else
                    orcCaptain.SetAction(new OrcCaptainIdle());
            }

            public override void End()
            {
                orcCaptain.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class OrcCaptainDead : IOrcCaptainAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                orcCaptain.MovementCollider.enabled = false;
                orcCaptain.CharacterAnimator.SetBool("IsFallDown", true);
                orcCaptain.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    orcCaptain.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(orcCaptain.gameObject);
            }

            public override void End()
            {
                orcCaptain.MovementCollider.enabled = true;
                orcCaptain.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}