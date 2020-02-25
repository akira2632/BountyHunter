using System;
using UnityEngine;

namespace Character
{
    public class WarriorEventsManager : MonoBehaviour
    {
        public event Action<float> OnWarriorMove, OnWarriorTurnInDeffend;
        public event Action OnWarriorBasicAttack, OnWarriorSpecailAttack1
            , OnWarriorSpecailAttack2, OnWarriorDeffend;

        #region MonoState
        private static WarriorEventsManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }
        #endregion

        public static void WarriorMove(float time)
        {
            if (_instance == null)
                return;

            //Debug.Log("WarriorMove");
            _instance.OnWarriorMove?.Invoke(time);
        }

        public static void WarriorBasicAttack()
        {
            if (_instance == null)
                return;

            //Debug.Log("WarriorBasicAttack");
            _instance.OnWarriorBasicAttack?.Invoke();
        }

        public static void WarriorSpecailAttack1()
        {
            if (_instance == null)
                return;

            //Debug.Log("WarriorSpecailAttack1");
            _instance.OnWarriorSpecailAttack1?.Invoke();
        }

        public static void WarriorSpecailAttack2()
        {
            if (_instance == null)
                return;

            //Debug.Log("WarriorSpecailAttack2");
            _instance.OnWarriorSpecailAttack2?.Invoke();
        }

        public static void WarriorDeffend()
        {
            if (_instance == null)
                return;

            //Debug.Log("WarriorDeffend");
            _instance.OnWarriorDeffend?.Invoke();
        }

        public static void WarriorTurnInDeffend(float time)
        {
            if (_instance == null)
                return;

            //Debug.Log("WarriorTurnInDeffend");
            _instance.OnWarriorTurnInDeffend?.Invoke(time);
        }
    }
}
