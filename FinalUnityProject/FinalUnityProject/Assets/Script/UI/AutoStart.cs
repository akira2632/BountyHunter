using UnityEngine;

namespace UI
{
    public class AutoStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<UIManager>().LoadCompelete();
        }
    }
}