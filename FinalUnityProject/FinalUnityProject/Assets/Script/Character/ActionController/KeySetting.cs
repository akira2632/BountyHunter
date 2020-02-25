using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "按鍵設定", menuName = "賞金獵人/按鍵設定", order = 0)]
    public class KeySetting : ScriptableObject
    {
        public KeyCode UpKey, DownKey, LeftKey, RightKey, LightAttack, HeavyAttack, Deffend;
    }
}
