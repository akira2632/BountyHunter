using UnityEngine;

namespace CharacterSystem.Controller
{
    [CreateAssetMenu(fileName = "歐克隊長AI設定", menuName = "賞金獵人/AI設定/歐克隊長AI設定", order = 4)]
    public class OrcCaptainAISetting : ScriptableObject
    {
        public float IdelTimeMax, IdelTimeMin,
            WounderDistanceMax, WounderDistanceMin,
            DetectedDistance,
            AttackDistance, StopDistance;
    }
}