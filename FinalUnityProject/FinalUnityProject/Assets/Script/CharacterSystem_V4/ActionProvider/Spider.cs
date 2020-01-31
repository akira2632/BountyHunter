using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class Spider : ICharacterActionManager
    {
        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public HitEffect DefaultHitEffect;

        void Start()
        {
            nowAction = new SpiderIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (CharacterData.Health <= 0 && !(nowAction is SpiderDead))
                SetAction(new SpiderDead());

            if (CharacterData.Health > 0
                && CharacterData.VertigoConter >= 4
                && !(nowAction is SpiderFall))
                SetAction(new SpiderFall());

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
                actionManager.CharacterData.Health -= damage.Damage;
                actionManager.CharacterData.VertigoConter += damage.Vertigo;

                spider.DefaultHitEffect.PlayEffect(damage);
                if (damage.KnockBackDistance > 0)
                    actionManager.SetAction(new SpiderKnockBack(damage));
            }
        }

        private class SpiderIdle : ISpiderAction
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
                actionManager.SetAction(new SpiderLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionManager.CharacterData.Direction = direction;
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
                    actionManager.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class SpiderLightAttack : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionManager.CharacterData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(new SpiderIdle());
                    return;
                }

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
                spider.LightAttackSound.Play();
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.CharacterData.BasicAttackTimer = actionManager.CharacterData.BasicAttackSpeed;
                actionManager.SetAction(new SpiderIdle());
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
                    actionManager.MovementBody.position - damage.HitFrom).normalized;
                actionManager.CharacterAnimator.SetBool("IsHurt", true);
                spider.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    actionManager.MovementBody.MovePosition(actionManager.MovementBody.position + temp);
                }
                else
                    actionManager.SetAction(new SpiderIdle());
            }

            public override void End()
            {
                actionManager.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class SpiderFall : ISpiderAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                spider.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionManager.SetAction(new SpiderIdle());
            }

            public override void End()
            {
                actionManager.CharacterData.VertigoConter = 0;
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class SpiderDead : ISpiderAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                actionManager.MovementCollider.enabled = false;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                spider.FallDownSound.Play();
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