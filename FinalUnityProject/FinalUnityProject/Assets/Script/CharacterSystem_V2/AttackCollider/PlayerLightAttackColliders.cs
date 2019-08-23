using CharacterSystem_V2.InterfaceAndData;
using UnityEngine;

namespace CharacterSystem_V2.AttackColliders
{
    public class PlayerLightAttackColliders : MonoBehaviour
    {
        public Damage lightAttackDamage;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Enemy")
                collision.gameObject.
                    GetComponent<ICharacterControll>().
                    OnHit(lightAttackDamage);
        }
    }
}
