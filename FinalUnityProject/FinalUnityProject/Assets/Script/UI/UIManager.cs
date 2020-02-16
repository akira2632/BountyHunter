using UnityEngine;
using UnityEngine.UI;
using CharacterSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        #region 角色UI更新
        public Image BloodBar;

        public Image BloodMargin;

        public Text BloodValue;

        public CharacterActionController character;

        private void Update()
        {
            UpdateCharacterUI();
        }

        private void UpdateCharacterUI()
        {
            BloodBar.fillAmount = character.CharacterData.Health / character.CharacterData.MaxHealth;

            BloodMargin.fillAmount = BloodBar.fillAmount;

            BloodValue.text = character.CharacterData.Health + " / " + character.CharacterData.MaxHealth;
        }
        #endregion

        #region PauseGame
        public void PauseGame(bool pause)
        {
            if (pause)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
        #endregion

        #region 重新開始
        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void RestartGame()
        {
            Application.LoadLevel("村莊地圖");
        }
        #endregion

        #region 回主選單
        /// <summary>
        /// 回主選單
        /// </summary>
        public void GoBackToMenu()
        {
            Application.LoadLevel("Menu");
        }
        #endregion

        #region 結束遊戲
        public void GoExit()
        {
            Application.LoadLevel("Exit");
        }
        #endregion
    }
}