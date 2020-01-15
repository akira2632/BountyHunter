using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterSystem_V4;
using CharacterSystem_V4.Controller;

public class SpwanPoint : MonoBehaviour
{
    public SpwanMobData[] SpwanMobs;
    public int MaxMobCount, MobCount;
    public float ActiveRange;

    private Transform player;
    private bool unvisible, isActive;

    // Start is called before the first frame update
    void Start()
    {
        MobCount = 0;
        player = FindObjectOfType<PlayerController>().MyCharacter.transform;

        foreach (SpwanMobData item in SpwanMobs)
        {
            item.Start(gameObject);
        }

        if (IsometricUtility.ToIsometricDistance(
                transform.position, player.position) <= ActiveRange)
            isActive = true;
        else
            isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SpwanMobData item in SpwanMobs)
        {
            item.Update();

            if (item.ReadyToSpwan() && MobCount < MaxMobCount && unvisible)
            {
                item.SpwanMob();
            }
        }

        if (!isActive &&
            IsometricUtility.ToIsometricDistance(
                transform.position, player.position) <= ActiveRange)
        {
            isActive = true;
            foreach (SpwanMobData item in SpwanMobs)
            {
                item.SetActive(true);
            }
        }
        else if(isActive &&
            IsometricUtility.ToIsometricDistance(
                transform.position, player.position) > ActiveRange)
        {
            isActive = false;
            foreach (SpwanMobData item in SpwanMobs)
            {
                item.SetActive(false);
            }
        }
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
            mySpwanPoint.MobCount++;

            myMob = Instantiate(MobPrefab, mySpwanPoint.transform.position, Quaternion.identity);
            myMob.GetComponent<ICharacterActionManager>().OnCharacterDead
                += () =>
                {
                    IsAlive = false;
                    mySpwanPoint.MobCount--;
                };
        }
    }
}
