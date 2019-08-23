using UnityEngine;
using CharacterSystem_V2.InterfaceAndData;

namespace CharacterSystem_V2.Action
{
    /// <summary>
    /// 玩家待機
    /// </summary>
    public class PlayerIdel : ICharacterAction
    {
        #region 動作更新
        public override void Inital()
        {
            myCharacter.CharacterAnimator.SetBool("IsFallDown", false);
            myCharacter.CharacterAnimator.SetBool("IsMove", false);
        }
        #endregion

        #region 外部操作
        public override void Deffend(bool deffend)
        {
            if (deffend)
                myCharacter.SetAction(new PlayerDeffend());
        }

        public override void HeavyAttack() =>
            myCharacter.SetAction(new PlayerHeavyAttackPrepar());

        public override void LightAttack() =>
            myCharacter.SetAction(new PlayerLightAttack());

        public override void Move(Vertical direction)
        {
            myCharacter.Vertiacl = direction;
            myCharacter.SetAction(new PlayerMove());
        }

        public override void Move(Horizontal direction)
        {
            myCharacter.Horizontal = direction;
            myCharacter.SetAction(new PlayerMove());
        }
        #endregion
    }

    /// <summary>
    /// 玩家移動
    /// </summary>
    public class PlayerMove : ICharacterAction
    {
        bool DirectionChange = false;
        Vertical vertical;
        Horizontal horizontal;

        #region 動作更新
        public override void Inital()
        {
            vertical = myCharacter.Vertiacl;
            horizontal = myCharacter.Horizontal;

            myCharacter.MoveSound.Play();
            myCharacter.CharacterAnimator.SetFloat("Vertical", (float)myCharacter.Vertiacl);
            myCharacter.CharacterAnimator.SetFloat("Horizontal", (float)myCharacter.Horizontal);
            myCharacter.CharacterAnimator.SetBool("IsMove", true);
        }

        public override void Update()
        {
            if (vertical == Vertical.None && horizontal == Horizontal.None)
                myCharacter.SetAction(new PlayerIdel());
            else
            {
                myCharacter.Vertiacl = vertical;
                myCharacter.Horizontal = horizontal;
            }

            MoveCharacter();

            if (DirectionChange)
            {
                myCharacter.CharacterAnimator.SetFloat("Vertical", (float)myCharacter.Vertiacl);
                myCharacter.CharacterAnimator.SetFloat("Horizontal", (float)myCharacter.Horizontal);
                DirectionChange = false;
            }
        }

        private void MoveCharacter()
        {
            Vector2 vector = new Vector2();

            if (myCharacter.Vertiacl == Vertical.Top)
                vector.y = 0.6f;
            else if (myCharacter.Vertiacl == Vertical.Down)
                vector.y = -0.6f;

            if (myCharacter.Horizontal == Horizontal.Right)
                vector.x = 1;
            else if (myCharacter.Horizontal == Horizontal.Left)
                vector.x = -1;

            myCharacter.MovementBody.MovePosition(
                myCharacter.MovementBody.position + vector.normalized
                * myCharacter.MoveSpeed * Time.deltaTime);
        }

        public override void End()
        {
            myCharacter.MoveSound.Stop();
        }
        #endregion

        #region 外部操作
        public override void Move(Vertical direction)
        {
            if (myCharacter.Vertiacl != direction)
            {
                DirectionChange = true;
                vertical = direction;
            }
        }

        public override void Move(Horizontal direction)
        {
            if (myCharacter.Horizontal != direction)
            {
                DirectionChange = true;
                horizontal = direction;
            }
        }

        public override void Deffend(bool deffend)
        {
            if (deffend)
                myCharacter.SetAction(new PlayerDeffend());
        }

        public override void HeavyAttack()
        {
            myCharacter.SetAction(new PlayerHeavyAttackPrepar());
        }

        public override void LightAttack()
        {
            myCharacter.SetAction(new PlayerLightAttack());
        }
        #endregion
    }

    /// <summary>
    /// 玩家防禦
    /// </summary>
    public class PlayerDeffend : ICharacterAction
    {
        bool DirectionChange = false;
        Vertical vertical;
        Horizontal horizontal;

        #region 動作更新
        public override void Inital()
        {
            vertical = myCharacter.Vertiacl;
            horizontal = myCharacter.Horizontal;
            myCharacter.CharacterAnimator.SetBool("IsDeffend", true);
        }

        public override void Update()
        {
            if (!(vertical == Vertical.None && horizontal == Horizontal.None)
                && DirectionChange)
            {
                myCharacter.CharacterAnimator.SetFloat("Vertical", (float)myCharacter.Vertiacl);
                myCharacter.CharacterAnimator.SetFloat("Horizontal", (float)myCharacter.Horizontal);
                DirectionChange = false;
            }
        }

        public override void End()
        {
            myCharacter.CharacterAnimator.SetBool("IsDeffend", false);
        }
        #endregion

        #region 外部操作
        public override void Move(Vertical direction)
        {
            if (myCharacter.Vertiacl != direction)
            {
                DirectionChange = true;
                vertical = direction;
            }
        }

