﻿using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class OrcCaptain : ICharacterActionManager
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

            nowAction = new OrcCaptainIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is OrcCaptainDead))
                SetAction(new OrcCaptainDead());
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

            public override void OnHit(DamageData wound)
            {
                orcCaptain.RunTimeData.Health -= wound.Damage;

                if (wound.KnockBackDistance > 0)
                    orcCaptain.SetAction(new OrcCaptainKnockBack(new DamageData
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
                IsometricUtility.GetVerticalAndHorizontal(
                    orcCaptain.RunTimeData.Direction, out var vertical, out var horizontal);
                orcCaptain.CharacterAnimator.SetFloat("Vertical", vertical);
                orcCaptain.CharacterAnimator.SetFloat("Horizontal", horizontal);

                orcCaptain.CharacterAnimator.SetBool("IsFallDown", false);
                orcCaptain.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
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
                IsometricUtility.GetVerticalAndHorizontal(
                    orcCaptain.RunTimeData.Direction, out var vertical, out var horizontal);
                orcCaptain.CharacterAnimator.SetFloat("Vertical", vertical);
                orcCaptain.CharacterAnimator.SetFloat("Horizontal", horizontal);
                orcCaptain.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    orcCaptain.RunTimeData.Direction, out var vertical, out var horizontal);
                orcCaptain.CharacterAnimator.SetFloat("Vertical", vertical);
                orcCaptain.CharacterAnimator.SetFloat("Horizontal", horizontal);

                orcCaptain.MovementBody.MovePosition(orcCaptain.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(orcCaptain.RunTimeData.Direction)
                    * orcCaptain.Property.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                orcCaptain.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
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
                if (orcCaptain.RunTimeData.BasicAttackTimer > 0)
                {
                    resetTimer = false;
                    orcCaptain.SetAction(new OrcCaptainIdle());
                }
                else
                {
                    resetTimer = true;
                    orcCaptain.animationEnd = false;

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
                    orcCaptain.RunTimeData.BasicAttackTimer = orcCaptain.Property.BasicAttackSpeed;
            }
            #endregion
        }

        private class OrcCaptainKnockBack : IOrcCaptainAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private DamageData wound;

            public OrcCaptainKnockBack(DamageData wound)
            {
                this.wound = wound;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = IsometricUtility.ToIsometricVector2(
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