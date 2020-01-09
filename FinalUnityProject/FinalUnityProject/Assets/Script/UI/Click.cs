using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
