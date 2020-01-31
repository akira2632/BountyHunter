using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class Orc : ICharacterActionManager
    {
        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public HitEffect DefalutHitEffect;

        protected override ICharacterAction IdelAction => throw new System.NotImplementedException();

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

            if (RunTimeData.Health > 0
                && RunTimeData.VertigoConter >= 4
                && !(nowAction is OrcFall))
                SetAction(new OrcFall());

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
                actionManager.RunTimeData.Health -= damage.Damage;
                actionManager.RunTimeData.VertigoConter += damage.Vertigo;

                orc.DefalutHitEffect.PlayEffect(damage);
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
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
                actionManager.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(new OrcLightAttack());

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionManager.RunTimeData.Direction = direction;
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
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
                actionManager.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionManager.RunTimeData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionManager.MovementBody.MovePosition(actionManager.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionManager.RunTimeData.Direction)
                    * actionManager.Property.MoveSpeed * Time.deltaTime);
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
                    actionManager.RunTimeData.Direction = direction;
            }
            #endregion
        }

        private class OrcLightAttack : IOrcAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionManager.RunTimeData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(new OrcIdle());
                    return;
                }

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
                orc.LightAttackSound.Play();
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.RunTimeData.BasicAttackTimer = orc.Property.BasicAttackSpeed;
                actionManager.SetAction(new OrcIdle());
            }
            #endregion
        }

        private class OrcFall : IOrcAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                orc.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionManager.SetAction(new OrcIdle());
            }

            public override void End()
            {
                actionManager.RunTimeData.VertigoConter = 0;
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
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
                    actionManager.MovementBody.position - damage.HitFrom).normalized;
                actionManager.CharacterAnimator.SetBool("IsHurt", true);
                orc.HurtSound.Play();
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
                    actionManager.SetAction(new OrcIdle());
            }

            public override void End()
            {
                actionManager.CharacterAnimator.SetBool("IsHurt", false);
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

                actionManager.MovementCollider.enabled = false;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                orc.FallDownSound.Play();
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