using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
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
