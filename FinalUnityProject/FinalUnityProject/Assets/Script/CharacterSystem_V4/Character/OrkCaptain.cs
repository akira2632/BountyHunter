using UnityEngine;

namespace CharacterSystem_V4
{
    public class OrkCaptain : ICharacterActionManager
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

            nowAction = new OrkCaptainIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is OrkCaptainDead))
                SetAction(new OrkCaptainDead());
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
        private class IOrkCaptainAction : ICharacterAction
        {
            protected OrkCaptain orkCaptain;
            protected Vertical verticalBuffer;
            protected Horizontal horizontalBuffer;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                orkCaptain = (OrkCaptain)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Wound wound)
            {
                orkCaptain.RunTimeData.Health -= wound.Damage;

                if (wound.KnockBackDistance > 0)
                    orkCaptain.SetAction(new OrkCaptainHurt(new Wound
                    {
                        Damage = wound.Damage,

                    }));
            }
        }

        private class OrkCaptainIdle : IOrkCaptainAction
        {
            #region 動作更新
            public override void Start()
            {
                orkCaptain.CharacterAnimator.SetFloat("Vertical", (float)orkCaptain.RunTimeData.Vertical);
                orkCaptain.CharacterAnimator.SetFloat("Horizontal", (float)orkCaptain.RunTimeData.Horizontal);

                orkCaptain.CharacterAnimator.SetBool("IsFallDown", false);
                orkCaptain.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
                actionManager.SetAction(new OrkCaptainLightAttack());

            public override void Move(Vertical direction) =>
                actionManager.SetAction(new OrkCaptainMove(direction, Horizontal.None));

            public override void Move(Horizontal direction) =>
                actionManager.SetAction(new OrkCaptainMove(Vertical.None, direction));
            #endregion
        }

        private class OrkCaptainMove : IOrkCaptainAction
        {
            public OrkCaptainMove(Vertical vertical, Horizontal horizontal)
            {
                verticalBuffer = vertical;
                horizontalBuffer = horizontal;
            }

            #region 動作更新
            public override void Start()
            {
                orkCaptain.MoveSound.Play();
                orkCaptain.CharacterAnimator.SetFloat("Vertical", (float)orkCaptain.RunTimeData.Vertical);
                orkCaptain.CharacterAnimator.SetFloat("Horizontal", (float)orkCaptain.RunTimeData.Horizontal);
                orkCaptain.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                if (verticalBuffer == Vertical.None && horizontalBuffer == Horizontal.None)
                {
                    actionManager.SetAction(new OrkCaptainIdle());
                }
                else
                {
                    orkCaptain.RunTimeData.Vertical = verticalBuffer;
                    orkCaptain.RunTimeData.Horizontal = horizontalBuffer;

                    orkCaptain.CharacterAnimator.SetFloat("Vertical", (float)orkCaptain.RunTimeData.Vertical);
                    orkCaptain.CharacterAnimator.SetFloat("Horizontal", (float)orkCaptain.RunTimeData.Horizontal);

                    orkCaptain.MovementBody.MovePosition(
                        orkCaptain.MovementBody.position +
                        new Vector2((float)orkCaptain.RunTimeData.Horizontal, (float)orkCaptain.RunTimeData.Vertical * 0.6f).normalized
                         * orkCaptain.Property.MoveSpeed * Time.deltaTime);
                }
            }

            public override void End()
            {
                orkCaptain.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
               actionManager.SetAction(new OrkCaptainLightAttack());

            public override void Move(Vertical direction)
            {
                verticalBuffer = direction;
            }

            public override void Move(Horizontal direction)
            {
                horizontalBuffer = direction;
            }
            #endregion
        }

        private class OrkCaptainLightAttack : IOrkCaptainAction
        {
            bool resetTimer;
            #region 動作更新
            public override void Start()
            {
                if (orkCaptain.RunTimeData.AttackTimer > 0)
                {
                    resetTimer = false;
                    orkCaptain.SetAction(new OrkCaptainIdle());
                }
                else
                {
                    resetTimer = true;
                    orkCaptain.animationEnd = false;

                    orkCaptain.LightAttackColliders.MyDamage
                        = new Wound { Damage = orkCaptain.Property.Damage, Vertigo = 0.4f };

                    orkCaptain.CharacterAnimator.SetTrigger("LightAttack");
                    orkCaptain.LightAttackSound.Play();
                }
            }

            public override void Update()
            {
                if (orkCaptain.animationEnd)
                    actionManager.SetAction(new OrkCaptainIdle());
            }

            public override void End()
            {
                if (resetTimer)
                    orkCaptain.RunTimeData.AttackTimer = orkCaptain.Property.AttackSpeed;
            }
            #endregion
        }

        private class OrkCaptainHurt : IOrkCaptainAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private Wound wound;

            public OrkCaptainHurt(Wound wound)
            {
                this.wound = wound;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = (wound.KnockBackFrom - orkCaptain.MovementBody.position).normalized;
                orkCaptain.CharacterAnimator.SetBool("IsHurt", true);
                orkCaptain.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < wound.KnockBackDistance)
                {
                    Vector2 temp = wound.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    orkCaptain.MovementBody.MovePosition(orkCaptain.MovementBody.position
                        + temp);
                }
                else
                    orkCaptain.SetAction(new OrkCaptainIdle());
            }

            public override void End()
            {
                orkCaptain.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class OrkCaptainDead : IOrkCaptainAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                orkCaptain.MovementCollider.enabled = false;
                orkCaptain.CharacterAnimator.SetBool("IsFallDown", true);
                orkCaptain.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    orkCaptain.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(orkCaptain.gameObject);
            }

            public override void End()
            {
                orkCaptain.MovementCollider.enabled = true;
                orkCaptain.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}