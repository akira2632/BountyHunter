using UnityEngine;
using DG.Tweening;

namespace CharacterSystem_V4
{
    public class RangeAttackColliders : MonoBehaviour , IAnimateStateInvokeTarget
    {
        public DamageData MyDamage;

        public string TargetTag;
        public int BlockingLayer;
        public bool HitAll;

        [Tooltip("子彈發射點, 請由左至右、由下至上排列")]
        public Transform[] StartPoints;
        public Transform Bullet;
        public float BulletSpeed, BulletRange;
        public Ease BulletMoveEase;

        private bool hasHitTarget;

        private void Start()
        {
            hasHitTarget = false;
        }

        public void SetProjecter(Vector3 targetPosition, int vertical, int horizontal)
        {
            var bulletStartPosition = StartPoints[(vertical + 1) * 3 + horizontal + 1].position;
            var bulletEndPosition = IsometricUtility.ToIsometricDirection(targetPosition - bulletStartPosition) * BulletRange;
            Bullet.gameObject.SetActive(true);
            Bullet.GetComponent<Animator>().SetTrigger("Throw");
            Bullet.transform
                .DOBlendableMoveBy(bulletEndPosition, (BulletRange / BulletSpeed))
                .ChangeStartValue(bulletStartPosition)
                .SetEase(BulletMoveEase);
        }

        public void AnimationStart() { }
        public void AnimationEnd()
        {
            Bullet.gameObject.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.layer == BlockingLayer)
            {
                Bullet.GetComponent<Animator>().SetTrigger("Destroy");
            }
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

                if(!HitAll)
                    Bullet.GetComponent<Animator>().SetTrigger("Destroy");
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
