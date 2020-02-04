using System;
using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    [CreateAssetMenu(fileName = "歐克隊長動作提供者", menuName = "賞金獵人/動作提供者/歐克隊長動作提供者")]
    public class OrcCaptainActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, HurtSound, BasicAttackSound, SpecailAttackSound;
        public Skill.HitEffect DefaultHitEffect;

        #region FactoryMethod
        public override ICharacterAction GetIdelAction(CharacterActionController controller)
        {
            return new OrcCaptainIdel()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController controller)
        {
            return new OrcCaptainFall()
            {　
                actionController = controller,
                actionProvider = this
            };
        }

        public override ICharacterAction GetDeadAction(CharacterActionController controller)
        {
            return new OrcCaptainDead()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetMoveAction(CharacterActionController controller)
        {
            return new OrcCaptainMove()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetBasicAttackAction(CharacterActionController controller)
        {
            return new OrcCaptainBasicAttack()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpacilAttackStartAction(CharacterActionController controller)
        {
            Debug.Log("SpacilAttackNotImplement");
            return GetIdelAction(controller);
        }
        #endregion

        #region OrcCaptainAction
        private class IOrcCaptainAction : ICharacterAction
        {
            public OrcCaptainActionProvider actionProvider;
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
            }
        }

        /// <summary>
        /// 歐克隊長待機
        /// </summary>
        private class OrcCaptainIdel : IOrcCaptainAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.CharacterAnimator.SetBool("IsMove", false);

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);
            }
            #endregion

            #region 外部操作
            public override void BasicAttack() =>
               actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));

            public override void SpecialAttack() =>
               actionController.SetAction(actionProvider.GetSpacilAttackStartAction(actionController));

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

        /// <summary>
        /// 歐克隊長移動
        /// </summary>
        private class OrcCaptainMove : IOrcCaptainAction
        {
            #region 動作更新
            public override void Start()
            {
                actionController.AudioSource.clip = actionProvider.MoveSound;
                actionController.AudioSource.loop = true;
                actionController.AudioSource.Play();

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);
                actionController.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.CharacterAnimator.SetFloat("Vertical", vertical);
                actionController.CharacterAnimator.SetFloat("Horizontal", horizontal);

                actionController.MovementBody.MovePosition(actionController.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                    * actionController.CharacterData.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                actionController.AudioSource.Stop();
                actionController.AudioSource.loop = false;
            }
            #endregion

            #region 外部操作
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                else
                    actionController.CharacterData.Direction = direction;
            }

            public override void SpecialAttack() =>
                actionController.SetAction(actionProvider.GetSpacilAttackStartAction(actionController));

            public override void BasicAttack() =>
                actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));
            #endregion
        }

        /// <summary>
        /// 歐克隊長基本攻擊
        /// </summary>
        private class OrcCaptainBasicAttack : IOrcCaptainAction
        {
            #region 動作更新
            public override void Start()
            {
                if (actionController.CharacterData.BasicAttackTimer > 0)
                {
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                    return;
                }

                actionController.AudioSource.clip = actionProvider.BasicAttackSound;
                actionController.AudioSource.Play();

                actionController.CharacterAnimator.SetTrigger("BasicAttack");
            }
            #endregion

            #region 外部操作
            public override void OnAnimationEnd()
            {
                actionController.CharacterData.BasicAttackTimer = actionController.CharacterData.BasicAttackSpeed;
                actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
            #endregion
        }

        private class OrcCaptainFall : IOrcCaptainAction
        {
            float fallDownTimer;
            #region 動作更新
            public override void Start()
            {
                fallDownTimer = 2;
                actionController.CharacterData.VertigoConter = 0;

                actionController.AudioSource.clip = actionProvider.HurtSound;
                actionController.AudioSource.Play();

                actionController.CharacterAnimator.SetBool("IsFallDown", true);
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }

            public override void End()
            {
                actionController.CharacterAnimator.SetBool("IsFallDown", false);
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

                actionController.AudioSource.clip = actionProvider.HurtSound;
                actionController.AudioSource.Play();

                actionController.MovementCollider.enabled = false;
                actionController.CharacterAnimator.SetBool("IsFallDown", true);
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
                actionController.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}
