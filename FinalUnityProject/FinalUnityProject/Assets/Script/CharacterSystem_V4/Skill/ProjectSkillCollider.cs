using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CharacterSystem.Skill
{
    public class ProjectSkillCollider : MonoBehaviour, IAnimateStateInvokeTarget
    {
        private DamageData MyDamage;

        public string TargetTag;
        public int BlockingLayer;
        public bool HitAll;

        public Vector3 target;
        public float BulletSpeed, BulletRange;
        public Ease BulletMoveEase;

        private List<Collider2D> hittedTargets = new List<Collider2D>();
        private bool hasShot = false;

        public void OnAnimationStart() { }
        public void OnAnimationEnd() => Destroy(gameObject);

        private void DestroyBullet()
        {
            transform.DOKill();
            GetComponent<Animator>().SetTrigger("Destroy");
        }

        public void Shooting(Vector3 target, DamageData damage)
        {
            MyDamage = damage;

            Vector3 endPosition = IsometricUtility.ToIsometricVector3(
                (target - transform.position).normalized * BulletRange);
            transform.DOBlendableMoveBy(endPosition, (BulletRange / BulletSpeed))
                .SetEase(BulletMoveEase)
                .onComplete += () => GetComponent<Animator>().SetTrigger("Destroy");
            hasShot = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!hasShot)
                return;

            if (collision.gameObject.layer == BlockingLayer)
            {
                DestroyBullet();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hasShot)
                return;

            if (collision.gameObject.tag == TargetTag
                && !hittedTargets.Contains(collision)
                && (HitAll || hittedTargets.Count <= 0))
            {
                //Debug.Log($"Target Enter : {TargetTag}");
                hittedTargets.Add(collision);
                MyDamage.HitAt = collision.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>().OnHit(MyDamage);

                if (!HitAll)
                    DestroyBullet();
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