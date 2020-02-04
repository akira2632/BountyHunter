using UnityEngine;

namespace CharacterSystem.Controller
{
    [CreateAssetMenu(fileName = "基本AI設定", menuName = "賞金獵人/AI設定/基本AI設定", order = 0)]
    public class BasicAISetting : ScriptableObject
    {
        public float IdelTimeMax, IdelTimeMin,
            WounderDistanceMax, WounderDistanceMin,
            DetectedDistance,
            AttackDistance, StopDistance;
    }
}
