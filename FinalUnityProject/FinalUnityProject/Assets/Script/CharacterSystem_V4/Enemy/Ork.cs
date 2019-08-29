using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4
{
    public class Ork : ICharacterActionManager
    {
        public int orkDamage;
        private CharacterRunTimeData RunTimeData;

        public Rigidbody2D MovementBody;
        public Animator CharacterAnimator;

        public AudioSource MoveSound, FallDownSound, LightAttackSound, HurtSound;
        public AttackColliders LightAttackColliders;

        void Start()
        {
            RunTimeData = new CharacterRunTimeData();

            nowAction = new OrkIdle();
            nowAction.SetManager(this);
        }

        private class IEnemyAction : ICharacterAction
        {
            protected Ork ork;

            public override void SetManager(ICharacterActionManager actionManager)
            {
                ork = (Ork)actionManager;
                base.SetManager(actionManager);
            }

            public override void OnHit(Wound damage)
            {
                ork.RunTimeData.Health -= damage.Damage;
                ork.RunTimeData.VertigoConter += damage.Vertigo;
            }
        }

        private class OrkIdle : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                ork.RunTimeData.Vertical = Vertical.None;
                ork.RunTimeData.Horizontal = Horizontal.None;
                ork.CharacterAnimator.SetBool("IsFallDown", false);
                ork.CharacterAnimator.SetBool("IsMove", false);
            }

            public override void End()
            {
                base.End();
            }
            #endregion

            #region 外部操作

            public override void LightAttack()
            {
                actionManager.SetAction(new OrkLightAttack());
            }

            public override void Move(Vertical direction)
            {
                ork.RunTimeData.Vertical = direction;
                actionManager.SetAction(new OrkMove());
            }

            public override void Move(Horizontal direction)
            {
                ork.RunTimeData.Horizontal = direction;
                actionManager.SetAction(new OrkMove());
            }

            #endregion
        }

        private class OrkMove : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                ork.MoveSound.Play();
                ork.CharacterAnimator.SetFloat("Vertical", (float)ork.RunTimeData.Vertical);
                ork.CharacterAnimator.SetFloat("Horizontal", (float)ork.RunTimeData.Horizontal);
                ork.CharacterAnimator.SetBool("IsMove", true);
            }

            public override void Update()
            {
                if (ork.RunTimeData.Vertical == Vertical.None &&
                    ork.RunTimeData.Horizontal == Horizontal.None)
                {
                    actionManager.SetAction(new OrkIdle());
                }
                else
                {
                    ork.CharacterAnimator.SetFloat("Vertical", (float)ork.RunTimeData.Vertical);
                    ork.CharacterAnimator.SetFloat("Horizontal", (float)ork.RunTimeData.Horizontal);

                    ork.MovementBody.MovePosition(
                        ork.MovementBody.position +
                        new Vector2((float)ork.RunTimeData.Horizontal, (float)ork.RunTimeData.Vertical * 0.6f).normalized
                         * Time.deltaTime);
                }
            }

            public override void End()
            {
                ork.MoveSound.Stop();
            }
            #endregion

            #region 外部操作
            public override void LightAttack()
            {
                actionManager.SetAction(new OrkLightAttack());
            }

            public override void Move(Vertical direction)
            {
                ork.RunTimeData.Vertical = direction;
                actionManager.SetAction(new OrkMove());
            }

            public override void Move(Horizontal direction)
            {
                ork.RunTimeData.Horizontal = direction;
                actionManager.SetAction(new OrkMove());
            }
            #endregion
        }

        private class OrkLightAttack : IEnemyAction
        {
            public override void Start()
            {
                ork.animationEnd = false;

                ork.LightAttackColliders.MyDamage
                    = new Wound { Damage = ork.orkDamage, Vertigo = 1 };

                ork.CharacterAnimator.SetTrigger("LightAttack");
                ork.LightAttackSound.Play();
            }

            public override void Update()
            {
                if (ork.animationEnd)
                    actionManager.SetAction(new OrkIdle());
            }

            public override void End()
            {
                base.End();
            }

        }

        private class OrkDead : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                ork.CharacterAnimator.SetBool("IsFallDown", true);
                ork.FallDownSound.Play();
            }
            #endregion

        }

        private class OrkHurt : IEnemyAction
        {
            #region 動作更新
            public override void Start()
            {
                ork.CharacterAnimator.SetTrigger("IsHurt");
                ork.HurtSound.Play();
            }

            public override void Update()
            {
                if (ork.animationEnd)
                    actionManager.SetAction(new OrkIdle());
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
