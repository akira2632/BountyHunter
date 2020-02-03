using UnityEngine;

namespace UI
{
    public class Click : MonoBehaviour
    {
        #region 點擊按鈕 --

        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void ClickSound()
        {
            GetComponent<AudioSource>().Play();
        }

        #endregion
    }
}
