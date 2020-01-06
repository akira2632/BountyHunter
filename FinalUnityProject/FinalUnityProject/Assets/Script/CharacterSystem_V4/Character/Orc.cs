using UnityEngine;

namespace CharacterSystem_V4
{
    public class Orc : ICharacterActionManager
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

            nowAction = new OrcIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is OrcDead))
                SetAction(new OrcDead());
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

            public override void OnHit(Wound wound)
            {
                orc.RunTimeData.Health -= wound.Damage;
                orc.RunTimeData.VertigoConter += wound.Vertigo;

                if (wound.KnockBackDistance > 0)
                    orc.SetAction(new OrcHurt(wound));
            }
        }

        private class OrcIdle : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                orc.CharacterAnimator.SetFloat("Vertical", orc.RunTimeData.Vertical);
                orc.CharacterAnimator.SetFloat("Horizontal", orc.RunTimeData.Horizontal);

                orc.CharacterAnimator.SetBool("IsFallDown", false);
                orc.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
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
                orc.CharacterAnimator.SetFloat("Vertical", orc.RunTimeData.Vertical);
                orc.CharacterAnimator.SetFloat("Horizontal", orc.RunTimeData.Horizontal);
                orc.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                orc.CharacterAnimator.SetFloat("Vertical", orc.RunTimeData.Vertical);
                orc.CharacterAnimator.SetFloat("Horizontal", orc.RunTimeData.Horizontal);

                orc.MovementBody.MovePosition(orc.MovementBody.position +
                    orc.RunTimeData.IsometricDirection * orc.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                orc.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
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
            bool resetTimer;
            #region 動作更新
            public override void Start()
            {
                if (orc.RunTimeData.AttackTimer > 0)
                {
                    resetTimer = false;
                    orc.SetAction(new OrcIdle());
                }
                else
                {
                    resetTimer = true;
                    orc.animationEnd = false;

                    orc.LightAttackColliders.MyDamage
                        = new Wound { Damage = orc.Property.Damage, Vertigo = 0.4f };

                    orc.CharacterAnimator.SetTrigger("LightAttack");
                    orc.LightAttackSound.Play();
                }
            }

            public override void Update()
            {
                if (orc.animationEnd)
                    actionManager.SetAction(new OrcIdle());
            }

            public override void End()
            {
                if (resetTimer)
                    orc.RunTimeData.AttackTimer = orc.Property.AttackSpeed;
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

        private class OrcHurt : IOrcAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private Wound wound;

            public OrcHurt(Wound wound)
            {
                this.wound = wound;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = CharacterRunTimeData.ToIsometricDirection(
                        wound.KnockBackFrom - orc.MovementBody.position).normalized;
                orc.CharacterAnimator.SetBool("IsHurt", true);
                orc.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < wound.KnockBackDistance)
                {
                    Vector2 temp = wound.KnockBackSpeed * knockBackDirection * Time.deltaTime;
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