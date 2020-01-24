using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4.SkillCollider
{
    public class SkillColliders : MonoBehaviour
    {
        public ICharacterActionManager Character;
        public SkillDamage SkillDamage;

        public string TargetTag;
        public bool HitAll;

        public DamageData MyDamage;

        private List<Collider2D> hittedTargets = new List<Collider2D>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == TargetTag
                && !hittedTargets.Contains(collision)
                && (HitAll || hittedTargets.Count <= 0))
            {
                //Debug.Log($"Target Enter : {TargetTag}");
                hittedTargets.Add(collision);
                var damage = SkillDamage.GetDamageData(Character.Property);
                damage.HitAt = collision.transform.position;
                damage.HitFrom = gameObject.gameObject.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>()
                    .OnHit(damage);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == TargetTag
                && hittedTargets.Contains(collision))
            {
                //Debug.Log($"Target Exit: {TargetTag}");
                hittedTargets.Remove(collision);
            }
        }
    }
}
