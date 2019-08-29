using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4
{
    public class OrkCaptain : ICharacterActionManager
    {
        public int orkCaptainDamage;
        private CharacterRunTimeData RunTimeData;

        public Rigidbody2D MovementBody;
        public Animator CharacterAnimator;

        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public AttackColliders LightAttackColliders;

        void Start()
        {
            RunTimeData = new CharacterRunTimeData();

            nowAction = new OrkCaptainIdle();
            nowAction.SetManager(this);
        }

        private class IEnemyAction : ICharacterAction
        {
            protected OrkCaptain orkCaptain;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                orkCaptain = (OrkCaptain)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Wound damage)
            {
                orkCaptain.RunTimeData.Health -= damage.Damage;
                orkCaptain.RunTimeData.VertigoConter += damage.Vertigo;
            }
        }

        private class OrkCaptainIdle : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                orkCaptain.RunTimeData.Vertical = Vertical.None;
                orkCaptain.RunTimeData.Horizontal = Horizontal.None;
                orkCaptain.CharacterAnimator.SetBool("IsFallDown", false);
                orkCaptain.CharacterAnimator.SetBool("IsMove", false);
            }

            public override void End()
            {
                base.End();
            }
            #endregion

            #region 外部操作

            public override void LightAttack()
            {
                actionManager.SetAction(new OrkCaptainLightAttack());
            }

            public override void Move(Vertical direction)
            {
                orkCaptain.RunTimeData.Vertical = direction;
                actionManager.SetAction(new OrkCaptainMove());
            }

            public override void Move(Horizontal direction)
            {
                orkCaptain.RunTimeData.Horizontal = direction;
                actionManager.SetAction(new OrkCaptainMove());
            }

            #endregion
        }

        private class OrkCaptainMove : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                orkCaptain.MoveSound.Play();
                orkCaptain.CharacterAnimator.SetFloat("Vertical", (float)orkCaptain.RunTimeData.Vertical);
                orkCaptain.CharacterAnimator.SetFloat("Horizontal", (float)orkCaptain.RunTimeData.Horizontal);
                orkCaptain.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                if (orkCaptain.RunTimeData.Vertical == Vertical.None &&
                    orkCaptain.RunTimeData.Horizontal == Horizontal.None)
                {
                    actionManager.SetAction(new OrkCaptainIdle());
                }
                else
                {
                    orkCaptain.CharacterAnimator.SetFloat("Vertical", (float)orkCaptain.RunTimeData.Vertical);
                    orkCaptain.CharacterAnimator.SetFloat("Horizontal", (float)orkCaptain.RunTimeData.Horizontal);

                    orkCaptain.MovementBody.MovePosition(
                        orkCaptain.MovementBody.position +
                        new Vector2((float)orkCaptain.RunTimeData.Horizontal, (float)orkCaptain.RunTimeData.Vertical * 0.6f).normalized
                         * Time.deltaTime);
                }
            }

            public override void End()
            {
                orkCaptain.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack()
            {
                actionManager.SetAction(new OrkCaptainLightAttack());
            }

            public override void Move(Vertical direction)
            {
                orkCaptain.RunTimeData.Vertical = direction;
                actionManager.SetAction(new OrkCaptainMove());
            }

            public override void Move(Horizontal direction)
            {
                orkCaptain.RunTimeData.Horizontal = direction;
                actionManager.SetAction(new OrkCaptainMove());
            }
            #endregion
        }

        private class OrkCaptainLightAttack : IEnemyAction
        {
            public override void Start()
            {
                orkCaptain.animationEnd = false;

                orkCaptain.LightAttackColliders.MyDamage
                    = new Wound { Damage = orkCaptain.orkCaptainDamage, Vertigo = 1 };

                orkCaptain.CharacterAnimator.SetTrigger("LightAttack");
                orkCaptain.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (orkCaptain.animationEnd)
                    actionManager.SetAction(new OrkCaptainIdle());
            }

            public override void End()
            {
                base.End();
            }

        }

        private class OrkCaptainDead : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                orkCaptain.CharacterAnimator.SetBool("IsFallDown", true);
                orkCaptain.FallDownSound.Play();
            }
            #endregion

        }

        private class OrkCaptainHurt : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                orkCaptain.CharacterAnimator.SetBool("IsHurt", true);
                orkCaptain.HurtSound.Play();
            }

            public override void Update()
            {
                if (orkCaptain.animationEnd)
                    actionManager.SetAction(new OrkCaptainIdle());
            }

            #endregion

            #region 外部操作
            public override void OnHit(Wound damage)
            {
                base.OnHit(damage);
            }
            #endregion
        }

    }

}
