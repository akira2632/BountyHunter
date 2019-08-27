using UnityEngine;

namespace CharacterSystem_V4
{
    public class AttackColliders : MonoBehaviour
    {
        public Damage MyDamage;
        public string TargetTag;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == TargetTag)
                collision.gameObject.GetComponent<ICharacterActionManager>().OnHit(MyDamage);
        }
    }
}
