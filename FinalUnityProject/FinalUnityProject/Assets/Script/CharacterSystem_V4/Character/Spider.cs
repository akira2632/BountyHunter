using UnityEngine;

namespace CharacterSystem_V4
{
    public class Spider : ICharacterActionManager
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

            nowAction = new SpiderIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is SpiderDead))
                SetAction(new SpiderDead());
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

                if (RunTimeData.VertigoConter >= 4 && !(nowAction is SpiderFall))
                    SetAction(new SpiderFall());

                RunTimeData.VertigoConter -= Time.deltaTime / 10;
            }

            base.ActionUpdate();
        }

        #region SpiderActions
        private class ISpiderAction : ICharacterAction
        {
            protected Spider spider;
            protected Vertical verticalBuffer;
            protected Horizontal horizontalBuffer;
            
            public override void SetManager(ICharacterActionManager actionManager)
            {
                spider = (Spider)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Wound wound)
            {
                spider.RunTimeData.Health -= wound.Damage;
                spider.RunTimeData.VertigoConter += wound.Vertigo;

                if (wound.KnockBackDistance > 0)
                    spider.SetAction(new SpiderHurt(wound));
            }
        }

        private class SpiderIdle : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                spider.CharacterAnimator.SetFloat("Vertical", (float)spider.RunTimeData.Vertical);
                spider.CharacterAnimator.SetFloat("Horizontal", (float)spider.RunTimeData.Horizontal);

                spider.CharacterAnimator.SetBool("IsFallDown", false);
                spider.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
                actionManager.SetAction(new SpiderLightAttack());

            public override void Move(Vertical direction) =>
                actionManager.SetAction(new SpiderMove(direction, Horizontal.None));

            public override void Move(Horizontal direction) =>
                actionManager.SetAction(new SpiderMove(Vertical.None, direction));
            #endregion
        }

        private class SpiderMove : ISpiderAction
        {
            public SpiderMove(Vertical vertical, Horizontal horizontal)
            {
                verticalBuffer = vertical;
                horizontalBuffer = horizontal;
            }

            #region 動作更新
            public override void Start()
            {
                spider.MoveSound.Play();
                spider.CharacterAnimator.SetFloat("Vertical", (float)spider.RunTimeData.Vertical);
                spider.CharacterAnimator.SetFloat("Horizontal", (float)spider.RunTimeData.Horizontal);
                spider.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                if (verticalBuffer == Vertical.None && horizontalBuffer == Horizontal.None)
                {
                    actionManager.SetAction(new SpiderIdle());
                }
                else
                {
                    spider.RunTimeData.Vertical = verticalBuffer;
                    spider.RunTimeData.Horizontal = horizontalBuffer;

                    spider.CharacterAnimator.SetFloat("Vertical", (float)spider.RunTimeData.Vertical);
                    spider.CharacterAnimator.SetFloat("Horizontal", (float)spider.RunTimeData.Horizontal);

                    spider.MovementBody.MovePosition(
                        spider.MovementBody.position +
                        new Vector2((float)spider.RunTimeData.Horizontal, (float)spider.RunTimeData.Vertical * 0.6f).normalized
                         * spider.Property.MoveSpeed * Time.deltaTime);
                }
            }

            public override void End()
            {
                spider.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
               actionManager.SetAction(new SpiderLightAttack());

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

        private class SpiderLightAttack : ISpiderAction
        {
            bool resetTimer;
            #region 動作更新
            public override void Start()
            {
                if (spider.RunTimeData.AttackTimer > 0)
                {
                    resetTimer = false;
                    spider.SetAction(new SpiderIdle());
                }
                else
                {
                    resetTimer = true;
                    spider.animationEnd = false;

                    spider.LightAttackColliders.MyDamage
                        = new Wound { Damage = spider.Property.Damage, Vertigo = 0.4f };

                    spider.CharacterAnimator.SetTrigger("LightAttack");
                    spider.LightAttackSound.Play();
                }

            }

            public override void Update()
            {
                if (spider.animationEnd)
                    actionManager.SetAction(new SpiderIdle());
            }

            public override void End()
            {
                if (resetTimer)
                    spider.RunTimeData.AttackTimer = spider.Property.AttackSpeed;
            }
            #endregion            
        }

        private class SpiderHurt : ISpiderAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private Wound wound;

            public SpiderHurt(Wound wound)
            {
                this.wound = wound;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = (wound.KnockBackFrom - spider.MovementBody.position).normalized;
                spider.CharacterAnimator.SetBool("IsHurt", true);
                spider.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < wound.KnockBackDistance)
                {
                    Vector2 temp = wound.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    spider.MovementBody.MovePosition(spider.MovementBody.position
                        + temp);
                }
                else
                    spider.SetAction(new SpiderIdle());
            }

            public override void End()
            {
                spider.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class SpiderFall : ISpiderAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                spider.CharacterAnimator.SetBool("IsFallDown", true);
                spider.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    spider.SetAction(new SpiderIdle());
            }

            public override void End()
            {
                spider.RunTimeData.VertigoConter = 0;
                spider.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class SpiderDead : ISpiderAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                spider.MovementCollider.enabled = false;
                spider.CharacterAnimator.SetBool("IsFallDown", true);
                spider.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    spider.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(spider.gameObject);
            }

            public override void End()
            {
                spider.MovementCollider.enabled = true;
                spider.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}