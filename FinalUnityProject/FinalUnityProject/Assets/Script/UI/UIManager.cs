using UnityEngine;
using UnityEngine.UI;
using CharacterSystem;
using DG.Tweening;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public bool AutoStart;
        private void Start()
        {
            if(AutoStart)
                LoadCompelete();
        }

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
        public AudioSource BGM;

        public void LoadCompelete()
        {
            BGM.DOFade(1, 1).SetDelay(1);
            ChangeScene.DOFade(0, 1).SetDelay(1).onComplete += 
                () => ActiveMiniMap(true);

            isPause = false;
        }

        /// <summary>
        /// 進入遊戲
        /// </summary>
        public void LoadVillageScene()
        {
            LoadScene("村莊地圖");
        }
        /// <summary>
        /// 回主選單
        /// </summary>
        public void LoadMenuScene()
        {
            LoadScene("Menu");
        }
        public void LoadExitScene()
        {
            LoadScene("Exit");
        }
        public void LoadCaveScene()
        {
            LoadScene("地牢");
        }

        private void LoadScene(string sceneName)
        {
            ActiveMiniMap(false);
            BGM.DOFade(0, 1);
            ChangeScene.DOFade(1, 1).onComplete +=
                () => StartCoroutine(WaitAndLoad(sceneName));
        }

        private System.Collections.IEnumerator WaitAndLoad(string sceneName)
        {
            yield return new WaitForSeconds(1);
            Application.LoadLevel(sceneName);
        }
        #endregion
    }
}