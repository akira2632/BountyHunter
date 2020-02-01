using System;
using UnityEngine;

namespace CharacterSystem
{
    public class CharacterActionManager : MonoBehaviour,
        ICharacterActionControll, IAnimationStateHandler
    {
        public CharacterData CharacterData;
        [SerializeField]
        private ICharacterActionProvider ActionProvider;

        public Rigidbody2D MovementBody;
        public Collider2D MovementCollider;
        public Animator CharacterAnimator;
        public AudioSource AudioSource;
        public SpriteRenderer SpriteRenderer;

        public event Action OnCharacterDead;
        private bool hasInvoke;

        #region 流程控制
        protected ICharacterAction nowAction;
        private bool IsStart;

        private void Start()
        {
            nowAction = ActionProvider.GetIdelAction(this);
        }

        public void FixedUpdate()
        {
            if (CharacterData.Health <= 0 && !hasInvoke)
            {
                OnCharacterDead?.Invoke();
                SetAction(ActionProvider.GetDeadAction(this));
                hasInvoke = true;
            }

            if (CharacterData.Health > 0
                && CharacterData.VertigoConter >= 4)
                SetAction(ActionProvider.GetFallDownAction(this));

            ActionUpdate();
        }

        public virtual void ActionUpdate()
        {
            if(!IsStart)
            {
                nowAction.Start();
                IsStart = true;
            }

            nowAction.Update();
        }

        public void SetAction(ICharacterAction nextAction)
        {
            nowAction.End();
            nowAction = nextAction;
            IsStart = false;
        }
        #endregion

        #region AnimationControll委派
        public void OnAnimationStart() => nowAction.OnAnimationStart();
        public void OnAnimationEnd() => nowAction.OnAnimationEnd();
        #endregion

        #region ICharacterActionControll委派
        public void Move(Vector2 direction) => nowAction.Move(direction);

        public void Dodge() => nowAction.Dodge();
        public void Deffend(bool deffend) => nowAction.Deffend(deffend);
        public void BasicAttack() => nowAction.BasicAttack();
        public void SpecialAttack() => nowAction.SpecialAttack();
        public void SpecialAttack(bool hold) => nowAction.SpecialAttack(hold);
        public void SpecialAttack(Vector3 tartgetPosition) => nowAction.SpecialAttack(tartgetPosition);

        public void OnHit(DamageData wound) => nowAction.OnHit(wound);
        #endregion
    }
}
