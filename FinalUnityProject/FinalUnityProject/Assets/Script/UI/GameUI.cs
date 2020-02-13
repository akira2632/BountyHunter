﻿using CharacterSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        #region 開啟選單 --
        [Header("開啟選單按紐")]
        public Button gamePlayMenuBtn;

        [Header("選單物件設定")]
        public GameObject MenuUI;

        /// <summary>
        /// 開啟選單
        /// </summary>
        public void OpenMenu()
        {
            MenuUI.SetActive(true);
        }
        #endregion

        #region 開啟角色數值介面 --
        [Header("開啟角色數值介面按紐")]
        public Button characterAttBtn;

        [Header("角色數值介面物件設定")]
        public GameObject CharacterAtt;

        [Header("開啟角色數值介面按紐物件")]
        public GameObject OpenCharacterAttBtn;

        [Header("收起角色數值介面按紐物件")]
        public GameObject CloseCharacterAttBtn;

        [Header("角色數值介面顯示與否")]
        bool isOpenCharacterAtt = false;

        /// <summary>
        /// 開關角色數值介面
        /// </summary>
        public void OpenCharacterAtt()
        {
            isOpenCharacterAtt = !isOpenCharacterAtt;

            CharacterAtt.SetActive(isOpenCharacterAtt);

            OpenCharacterAttBtn.SetActive(false);
            CloseCharacterAttBtn.SetActive(true);
        }

        public void CloseCharacterAtt()
        {
            isOpenCharacterAtt = !isOpenCharacterAtt;

            CharacterAtt.SetActive(isOpenCharacterAtt);

            OpenCharacterAttBtn.SetActive(true);
            CloseCharacterAttBtn.SetActive(false);
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

        #region
        public Image BloodBar;

        public Image BloodMargin;

        public Text BloodValue;

        public CharacterActionController character;
        #endregion

        private void Update()
        {
            BloodBar.fillAmount = character.CharacterData.Health / character.CharacterData.MaxHealth;

            BloodMargin.fillAmount = BloodBar.fillAmount;

            BloodValue.text = character.CharacterData.Health + " / " + character.CharacterData.MaxHealth;
        }

    }
}
