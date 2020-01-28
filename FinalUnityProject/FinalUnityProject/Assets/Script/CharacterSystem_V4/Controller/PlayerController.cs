using UnityEngine;

namespace CharacterSystem.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public KeySetting PlayerKeySetting;
        public ICharacterActionManager MyCharacter;

        [SerializeField]
        private Vector2 direction;

        private void Start()
        {
            direction = new Vector2();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(PlayerKeySetting.UpKey))
                direction.y = 1;
            else if (Input.GetKey(PlayerKeySetting.DownKey))
                direction.y = -1;

            if (Input.GetKeyUp(PlayerKeySetting.UpKey) || Input.GetKeyUp(PlayerKeySetting.DownKey))
                direction.y = 0;

            if (Input.GetKey(PlayerKeySetting.LeftKey))
                direction.x = -1;
            else if (Input.GetKey(PlayerKeySetting.RightKey))
                direction.x = 1;

            if (Input.GetKeyUp(PlayerKeySetting.LeftKey) || Input.GetKeyUp(PlayerKeySetting.RightKey))
                direction.x = 0;

            MyCharacter.Move(direction.normalized);

            if (Input.GetKeyDown(PlayerKeySetting.LightAttack))
                MyCharacter.BasicAttack();

            if (Input.GetKeyDown(PlayerKeySetting.HeavyAttack))
                MyCharacter.SpecialAttack();
            if (Input.GetKey(PlayerKeySetting.HeavyAttack))
                MyCharacter.SpecialAttack(true);
            if (!Input.GetKey(PlayerKeySetting.HeavyAttack))
                MyCharacter.SpecialAttack(false);

            if (Input.GetKey(PlayerKeySetting.Deffend))
                MyCharacter.Deffend(true);
            else if (Input.GetKeyUp(PlayerKeySetting.Deffend))
                MyCharacter.Deffend(false);
        }
    }
}
