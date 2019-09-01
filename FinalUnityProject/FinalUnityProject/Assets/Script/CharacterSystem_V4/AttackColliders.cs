using UnityEngine;

namespace CharacterSystem_V4
{
    public class AttackColliders : MonoBehaviour
    {
        public Wound MyDamage;
        public string TargetTag;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == TargetTag)
            {
                MyDamage.KnockBackFrom = gameObject.gameObject.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>().OnHit(MyDamage);
            }
        }
    }
}
