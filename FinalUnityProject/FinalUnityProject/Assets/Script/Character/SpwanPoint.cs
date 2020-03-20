using Character.Controller;
using UnityEngine;

namespace Character
{
    public class SpwanPoint : MonoBehaviour
    {
        public float ActiveRange, PlayerDistance;
        public bool SpwanWhenVisible;
        public SpwanMobData[] SpwanMobs;

        private Transform player;
        private bool unvisible, isActive;

        // Start is called before the first frame update
        void Start()
        {
            player = FindObjectOfType<PlayerController>().MyCharacter.transform;

            foreach (SpwanMobData item in SpwanMobs)
            {
                item.Start(gameObject);
            }

            isActive = CaculatePlayerDistance() <= ActiveRange;
            unvisible = true;        }

        // Update is called once per frame
        void Update()
        {
            PlayerDistance = CaculatePlayerDistance();

            foreach (SpwanMobData item in SpwanMobs)
            {
                item.Update();

                if (isActive
                    && item.ReadyToSpwan()
                    && (unvisible || SpwanWhenVisible))
                    item.SpwanMob();
            }

            //重新啟動的怪物及AI會出現狀態不同步的錯誤、在相關問題解決前先關閉重設相關程式
            if (!isActive &&
                PlayerDistance <= ActiveRange)
            {
                //Debug.Log($"{transform.name} player distance = {PlayerDistance}");
                isActive = true;
                //foreach (SpwanMobData item in SpwanMobs)
                //{
                //    item.SetActive(true);
                //}
            }
            else if (isActive &&
                PlayerDistance > ActiveRange)
            {
                //Debug.Log($"{transform.name} player distance = {PlayerDistance}");
                isActive = false;
                //foreach (SpwanMobData item in SpwanMobs)
                //{
                //    item.SetActive(false);
                //}
            }
        }

        private float CaculatePlayerDistance()
        {
            return transform.position.IsoDistance(player.position);
        }

        private void OnBecameVisible()
        {
            unvisible = false;
        }

        private void OnBecameInvisible()
        {
            unvisible = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "SpawnPointIcon.png");
        }

        [System.Serializable]
        public class SpwanMobData
        {
            public GameObject MobPrefab;
            public float SpwanRate;
            public float Timer;
            public bool IsAlive;

            private SpwanPoint mySpwanPoint;
            private GameObject myMob;

            public void Start(GameObject spwanPoint)
            {
                Timer = 0;
                IsAlive = false;
                mySpwanPoint = spwanPoint.GetComponent<SpwanPoint>();
            }

            public void Update()
            {
                if (!IsAlive && Timer > 0)
                    Timer -= Time.deltaTime;
            }

            public void SetActive(bool active)
            {
                if (myMob == null)
                    return;

                if (active)
                    myMob.transform.position = mySpwanPoint.transform.position;

                myMob.SetActive(active);
            }

            public bool ReadyToSpwan()
            {
                return !IsAlive && Timer <= 0;
            }

            public void SpwanMob()
            {
                IsAlive = true;
                Timer = SpwanRate;

                myMob = Instantiate(MobPrefab, mySpwanPoint.transform.position, Quaternion.identity);
                myMob.GetComponent<CharacterActionController>().OnCharacterDead
                    += () => IsAlive = false;
            }
        }
    }
}