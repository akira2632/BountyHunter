﻿using UnityEngine;
using Character.Skill;

namespace Character.ActionProvider
{
    [CreateAssetMenu(fileName = "蜘蛛動作提供者", menuName = "賞金獵人/動作提供者/蜘蛛動作提供者")]
    public class SpiderActionProvidedr : ICharacterActionProvider
    {
        public AudioClip MoveSound, FallDownSound, BasicAttackSound, HurtSound;
        public HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController comtroller)
        {
            return new SpiderIdle()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetDeadAction(CharacterActionController comtroller)
        {
            return new SpiderDead()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController comtroller)
        {
            return new SpiderFall()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetMoveAction(CharacterActionController comtroller)
        {
            return  new SpiderMove()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetBasicAttackAction(CharacterActionController comtroller)
        {
            return new SpiderBasicAttack()
            {
                actionController = comtroller,
                actionProvider = this
            };
        }

        private ICharacterAction GetKnockBackAction(CharacterActionController comtroller, DamageData damage)
        {
            return new SpiderKnockBack(damage)
            {
                actionController = comtroller,
                actionProvider = this
            };
        }
        #endregion

        #region SpiderActions
        private class ISpiderAction : ICharacterAction
        {
            public SpiderActionProvidedr actionProvider;
            public CharacterActionController actionController;

            public virtual void Start() { }
            public virtual void Update() { }
            public virtual void End() { }

            public virtual void OnAnimationStart() { }
            public virtual void OnAnimationEnd() { }

            public virtual void Move(Vector2 direction) { }
            public virtual void Dodge() { }
            public virtual void Deffend(bool deffend) { }

            public virtual void BasicAttack() { }
            public virtual void SpecialAttack() { }
            public virtual void SpecialAttack(bool hold) { }
            public virtual void SpecialAttack(Vector3 tartgetPosition) { }

            public virtual void Hit(DamageData damage)
            {
                actionController.CharacterData.Health -= damage.Damage;
                actionController.CharacterData.VertigoConter += damage.Vertigo;

                actionProvider.DefaultHitEffect.PlayEffect(damage);
                if (actionController.CharacterData.Health > 0
                    && damage.KnockBackDistance > 0)
                    actionController.SetAction(actionProvider.GetKnockBackAction(actionController, damage));
            }
        }

        private class SpiderIdle : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.CharacterData.Direction.
                    GetVerticalAndHorizontal(out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);

                actionController.Animator.SetBool("IsFallDown", false);
                actionController.Animator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部事件
            public override void BasicAttack()
            {
                if(actionController.CharacterData.BasicAttackTimer <= 0)
                    actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                {
                    actionController.CharacterData.Direction = direction;
                    actionController.SetAction(actionProvider.GetMoveAction(actionController));
                }
            }
            #endregion
        }

        private class SpiderMove : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.AudioSource.clip = actionProvider.MoveSound;
                actionController.AudioSource.Play();

                actionController.CharacterData.Direction.
                    GetVerticalAndHorizontal(out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);
                actionController.Animator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                actionController.CharacterData.Direction.
                    GetVerticalAndHorizontal(out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);

                actionController.MovementBody.MovePosition(
                    actionController.MovementBody.position
                    + actionController.CharacterData.Direction.IsoNormalized()
                    * actionController.CharacterData.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                actionController.AudioSource.Stop();

                actionController.Animator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部事件
            public override void BasicAttack()
            {
                if (actionController.CharacterData.BasicAttackTimer <= 0)
                    actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));
            }

            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                else
                    actionController.CharacterData.Direction = direction;
            }
            #endregion
        }

        private class SpiderBasicAttack : ISpiderAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionController.CharacterData.BasicAttackTimer > 0)
                {
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                    return;
                }

                actionController.AudioSource.PlayOneShot(actionProvider.BasicAttackSound);

                actionController.Animator.SetTrigger("BasicAttack");
            }
            #endregion

            #region 外部事件
            public override void OnAnimationEnd()
            {
                actionController.CharacterData.BasicAttackTimer = actionController.CharacterData.BasicAttackSpeed;
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
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
                knockBackDirection = 
                    (actionController.MovementBody.position - damage.HitFrom).IsoNormalized();

                actionController.AudioSource.PlayOneShot(actionProvider.HurtSound);

                actionController.Animator.SetBool("IsHurt", true);
            }

            public override void Update()
            {
                if (nowDistance < damage.KnockBackDistance)
                {
                    Vector2 temp = damage.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    actionController.MovementBody.MovePosition(actionController.MovementBody.position + temp);
                }
                else
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void End()
            {
                actionController.Animator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class SpiderFall : ISpiderAction
        {
            float fallDownTimer;

            #region 動作更新
            public override void Start()
            {
                fallDownTimer = 2;
                actionController.CharacterData.VertigoConter = 0;

                actionController.AudioSource.PlayOneShot(actionProvider.HurtSound);

                actionController.Animator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void End()
            {
                actionController.Animator.SetBool("IsFallDown", false);
            }
            #endregion
        }

        private class SpiderDead : ISpiderAction
        {
            float desdroyedTimer;

            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                actionController.MovementCollider.enabled = false;

                actionController.AudioSource.PlayOneShot(actionProvider.FallDownSound);

                actionController.Animator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    actionController.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(actionController.gameObject);
            }

            public override void End()
            {
                actionController.MovementCollider.enabled = true;
                actionController.Animator.SetBool("IsFallDown", false);
            }
            #endregion
            public override void Hit(DamageData damage)
            {
                actionProvider.DefaultHitEffect.PlayEffect(damage, false);
            }
        }
        #endregion
    }
}