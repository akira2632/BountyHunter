using UnityEngine;

namespace Character.Controller
{
    public class ScarecrowAI : MonoBehaviour
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
            if (player.transform.position.IsoDistance(Character.transform.position) <= AttackDistance
                && Character.CharacterData.BasicAttackTimer <= 0)
                Character.BasicAttack();
        }
        #endregion
    }
}