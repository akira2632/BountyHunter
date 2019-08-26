using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4
{
    public class AttackColliders : MonoBehaviour
    {
        public Damage MyDamage;
        public string TargetTag;

        public Collision2D[] collision2Ds;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == TargetTag)
                collision.gameObject.GetComponent<ICharacterActionManager>().OnHit(MyDamage);
        }

        public void EnableCollider()
        {

        }
    }
}
