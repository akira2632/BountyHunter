using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePlayMenu : MonoBehaviour
    {
        #region 繼續遊戲 --

        [Header("繼續遊戲按紐")]
        public Button continueButton;

        /// <summary>
        /// 關閉選單返回遊戲
        /// </summary>
        public void ContinueGame()
        {
            GameObject.Find("GamePlayMenu").SetActive(false);
        }

        #endregion

        #region 重新開始 --

        [Header("重新開始按紐")]
        public Button RestartButton;

        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void RestartGame()
        {
            Application.LoadLevel("村莊地圖");
        }

        #endregion

        #region 回主選單 --

        [Header("回主選單按紐")]
        public Button goBackToMenuButton;

        /// <summary>
        /// 回主選單
        /// </summary>
        public void GoBackToMenu()
        {
            Application.LoadLevel("Menu");
        }

        #endregion

        #region 儲存進度 --

        [Header("儲存進度按紐")]
        public Button SaveGameButton;



        #endregion

        #region 載入進度 --

        [Header("載入進度按紐")]
        public Button LoadGameButton;



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
        bool isExit;

        /// <summary>
        /// 按下離開按紐
        /// </summary>
        public void ExitOnClick()
        {
            isExit = true;
        }

        public void GoExit()
        {
            Application.LoadLevel("Exit");
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