        public override void Move(Horizontal direction)
        {
            if (myCharacter.Horizontal != direction)
            {
                DirectionChange = true;
                horizontal = direction;
            }
        }

        public override void Deffend(bool deffend)
        {
            if (!deffend)
            {
                if (vertical == Vertical.None &&
                    horizontal == Horizontal.None)
                    myCharacter.SetAction(new PlayerIdel());
                else
                    myCharacter.SetAction(new PlayerMove());
            }
        }

        public override void LightAttack()
        {
            myCharacter.SetAction(new PlayerLightAttack());
        }

        public override void OnHit(Damage damage)
        {
            myCharacter.DeffendSound.Play();
            base.OnHit(new Damage
            {
                damage = (int)(damage.damage * 0.1f)
            });
        }
        #endregion
    }

    /// <summary>
    /// 玩家輕攻擊
    /// </summary>
    public class PlayerLightAttack : ICharacterAction
    {
        Horizontal horizontal;
        Vertical vertical;

        #region 動作更新
        public override void Inital()
        {
            horizontal = Horizontal.None;
            vertical = Vertical.None;

            myCharacter.CharacterAnimator.SetTrigger("LightAttack");
            myCharacter.LightAttackSound.Play();
        }

        public override void Update()
        {
            if (myCharacter.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95)
            {
                if (horizontal == Horizontal.None && vertical == Vertical.None)
                    myCharacter.SetAction(new PlayerIdel());
                else
                {
                    myCharacter.Horizontal = horizontal;
                    myCharacter.Vertiacl = vertical;
                    myCharacter.SetAction(new PlayerMove());
                }
            }
        }
        #endregion

        #region 外部操作
        public override void Move(Horizontal direction)
        {
            horizontal = direction;
        }

        public override void Move(Vertical direction)
        {
            vertical = direction;
        }
        #endregion
    }

    /// <summary>
    /// 玩家重攻擊預備
    /// </summary>
    public class PlayerHeavyAttackPrepar : ICharacterAction
    {
        bool isCharge;

        #region 動作更新
        public override void Inital()
        {
            isCharge = true;

            myCharacter.CharacterAnimator.SetBool("HeavyAttackStart", true);
        }

        public override void Update()
        {
            if (myCharacter.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
                myCharacter.SetAction(new PlayerHeavyAttackDodge(isCharge));
        }
        #endregion

        #region 外部操作
        public override void HeavyAttack(bool hold)
        {
            if (!hold)
                isCharge = false;
        }
        #endregion
    }

    /// <summary>
    /// 玩家重攻擊衝刺
    /// </summary>
    public class PlayerHeavyAttackDodge : ICharacterAction
    {
        float DodgeDistance, TargetDistance = 1f;
        bool isCharge;

        public PlayerHeavyAttackDodge(bool isCharge)
        {
            this.isCharge = isCharge;
        }

        #region 動作更新
        public override void Inital()
        {
            //GameManager.Instance.PlayerDodge();
            DodgeDistance = 0;
        }

        public override void Update()
        {
            Vector2 vector = new Vector2();

            if (myCharacter.Vertiacl == Vertical.Top)
                vector.y = 0.6f;
            else if (myCharacter.Vertiacl == Vertical.Down)
                vector.y = -0.6f;

            if (myCharacter.Horizontal == Horizontal.Right)
                vector.x = 1;
            else if (myCharacter.Horizontal == Horizontal.Left)
                vector.x = -1;

            myCharacter.MovementBody.MovePosition(
                myCharacter.MovementBody.position + vector.normalized
                * myCharacter.MoveSpeed * Time.deltaTime);

            DodgeDistance += vector.magnitude;

            myCharacter.MovementBody.MovePosition(
                myCharacter.MovementBody.position + vector);

            if (DodgeDistance > TargetDistance)
                myCharacter.SetAction(new PlayerHeavyAttack1(isCharge));
        }
        #endregion

        #region 外部操作
        public override void HeavyAttack(bool hold)
        {
            if (!hold)
                isCharge = false;
        }

        public override void OnHit(Damage damage) { }
        #endregion
    }

    /// <summary>
    /// 玩家重攻擊第一段
    /// </summary>
    public class PlayerHeavyAttack1 : ICharacterAction
    {
        bool isCharge;

        public PlayerHeavyAttack1(bool isCharge)
        {
            this.isCharge = isCharge;
        }

        #region 動作更新
        public override void Inital()
        {
            myCharacter.CharacterAnimator.SetBool("HeavyAttackStart", false);
            myCharacter.HeavyAttack1Sound.Play();
        }

        public override void Update()
        {
            if (isCharge)
                myCharacter.SetAction(new PlayerHeavyAttackCharge());
            else
                myCharacter.SetAction(new PlayerIdel());
        }
        #endregion

        #region 外部操作
        public override void HeavyAttack(bool hold)
        {
            if (!hold)
                isCharge = false;
        }

        public override void OnHit(Damage damage) { }
        #endregion
    }

