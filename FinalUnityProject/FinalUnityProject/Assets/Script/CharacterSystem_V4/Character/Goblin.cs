﻿using UnityEngine;

namespace CharacterSystem_V4
{
    public class Goblin : ICharacterActionManager
    {
        public CharacterProperty Property;
        public CharacterRunTimeData RunTimeData;

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

            nowAction = new GoblinIdle();
            nowAction.SetManager(this);
        }

        public override void ActionUpdate()
        {
            if (RunTimeData.Health <= 0 && !(nowAction is GoblinDead))
                SetAction(new GoblinDead());
            else if (RunTimeData.Health > 0)
            {
                RunTimeData.AttackTimer += Time.deltaTime;

                RunTimeData.RegenTimer += Time.deltaTime;
                if (RunTimeData.Health < Property.MaxHealth &&
                    RunTimeData.RegenTimer >= Property.CharacterRegenSpeed)
                {
                    RunTimeData.Health += Property.CharacterRegenHealth;
                    RunTimeData.RegenTimer = 0;
                }

                if (RunTimeData.VertigoConter >= 4 && !(nowAction is GoblinFall))
                    SetAction(new GoblinFall());

                RunTimeData.VertigoConter -= Time.deltaTime / 10;
            }

            base.ActionUpdate();
        }

        #region GoblinActions
        private class IGoblinAction : ICharacterAction
        {
            protected Goblin goblin;
            protected Vertical verticalBuffer;
            protected Horizontal horizontalBuffer;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                goblin = (Goblin)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Wound wound)
            {
                goblin.RunTimeData.Health -= wound.Damage;
                goblin.RunTimeData.VertigoConter += wound.Vertigo;

                if (wound.KnockBackDistance > 0)
                    goblin.SetAction(new GoblinHurt(wound));
            }
        }

        private class GoblinIdle : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                goblin.CharacterAnimator.SetFloat("Vertical", (float)goblin.RunTimeData.Vertical);
                goblin.CharacterAnimator.SetFloat("Horizontal", (float)goblin.RunTimeData.Horizontal);

                goblin.CharacterAnimator.SetBool("IsFallDown", false);
                goblin.CharacterAnimator.SetBool("IsMove", false);
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
                actionManager.SetAction(new GoblinLightAttack());

            public override void Move(Vertical direction) =>
                actionManager.SetAction(new GoblinMove(direction, Horizontal.None));

            public override void Move(Horizontal direction) =>
                actionManager.SetAction(new GoblinMove(Vertical.None, direction));
            #endregion
        }

        private class GoblinMove : IGoblinAction
        {
            public GoblinMove(Vertical vertical, Horizontal horizontal)
            {
                verticalBuffer = vertical;
                horizontalBuffer = horizontal;
            }

            #region 動作更新
            public override void Start()
            {
                goblin.MoveSound.Play();
                goblin.CharacterAnimator.SetFloat("Vertical", (float)goblin.RunTimeData.Vertical);
                goblin.CharacterAnimator.SetFloat("Horizontal", (float)goblin.RunTimeData.Horizontal);
                goblin.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                if (verticalBuffer == Vertical.None && horizontalBuffer == Horizontal.None)
                {
                    actionManager.SetAction(new GoblinIdle());
                }
                else
                {
                    goblin.RunTimeData.Vertical = verticalBuffer;
                    goblin.RunTimeData.Horizontal = horizontalBuffer;

                    goblin.CharacterAnimator.SetFloat("Vertical", (float)goblin.RunTimeData.Vertical);
                    goblin.CharacterAnimator.SetFloat("Horizontal", (float)goblin.RunTimeData.Horizontal);

                    goblin.MovementBody.MovePosition(
                        goblin.MovementBody.position +
                        new Vector2((float)goblin.RunTimeData.Horizontal, (float)goblin.RunTimeData.Vertical * 0.6f).normalized
                         * goblin.Property.MoveSpeed * Time.deltaTime);
                }
            }

            public override void End()
            {
                goblin.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack() =>
               actionManager.SetAction(new GoblinLightAttack());

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

        private class GoblinLightAttack : IGoblinAction
        {
            #region 動作更新
            public override void Start()
            {
                goblin.animationEnd = false;

                goblin.LightAttackColliders.MyDamage
                    = new Wound { Damage = goblin.Property.Attack, Vertigo = 0.4f };

                goblin.CharacterAnimator.SetTrigger("LightAttack");
                goblin.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (goblin.animationEnd)
                    actionManager.SetAction(new GoblinIdle());
            }
            #endregion
        }

        private class GoblinHurt : IGoblinAction
        {
            float nowDistance;
            Vector2 knockBackDirection;
            private Wound wound;

            public GoblinHurt(Wound wound)
            {
                this.wound = wound;
            }

            #region 動作更新
            public override void Start()
            {
                nowDistance = 0;
                knockBackDirection = (wound.KnockBackFrom - goblin.MovementBody.position).normalized;
                goblin.CharacterAnimator.SetBool("IsHurt", true);
                goblin.HurtSound.Play();
            }

            public override void Update()
            {
                if (nowDistance < wound.KnockBackDistance)
                {
                    Vector2 temp = wound.KnockBackSpeed * knockBackDirection * Time.deltaTime;
                    nowDistance += temp.magnitude;

                    goblin.MovementBody.MovePosition(goblin.MovementBody.position
                        + temp);
                }
                else
                    goblin.SetAction(new GoblinIdle());
            }

            public override void End()
            {
                goblin.CharacterAnimator.SetBool("IsHurt", false);
            }
            #endregion
        }

        private class GoblinFall : IGoblinAction
        {
            float fallDownTimer;

            public override void Start()
            {
                fallDownTimer = 2;
                goblin.CharacterAnimator.SetBool("IsFallDown", true);
                goblin.HurtSound.Play();
            }

            public override void Update()
            {
                fallDownTimer -= Time.deltaTime;
                if (fallDownTimer <= 0)
                    goblin.SetAction(new GoblinIdle());
            }

            public override void End()
            {
                goblin.CharacterAnimator.SetBool("IsFallDown", false);
            }
        }

        private class GoblinDead : IGoblinAction
        {
            float desdroyedTimer;
            #region 動作更新
            public override void Start()
            {
                desdroyedTimer = 120;

                goblin.MovementCollider.enabled = false;
                goblin.CharacterAnimator.SetBool("IsFallDown", true);
                goblin.FallDownSound.Play();
            }

            public override void Update()
            {
                desdroyedTimer -= Time.deltaTime;
                if (desdroyedTimer < 3)
                    goblin.SpriteRenderer.color = new Color(1, 1, 1, desdroyedTimer / 3);

                if (desdroyedTimer <= 0)
                    Destroy(goblin.gameObject);
            }

            public override void End()
            {
                goblin.MovementCollider.enabled = true;
                goblin.CharacterAnimator.SetBool("IsFallDown", false);
            }
            #endregion
        }
        #endregion
    }
}