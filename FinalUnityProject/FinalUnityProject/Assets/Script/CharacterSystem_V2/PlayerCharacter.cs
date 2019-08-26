using UnityEngine;
using CharacterSystem_V2.InterfaceAndData;
using CharacterSystem_V2.Action;
using CharacterSystem_V2.AttackColliders;

namespace CharacterSystem_V2
{
    #region 角色狀態機
    public class PlayerCharacter : MonoBehaviour, ICharacter
    {
        public CharacterData Data;
        public ICharacterAttribute Attirbute;

        public Rigidbody2D MovementBody;
        public Animator CharacterAnimator;

        public AudioSource MoveSound ,DeffendSound, FallDownSound, LightAttackSound,
            HeavyAttack1Sound, HeavyAttackChargeSound, HeavyAttack2Sound;

        public PlayerLightAttackColliders lightAttackCollider;
        public PlayerHeavyAttack1Colliders playerHeavyAttack1;
        public PlayerHeavyAttack2Colliders playerHeavyAttack2;

        #region 狀態管理
        private ICharacterAction nowAction;
        private bool hasInitail = false;

        public void Start()
        {
            nowAction = new PlayerIdel();
            nowAction.SetManager(this);

            Attirbute = new TempAttribute();
            Data = new CharacterData(this);
        }

        public void FixedUpdate()
        {
            if (!hasInitail)
            {
                nowAction.Inital();
                hasInitail = true;
            }

            if (Data.Recovery > 4)
                SetAction(new PlayerFall(false));

            if (Data.Health > 0)
                Regen();
            else
                SetAction(new PlayerDead());

            nowAction.Update();
        }

        public void SetAction(ICharacterAction nextActoin)
        {
            nowAction.End();
            nowAction = nextActoin;
            nowAction.SetManager(this);
            hasInitail = false;
        }
        #endregion

        #region ICharacterData委派
        public Vertical Vertiacl { get => Data.Vertiacl; set => Data.Vertiacl = value; }
        public Horizontal Horizontal { get => Data.Horizontal; set => Data.Horizontal = value; }
        public float RegenTime { get => Data.RegenTime; set => Data.RegenTime = value; }
        public int Health { get => Data.Health; set => Data.Health = value; }
        public float Recovery { get => Data.Recovery; set => Data.Recovery = value; }
        #endregion

        #region ICharacterAttribute委派
        public float RegenSpeed => Attirbute.RegenSpeed;
        public int RegenHealth => Attirbute.RegenHealth;
        public float MoveSpeed => Attirbute.MoveSpeed;
        public float DodgeSpeed => Attirbute.DodgeSpeed;
        public int MaxHealth => Attirbute.MaxHealth;
        #endregion

        #region ICharacterControll委派
        public void Move(Vertical direction) => nowAction.Move(direction);
        public void Move(Horizontal direction) => nowAction.Move(direction);

        public void Dodge() => nowAction.Dodge();
        public void LightAttack() => nowAction.LightAttack();
        public void HeavyAttack() => nowAction.HeavyAttack();
        public void HeavyAttack(bool hold) => nowAction.HeavyAttack(hold);
        public void Deffend(bool deffend) => nowAction.Deffend(deffend);

        public void OnHit(Damage damage) => nowAction.OnHit(damage);
        #endregion

        public void Regen()
        {
            Data.RegenTime += Time.deltaTime;
            if (Data.RegenTime > Attirbute.RegenSpeed)
            {
                Data.Health += Attirbute.RegenHealth;
                Data.RegenTime = 0;
            }

            Data.Recovery -= Time.deltaTime;
        }
    }

    namespace InterfaceAndData
    {
        public abstract class ICharacterAction : ICharacterControll
        {
            #region 狀態管理
            protected PlayerCharacter myCharacter;
            public void SetManager(PlayerCharacter manager)
            {
                myCharacter = manager;
            }

            public virtual void Inital() { }
            public virtual void Update() { }
            public virtual void End() { }
            #endregion

            #region ICharacterControll方法
            public virtual void Move(Vertical direction) { }
            public virtual void Move(Horizontal direction) { }

            public virtual void Dodge() { }
            public virtual void LightAttack() { }
            public virtual void HeavyAttack() { }
            public virtual void HeavyAttack(bool hold) { }
            public virtual void Deffend(bool deffend) { }

            public virtual void OnHit(Damage damage)
            {
                myCharacter.Data.Health -= damage.damage;
                myCharacter.Data.Recovery -= damage.vertigo;
            }
            #endregion
        }
    }
    #endregion
}
