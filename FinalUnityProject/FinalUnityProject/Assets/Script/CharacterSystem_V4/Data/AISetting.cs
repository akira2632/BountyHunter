using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    [CreateAssetMenu(fileName ="AI設定", menuName = "賞金獵人_角色系統V4 /AI設定")]
    public class AISetting : ScriptableObject
    {
        public float IdelTimeMax, IdelTimeMin,
            WounderDistanceMax, WounderDistanceMin;
    }
}
