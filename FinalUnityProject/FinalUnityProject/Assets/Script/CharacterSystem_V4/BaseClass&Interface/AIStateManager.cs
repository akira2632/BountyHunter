using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public abstract class AIStateManager : MonoBehaviour
    {
        public ICharacterActionManager Character;
        public BasicAISenser Senser;
        public AISetting AISetting;

        protected bool isInitial = false;
        protected AIState nowState;

        private void Update()
        {
            if (Character == null)
                Destroy(gameObject);

            if (!isInitial)
            {
                nowState.Initial();
                isInitial = true;
            }

            nowState.Update();
        }

        public void SetState(AIState nextState)
        {
            nowState.End();
            isInitial = false;
            nowState = nextState;
            nowState.SetManager(this);
        }
    }

    public abstract class AIState
    {
        protected AIStateManager manager;

        public void SetManager(AIStateManager manager)
            => this.manager = manager;

        public virtual void Initial() { }
        public virtual void Update() { }
        public virtual void End() { }
    }
}
