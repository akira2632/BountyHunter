using UnityEngine;
using UnityEngine.UI;
using CharacterSystem;
using DG.Tweening;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        #region 角色UI更新
        public Image BloodBar;
        public Image BloodMargin;
        public Text BloodValue;
        public CharacterActionController Character;

        public Image ChangeScene;

        private void Update()
        {
            UpdateCharacterUI();
        }

        private void UpdateCharacterUI()
        {
            BloodBar.fillAmount = (float)Character.CharacterData.Health / (float)Character.CharacterData.MaxHealth;

            BloodMargin.fillAmount = BloodBar.fillAmount;

            BloodValue.text = Character.CharacterData.Health + " / " + Character.CharacterData.MaxHealth;
        }
        #endregion

        public void LoadCompelete()
        {
            ChangeScene.DOFade(0, 1).SetDelay(1);
        }

        public void PauseGame(bool pause)
        {
            if (pause)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void RestartGame()
        {
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad("村莊地圖"));
        }
        /// <summary>
        /// 回主選單
        /// </summary>
        public void GoBackToMenu()
        {
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad("Menu"));
        }
        public void GoExit()
        {
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad("Exit"));
        }

        private System.Collections.IEnumerator WaitAndLoad(string sceneName)
        {
            yield return new WaitForSeconds(1);
            Application.LoadLevel(sceneName);
        }
    }
}