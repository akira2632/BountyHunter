using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CharacterSystem_V4;

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

    public int BloodHP;

    public ICharacterActionManager character;
    #endregion

    private void Update()
    {
        float bloodHp = character.RunTimeData.Health;

        //BloodHP = Mathf.Clamp(BloodHP, 0, 200);

        BloodBar.fillAmount = bloodHp / 200;

        BloodMargin.fillAmount = BloodBar.fillAmount;

        BloodValue.text = (int)bloodHp + " / 200";
    }

}
