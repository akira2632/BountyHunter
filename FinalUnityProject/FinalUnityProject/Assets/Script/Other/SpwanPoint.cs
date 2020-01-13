using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPoint : MonoBehaviour
{
    public SpwanMobData[] SpwanMobs;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        foreach (SpwanMobData item in SpwanMobs)
        {
            item.Start(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SpwanMobData item in SpwanMobs)
        {
            item.Update();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "SpawnPointIcon.png");
    }

    public enum SpwanState
    {
        CoolDown
    }

    [System.Serializable]
    public struct SpwanMobData
    {

        public GameObject Mob;
        public float SpwanRate;

        public float Timer { get => _timer; }
        public bool IsAlive { get => _isAlive; }

        private float _timer;
        private bool _isAlive;
        private SpwanPoint mySpwanPoint;

        public void Start(SpwanPoint spwanPoint)
        {
            _timer = 0;
            _isAlive = true;
            mySpwanPoint = spwanPoint;
        }

        public void Update()
        {
            if (!_isAlive)
                _timer -= Time.deltaTime;
            else
                SpwanMob();
        }

        private void SpwanMob()
        {
            var temp = Instantiate(Mob, mySpwanPoint.transform.position, Quaternion.identity);

        }
    }
}
