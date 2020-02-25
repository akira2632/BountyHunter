using UnityEngine;

namespace Character.Controller
{
    [CreateAssetMenu(fileName = "蜘蛛AI設定", menuName = "賞金獵人/AI設定/蜘蛛AI設定", order = 2)]
    public class SpiderAISetting : ScriptableObject
    {
        public float IdelTimeMax, IdelTimeMin,
            WounderDistanceMax, WounderDistanceMin,
            DetectedDistance,
            AttackDistance, StopDistance;
        [Range(0, 360)]
        public float AroundDegree;
        [Min(0)]
        public float AroundRadius;
        [Min(1)]
        public int RoundTurn;
    }
}