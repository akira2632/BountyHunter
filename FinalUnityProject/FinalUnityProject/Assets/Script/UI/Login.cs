using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour
{
    bool isMusicFadeOut = false;

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            GetComponent<Animation>().Play("FadeOut_Login");
            GoToMenuSound.Play();
            

        }

        if (isMusicFadeOut)
        {
            LogoSound.volume -= 0.00075f;
            GoToMenuSound.volume -= 0.0005f; 
        }
    }
    
    public void GoToGameMenu()
    {
        Application.LoadLevel("Menu");
    }

    [Header("標題按下確定聲音")]
    public AudioSource LogoSound;

    [Header("標題按下確定聲音")]
    public AudioSource GoToMenuSound;

    public void MusicFadeOut()
    {
        isMusicFadeOut = true;
    }


}
