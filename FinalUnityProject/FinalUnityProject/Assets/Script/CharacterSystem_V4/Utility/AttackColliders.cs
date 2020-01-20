using UnityEngine;

namespace CharacterSystem_V4
{
    public class AttackColliders : MonoBehaviour
    {
        public Wound MyDamage;
        public string TargetTag;
        public bool HitAll;

        private bool hasHitTarget;

        private void Start()
        {
            hasHitTarget = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((HitAll || !hasHitTarget) && collision.gameObject.tag == TargetTag)
            {
                //Debug.Log($"Target Enter : {TargetTag}");
                hasHitTarget = true;
                MyDamage.HitFrom = gameObject.gameObject.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>().OnHit(MyDamage);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.gameObject.tag == TargetTag)
            {
                //Debug.Log($"Target Exit: {TargetTag}");
                hasHitTarget = false;
            }
        }
    }
}
