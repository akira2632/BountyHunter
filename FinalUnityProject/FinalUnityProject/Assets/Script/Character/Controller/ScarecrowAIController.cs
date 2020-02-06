using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem.Controller
{
    public class ScarecrowAIController : MonoBehaviour
    {
        public CharacterActionController Character;
        public BasicAISenser Senser;
        public float AttackDistance;

        private GameObject player;

        #region StateControl
        private void Start()
        {
            player = FindObjectOfType<PlayerController>().MyCharacter.gameObject;
        }

        private void Update()
        {
            if (IsometricUtility.ToIsometricDistance(player.transform.position, Character.transform.position) <= AttackDistance
                && Character.CharacterData.BasicAttackTimer <= 0)
                Character.BasicAttack();
        }
        #endregion
    }
}