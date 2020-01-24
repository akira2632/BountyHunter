using UnityEngine;

namespace CharacterSystem_V4.SkillCollider
{
    public class ProjectorShooter : MonoBehaviour
    {
        public DamageData MyDamage;

        [Tooltip("子彈發射點, 請由左至右、由下至上排列")]
        public Transform[] ShottingPoints;
        public GameObject Bullet;

        public void ShootingProjector(Vector3 target, int vertical, int horizontal)
        {
            var StartPosition = ShottingPoints[(vertical + 1) * 3 + horizontal + 1].position;
            var bullet = Instantiate(Bullet, StartPosition, Quaternion.identity);
            bullet.GetComponent<ProjectSkillCollider>().Shooting(target, MyDamage);
        }
    }
}
