using UnityEngine;

namespace UI
{
    public class Exit : MonoBehaviour
    {
        public void Update()
        {
            if (Input.anyKeyDown)
            {
                GetComponent<Animation>().Play("FadeOut_Exit");
            }
        }

        public void ReadyToLeave()
        {
            GameObject.Find("離開指令").GetComponent<Animation>().Play("Twinkle");

        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
