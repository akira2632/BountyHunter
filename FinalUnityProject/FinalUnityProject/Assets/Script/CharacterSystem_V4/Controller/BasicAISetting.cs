using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    [CreateAssetMenu(fileName = "基本AI設定", menuName = "賞金獵人_角色系統V4 /基本AI設定", order = 0)]
    public class BasicAISetting : ScriptableObject
    {
        public float IdelTimeMax, IdelTimeMin,
            WounderDistanceMax, WounderDistanceMin,
            DetectedDistance,
            AttackDistance, StopDistance;
    }
}
