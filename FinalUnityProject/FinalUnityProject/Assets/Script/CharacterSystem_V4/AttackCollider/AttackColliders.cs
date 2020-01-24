using UnityEngine;

namespace CharacterSystem_V4.SkillCollider
{
    public class AttackColliders : MonoBehaviour
    {
        public DamageData MyDamage;
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
                MyDamage.HitAt = collision.transform.position;
                MyDamage.HitFrom = gameObject.gameObject.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>().OnHit(MyDamage);
            }
        }
    }
}
