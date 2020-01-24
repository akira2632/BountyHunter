using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4.SkillCollider
{
    public class AttackColliders : MonoBehaviour
    {
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
                MyDamage.HitAt = collision.transform.position;
                MyDamage.HitFrom = gameObject.gameObject.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>().OnHit(MyDamage);
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
