using UnityEngine;

namespace CharacterSystem.Skill
{
    public class ProjectorShooter : MonoBehaviour
    {
        public CharacterActionManager Character;
        public SkillDamage SkillDamage;
        public bool shoot;
        private bool hasShoot;

        [Tooltip("子彈發射點, 請由左至右、由下至上排列")]
        public Transform[] ShottingPoints;
        public GameObject Bullet;

        public void Update()
        {
            if(shoot && !hasShoot)
            {
                ShootingProjector();
                hasShoot = true;
            }

            if(!shoot && hasShoot)
            {
                hasShoot = false;
            }
        }

        private void ShootingProjector()
        {
            #region 取得發射位置
            IsometricUtility.GetVerticalAndHorizontal(Character.CharacterData.Direction
                ,out float vertical, out float horizontal);
            var positionIndex = ((int)vertical + 1) * 3 + (int)horizontal + 1;
            positionIndex = positionIndex > 4 ? positionIndex - 1 : positionIndex;
            var startPosition = ShottingPoints[positionIndex].position;
            #endregion

            var damage = SkillDamage.GetDamageData(Character.CharacterData);
            damage.HitFrom = gameObject.gameObject.transform.position;
            Instantiate(Bullet, startPosition, Quaternion.identity)
                .GetComponent<ProjectSkillCollider>()
                .Shooting(Character.CharacterData.TargetPosition, damage);
        }
    }
}
