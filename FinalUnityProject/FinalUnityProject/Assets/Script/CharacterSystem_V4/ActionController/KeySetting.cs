using UnityEngine;

namespace CharacterSystem
{
    [CreateAssetMenu(fileName = "按鍵設定", menuName = "賞金獵人_角色系統V4/按鍵設定", order = 0)]
    public class KeySetting : ScriptableObject
    {
        public KeyCode UpKey, DownKey, LeftKey, RightKey, LightAttack, HeavyAttack, Deffend;
    }
}
