using UnityEngine;
using UnityEngine.UI;
using CharacterSystem;
using DG.Tweening;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private void Update()
        {
            UpdateMiniMap();
            UpdateCharacterUI();
        }

        #region 角色UI更新
        public Image BloodBar;
        public Image BloodMargin;
        public Text BloodValue;
        public CharacterActionController Character;

        private void UpdateCharacterUI()
        {
            BloodBar.fillAmount = (float)Character.CharacterData.Health / (float)Character.CharacterData.MaxHealth;

            BloodMargin.fillAmount = BloodBar.fillAmount;

            BloodValue.text = Character.CharacterData.Health + " / " + Character.CharacterData.MaxHealth;
        }
        #endregion

        #region 小地圖控制
        public RawImage MiniMap;
        public Animator MiniMapAnimator;
        private bool isShow = false;

        private void UpdateMiniMap()
        {
            if (!isPause && Input.GetKeyDown(KeyCode.Tab))
                ShowMiniMap(!isShow);
        }

        private void ShowMiniMap(bool show)
        {
            if (MiniMap == null)
                return;

            isShow = show;
            MiniMapAnimator.SetBool("ShowMiniMap", show);
        }
        private void ActiveMiniMap(bool active)
        {
            if (MiniMap != null)
                MiniMap.gameObject.SetActive(active);
        }
        #endregion

        private bool isPause = true;
        public void PauseGame(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0;
                isPause = true;
                ShowMiniMap(false);
            }
            else
            {
                Time.timeScale = 1;
                isPause = false;
            }
        }

        #region 轉場控制
        public Image ChangeScene;
        public void LoadCompelete()
        {
            ChangeScene.DOFade(0, 1).SetDelay(1).onComplete += 
                () => ActiveMiniMap(true);

            isPause = false;
        }

        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void RestartGame()
        {
            ActiveMiniMap(false);
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad("村莊地圖"));
        }
        /// <summary>
        /// 回主選單
        /// </summary>
        public void GoBackToMenu()
        {
            ActiveMiniMap(false);
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad("Menu"));
        }
        public void GoExit()
        {
            ActiveMiniMap(false);
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad("Exit"));
        }

        private System.Collections.IEnumerator WaitAndLoad(string sceneName)
        {
            yield return new WaitForSeconds(1);
            Application.LoadLevel(sceneName);
        }
        #endregion
    }
}