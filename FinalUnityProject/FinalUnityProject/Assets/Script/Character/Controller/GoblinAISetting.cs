using UnityEngine;

namespace CharacterSystem.Controller
{
    [CreateAssetMenu(fileName = "哥布林AI設定", menuName = "賞金獵人/AI設定/哥布林AI設定", order = 1)]
    public class GoblinAISetting : ScriptableObject
    {
        public float IdelTimeMax, IdelTimeMin,
            WounderDistanceMax, WounderDistanceMin,
            DetectedDistance,
            BasicAttackDistance, SpacilAttackDistance, StopDistance
            , AroundRadius, AroundDegree;
        public int SpacilAttackChangeSideConter, RoundTurn;
    }
}
