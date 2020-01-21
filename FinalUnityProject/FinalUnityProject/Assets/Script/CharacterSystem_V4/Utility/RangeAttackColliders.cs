using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem_V4
{
    public class RangeAttackColliders : MonoBehaviour
    {
        public DamageData MyDamage;
        public string TargetTag;
        public int BlockingLayer;
        public bool HitAll;

        public Transform Bullet;

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

            if ((!HitAll && hasHitTarget)
                || collision.gameObject.layer == BlockingLayer)
            {
                
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == TargetTag)
            {
                //Debug.Log($"Target Exit: {TargetTag}");
                hasHitTarget = false;
            }
        }
    }
}
