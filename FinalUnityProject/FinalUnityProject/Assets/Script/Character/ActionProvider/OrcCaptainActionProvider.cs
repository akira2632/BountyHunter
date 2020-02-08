using UnityEngine;

namespace CharacterSystem.ActionProvider
{
    [CreateAssetMenu(fileName = "歐克隊長動作提供者", menuName = "賞金獵人/動作提供者/歐克隊長動作提供者")]
    public class OrcCaptainActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound, HurtSound, DeffendSound, BasicAttackSound, SpecailAttackSound;
        public Skill.HitEffect DefaultHitEffect, DefaultDeffendEffect;

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

        private ICharacterAction GetSpecialAttackStartAction(CharacterActionController controller)
        {
            return new OrcCaptainSpecialAttackStart()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttackAction(CharacterActionController controller)
        {
            return new OrcCaptainSpecialAttack()
            {
                actionController = controller,
                actionProvider = this
            };
        }

        private ICharacterAction GetSpecialAttackStiffAction(CharacterActionController controller)
        {
            return new OrcCaptainSpecialAttackStiff()
            {
                actionController = controller,
                actionProvider = this
            };
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
                actionController.Animator.SetBool("IsMove", false);

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);
            }
            #endregion

            #region 外部事件
            public override void BasicAttack() =>
               actionController.SetAction(actionProvider.GetBasicAttackAction(actionController));

            public override void SpecialAttack() =>
               actionController.SetAction(actionProvider.GetSpecialAttackStartAction(actionController));

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
                actionController.AudioSource.Play();

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);
                actionController.Animator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);

                actionController.MovementBody.MovePosition(actionController.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                    * actionController.CharacterData.MoveSpeed * Time.deltaTime);
            }

            public override void End()
            {
                actionController.AudioSource.Stop();

                actionController.Animator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部事件
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
                else
                    actionController.CharacterData.Direction = direction;
            }

            public override void SpecialAttack() =>
                actionController.SetAction(actionProvider.GetSpecialAttackStartAction(actionController));

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

        /// <summary>
        /// 歐克隊長特殊攻擊預備
        /// </summary>
        private class OrcCaptainSpecialAttackStart : IOrcCaptainAction
        {
            private float timer;
            private bool timerLock;

            #region 動作更新
            public override void Start()
            {
                timer = 1;
                timerLock = true;
                actionController.Animator.SetTrigger("SpecialAttackStart");
            }

            public override void Update()
            {
                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);

                if (!timerLock)
                    timer -= Time.deltaTime;

                if (timer <= 0)
                    actionController.SetAction(actionProvider.GetSpecialAttackAction(actionController));
            }
            #endregion

            #region 外部事件
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    actionController.CharacterData.Direction = direction;
            }

            public override void OnAnimationEnd() =>
                timerLock = false;

            public override void Hit(DamageData damage)
            {
                damage.Damage = 0;

                actionController.AudioSource.PlayOneShot(actionProvider.DeffendSound);

                actionProvider.DefaultDeffendEffect.PlayEffect(damage);
            }
            #endregion
        }
        
        /// <summary>
        /// 歐克隊長特殊攻擊
        /// </summary>
        private class OrcCaptainSpecialAttack : IOrcCaptainAction
        {
            private float timer;

            #region 動作更新
            public override void Start()
            {
                timer = 7;

                actionController.Animator.SetBool("SpecialAttack", true);
            }

            public override void Update()
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                    actionController.SetAction(actionProvider.GetSpecialAttackStiffAction(actionController));

                IsometricUtility.GetVerticalAndHorizontal(
                    actionController.CharacterData.Direction, out var vertical, out var horizontal);
                actionController.Animator.SetFloat("Vertical", vertical);
                actionController.Animator.SetFloat("Horizontal", horizontal);

                actionController.MovementBody.MovePosition(actionController.MovementBody.position +
                    IsometricUtility.ToIsometricVector2(actionController.CharacterData.Direction)
                    * actionController.CharacterData.MoveSpeed * 2 * Time.deltaTime);
            }

            public override void End()
            {
                actionController.AudioSource.Stop();

                actionController.Animator.SetBool("SpecialAttack", false);
            }
            #endregion

            #region 外部事件
            public override void Move(Vector2 direction)
            {
                if (direction.magnitude > 0)
                    actionController.CharacterData.Direction = direction;
            }

            public override void OnAnimationStart()
            {
                actionController.AudioSource.PlayOneShot(actionProvider.SpecailAttackSound);
            }

            public override void Hit(DamageData damage)
            {
                damage.Damage = 0;

                actionController.AudioSource.PlayOneShot(actionProvider.DeffendSound);

                actionProvider.DefaultDeffendEffect.PlayEffect(damage);
            }
            #endregion
        }

        /// <summary>
        /// 歐克隊長特殊攻擊硬直
        /// </summary>
        private class OrcCaptainSpecialAttackStiff : IOrcCaptainAction
        {
            private float stiffTimer;

            #region 動作更新
            public override void Start()
            {
                stiffTimer = 3;
            }

            public override void Update()
            {
                stiffTimer -= Time.deltaTime;

                if (stiffTimer <= 0)
                    actionController.SetAction(actionProvider.GetIdelAction(actionController));
            }
            #endregion
        }

        /// <summary>
        /// 歐克隊長倒地
        /// </summary>
        private class OrcCaptainFall : IOrcCaptainAction
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

        /// <summary>
        /// 歐克隊長死亡
        /// </summary>
        private class OrcCaptainDead : IOrcCaptainAction
        {
            float desdroyedTimer;

            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                actionController.AudioSource.PlayOneShot(actionProvider.HurtSound);

                actionController.MovementCollider.enabled = false;
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
        }
        #endregion
    }
}
