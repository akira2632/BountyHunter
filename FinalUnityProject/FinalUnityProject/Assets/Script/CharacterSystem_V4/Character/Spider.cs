using UnityEngine;
using CharacterSystem_V4.Skill;

namespace CharacterSystem_V4
{
    public class Spider : ICharacterActionManager
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

            nowAction = new SpiderIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is SpiderDead))
                SetAction(new SpiderDead());
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

            public override void SetManager(ICharacterActionManager actionManager)
            {
                spider = (Spider)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(DamageData damage)
            {
                spider.RunTimeData.Health -= damage.Damage;
                spider.RunTimeData.VertigoConter += damage.Vertigo;

                spider.DefaultHitEffect.PlayEffect(damage);
                if (damage.KnockBackDistance > 0)
                    spider.SetAction(new SpiderKnockBack(damage));
            }
        }

        private class SpiderIdle : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    spider.RunTimeData.Direction, out var vertical, out var horizontal);
                spider.CharacterAnimator.SetFloat("Vertical", vertical);
                spider.CharacterAnimator.SetFloat("Horizontal", horizontal);

                spider.CharacterAnimator.SetBool("IsFallDown", false);
                spider.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(new SpiderLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    spider.RunTimeData.Direction = direction;
                    actionManager.SetAction(new SpiderMove());
                }
            }
            #endregion
        }

        private class SpiderMove : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                spider.MoveSound.Play();
                IsometricUtility.GetVerticalAndHorizontal(
                    spider.RunTimeData.Direction, out var vertical, out var horizontal);
                spider.CharacterAnimator.SetFloat("Vertical", vertical);
                spider.CharacterAnimator.SetFloat("Horizontal", horizontal);
                spider.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    spider.RunTimeData.Direction, out var vertical, out var horizontal);
                spider.CharacterAnimator.SetFloat("Vertical", vertical);
                spider.CharacterAnimator.SetFloat("Horizontal", horizontal);

                spider.MovementBody.MovePosition(spider.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(spider.RunTimeData.Direction)
                    * spider.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                spider.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
               actionManager.SetAction(new SpiderLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(new SpiderIdle());
                else
                    spider.RunTimeData.Direction = direction;
            }
            #endregion
        }

        private class SpiderLightAttack : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                if (spider.RunTimeData.BasicAttackTimer > 0)
                {
                    spider.SetAction(new SpiderIdle());
                    return;
                }

                spider.animationEnd = false;

                spider.CharacterAnimator.SetTrigger("LightAttack");
                spider.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (spider.animationEnd)
                {
                    spider.RunTimeData.BasicAttackTimer = spider.Property.BasicAttackSpeed;
                    actionManager.SetAction(new SpiderIdle());
                }
            }
            #endregion            
        }

        private class SpiderKnockBack : ISpiderAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private DamageData damage;

            public SpiderKnockBack(DamageData damage)
            {
                this.damage = damage;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = IsometricUtility.ToIsometricVector2(
                    spider.MovementBody.position - damage.HitFrom).normalized;
                spider.CharacterAnimator.SetBool("IsHurt", true);
                spider.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
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