    /// <summary>
    /// 玩家重攻擊蓄力
    /// </summary>
    public class PlayerHeavyAttackCharge : ICharacterAction
    {
        bool DirectionChange, IsCharge;
        float ChargeTime;

        #region 動作更新
        public override void Inital()
        {
            DirectionChange = false;
            IsCharge = true;
            ChargeTime = 0;

            myCharacter.CharacterAnimator.SetBool("HeavyAttackCharge", true);
            myCharacter.HeavyAttackChargeSound.Play();
        }

        public override void Update()
        {
            ChargeTime += Time.deltaTime;

            if (DirectionChange)
            {
                DirectionChange = false;
            }

            if (ChargeTime > 2.1 || !IsCharge)
            {
                if (ChargeTime < 0.7)
                    myCharacter.SetAction(new PlayerHeavyAttack2(0));
                else if (ChargeTime < 1.4)
                    myCharacter.SetAction(new PlayerHeavyAttack2(1));
                else
                    myCharacter.SetAction(new PlayerHeavyAttack2(2));
            }
        }
        #endregion

        #region 外部操作
        public override void HeavyAttack(bool hold)
        {
            if (!hold)
                IsCharge = false;
        }

        public override void Move(Vertical direction)
        {
            if (myCharacter.Vertiacl != direction)
            {
                DirectionChange = true;
                myCharacter.Vertiacl = direction;
            }
        }

        public override void Move(Horizontal direction)
        {
            if (myCharacter.Horizontal != direction)
            {
                DirectionChange = true;
                myCharacter.Horizontal = direction;
            }
        }
        #endregion
    }

    /// <summary>
    /// 玩家重攻擊第二段
    /// </summary>
    public class PlayerHeavyAttack2 : ICharacterAction
    {
        int chargeState;

        public PlayerHeavyAttack2(int chargeState)
        {
            this.chargeState = chargeState;
        }

        #region 動作更新
        public override void Inital()
        {
            myCharacter.CharacterAnimator.SetBool("HeavyAttackCharge", false);
            myCharacter.HeavyAttack2Sound.Play();
        }

        public override void Update()
        {
            if (myCharacter.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95)
            {
                if (chargeState == 2)
                    myCharacter.SetAction(new PlayerHeavyAttackRecovery());
                else
                    myCharacter.SetAction(new PlayerIdel());
            }
        }
        #endregion

        #region 外部操作
        public override void OnHit(Damage damage) { }
        #endregion
    }

    /// <summary>
    /// 玩家重攻擊硬直
    /// </summary>
    public class PlayerHeavyAttackRecovery : ICharacterAction
    {
        float recoveryTime;

        #region 動作更新
        public override void Inital()
        {
            recoveryTime = 0;

            myCharacter.CharacterAnimator.SetBool("IsMove", false);
        }

        public override void Update()
        {
            recoveryTime += Time.deltaTime;
            if (recoveryTime > 2)
                myCharacter.SetAction(new PlayerIdel());
        }
        #endregion

        #region 外部操作
        public override void OnHit(Damage damage)
        {
            base.OnHit(new Damage
            {
                damage = (int)(damage.damage * 2.5)
            });
            myCharacter.SetAction(new PlayerFall(true));
        }
        #endregion
    }

    /// <summary>
    /// 玩家倒地
    /// </summary>
    public class PlayerFall : ICharacterAction
    {
        bool hitable;
        float fallDownTime;

        public PlayerFall(bool hitable)
        {
            this.hitable = hitable;
        }

        #region 動作更新
        public override void Inital()
        {
            fallDownTime = 0;

            myCharacter.CharacterAnimator.SetBool("IsFallDown", true);
            myCharacter.FallDownSound.Play();
        }

        public override void Update()
        {
            fallDownTime += Time.deltaTime;
            if (fallDownTime > 5)
                myCharacter.SetAction(new PlayerIdel());
        }

        public override void End()
        {
            myCharacter.Data.Recovery = 0;
        }
        #endregion

        #region 外部操作
        public override void Move(Vertical direction)
        {
            TryToRecurve(hitable);
        }

        public override void Move(Horizontal direction)
        {
            TryToRecurve(hitable);
        }

        public override void Dodge()
        {
            TryToRecurve(hitable);
        }

        public override void LightAttack()
        {
            TryToRecurve(hitable);
        }

        public override void HeavyAttack()
        {
            TryToRecurve(hitable);
        }

        public override void Deffend(bool deffend)
        {
            TryToRecurve(hitable);
        }

        public override void OnHit(Damage damage)
        {
            if (hitable)
                base.OnHit(new Damage
                {
                    damage = (int)(damage.damage * 0.5),
                    vertigo = damage.vertigo * 0.5f
                });
        }
        #endregion

        private void TryToRecurve(bool IsHitable)
        {
            if (IsHitable)
                fallDownTime += 0.1f;
            else
                fallDownTime += 0.2f;
        }
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    public class PlayerDead : ICharacterAction
    {
        #region 動作更新
        public override void Inital()
        {
            myCharacter.CharacterAnimator.SetBool("IsFallDown", true);
            myCharacter.FallDownSound.Play();
        }
        #endregion
    }
}
