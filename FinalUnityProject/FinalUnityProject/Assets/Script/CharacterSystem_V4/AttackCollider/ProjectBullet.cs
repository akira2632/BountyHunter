using DG.Tweening;
using UnityEngine;

namespace CharacterSystem_V4.SkillCollider
{
    public class ProjectBullet : MonoBehaviour, IAnimateStateInvokeTarget
    {
        public DamageData MyDamage;

        public string TargetTag;
        public int BlockingLayer;
        public bool HitAll;

        public Vector3 target;
        public float BulletSpeed, BulletRange;
        public Ease BulletMoveEase;

        private bool hasHitTarget, hasShot;

        public void AnimationStart() { }
        public void AnimationEnd() => Destroy(gameObject);

        private void Start()
        {
            hasHitTarget = false;
            hasShot = false;
        }

        private void DestroyBullet()
        {
            transform.DOKill();
            GetComponent<Animator>().SetTrigger("Destroy");
        }

        public void Shooting(Vector3 target, DamageData damage)
        {
            MyDamage = damage;
            GetComponent<Animator>().SetTrigger("Thow");

            Vector3 endPosition = IsometricUtility.ToIsometricVector(
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

            if ((HitAll || !hasHitTarget) && collision.gameObject.tag == TargetTag)
            {
                //Debug.Log($"Target Enter : {TargetTag}");
                hasHitTarget = true;
                MyDamage.HitAt = collision.transform.position;
                MyDamage.HitFrom = gameObject.gameObject.transform.position;
                collision.gameObject.GetComponentInParent<ICharacterActionManager>().OnHit(MyDamage);

                if (!HitAll)
                    DestroyBullet();
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