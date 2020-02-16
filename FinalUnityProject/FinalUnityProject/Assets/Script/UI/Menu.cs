using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        //[Header("遊戲按紐群組")]
        //public GameObject AllBtn;

        #region 建立新遊戲 --

        [Header("建立新遊戲按紐")]
        public Button NewGameButton;

        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void GoIntoGame()
        {
            Application.LoadLevel("村莊地圖");
        }

        #endregion


        #region 遊戲操作 --

        [Header("遊戲操作按紐")]
        public Button gameControll;

        [Header("遊戲操作介面")]
        public GameObject gameControllUI;

        /// <summary>
        /// 開啟選單
        /// </summary>
        public void OpenGameControll()
        {
            gameControllUI.SetActive(true);
        }

        /// <summary>
        /// 開啟選單
        /// </summary>
        public void CloseGameControll()
        {
            gameControllUI.SetActive(false);
        }



        #endregion

        #region 結束遊戲 --

        [Header("結束遊戲按紐")]
        public Button ExitButton;
        [Header("結束遊戲判斷")]
        bool isExit = false;

        /// <summary>
        /// 按下離開按紐
        /// </summary>
        public void ExitOnClick()
        {
            if (!isExit)
            {
                isExit = true;
                GetComponent<Animation>().Play("FadeOut_EndGame");
            }
        }

        public void GoExit()
        {
            Application.LoadLevel("Exit");
        }

        public void Quit()
        {
            Application.Quit();
        }

        #endregion


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