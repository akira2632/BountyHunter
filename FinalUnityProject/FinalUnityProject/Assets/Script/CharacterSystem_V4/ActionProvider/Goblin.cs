﻿using UnityEngine;
using CharacterSystem.Skill;

namespace CharacterSystem
{
    public class Goblin : ICharacterActionManager
    {
        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public HitEffect DefaultHitEffect;

        void Start()
        {
            nowAction = new GoblinIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (CharacterData.Health <= 0 && !(nowAction is GoblinDead))
                SetAction(new GoblinDead());

            if (CharacterData.Health > 0
                && CharacterData.VertigoConter >= 4
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
                actionManager.CharacterData.Health -= damage.Damage;
                actionManager.CharacterData.VertigoConter += damage.Vertigo;

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
                    actionManager.CharacterData.Direction, out var vertical, out var horizontal);
                actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
                actionManager.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
                actionManager.SetAction(new GoblinBasicAttack());

            public override void SpecialAttack() =>
                actionManager.SetAction(new GoblinSpacilAttack());

            public override void SpecialAttack(Vector3 tartgetPosition)
            {
                actionManager.CharacterData.TargetPosition = tartgetPosition;
                actionManager.SetAction(new GoblinSpacilAttack(tartgetPosition));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionManager.CharacterData.Direction = direction;
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
                actionManager.CharacterData.TargetPosition = tartgetPosition;
                actionManager.SetAction(new GoblinSpacilAttack(tartgetPosition));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionManager.SetAction(new GoblinIdle());
                else
                    actionManager.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class GoblinBasicAttack : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionManager.CharacterData.BasicAttackTimer > 0)
                {
                    actionManager.SetAction(new GoblinIdle());
                    return;
                }

                actionManager.CharacterAnimator.SetTrigger("LightAttack");
                goblin.LightAttackSound.Play();
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.CharacterData.BasicAttackTimer = actionManager.CharacterData.BasicAttackSpeed;
                actionManager.SetAction(new GoblinIdle());
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

            #region 動作更新
            public override void Start()
            {
                if (actionManager.CharacterData.SpacilAttackTimer > 0)
                {
                    actionManager.SetAction(new GoblinIdle());
                    return;
                }

                if (hasTarget)
                {
                    IsometricUtility.GetVerticalAndHorizontal(
                        targetPosition - actionManager.transform.position, out var vertical, out var horizontal);
                    actionManager.CharacterAnimator.SetFloat("Vertical", vertical);
                    actionManager.CharacterAnimator.SetFloat("Horizontal", horizontal);
                }
                actionManager.CharacterAnimator.SetTrigger("RangeAttack");
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionManager.CharacterData.SpacilAttackTimer = actionManager.CharacterData.SpacilAttackSpeed;
                actionManager.SetAction(new GoblinIdle());
            }
            #endregion
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
                actionManager.CharacterAnimator.SetBool("IsHurt", true);
                goblin.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    actionManager.MovementBody.MovePosition(goblin.MovementBody.position
                        + temp);
                }
                else
                    actionManager.SetAction(new GoblinIdle());
            }

            public override void End()
            {
                actionManager.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class GoblinFall : IGoblinAction
        {
            float fallDownTimer;
            #region 動作更新
            public override void Start()
            {
                fallDownTimer = 2;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                goblin.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionManager.SetAction(new GoblinIdle());
            }

            public override void End()
            {
                actionManager.CharacterData.VertigoConter = 0;
                actionManager.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }

        private class GoblinDead : IGoblinAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 10;

                actionManager.MovementCollider.enabled = false;
                actionManager.CharacterAnimator.SetBool("IsFallDown", true);
                goblin.FallDownSound.Play();
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