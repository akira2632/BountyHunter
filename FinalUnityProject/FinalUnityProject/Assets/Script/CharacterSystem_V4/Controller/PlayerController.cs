﻿using UnityEngine;

namespace CharacterSystem_V4.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public KeySetting PlayerKeySetting;

        public ICharacterActionManager MyCharacter;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(PlayerKeySetting.UpKey))
                MyCharacter.Move(Vertical.Top);
            else if (Input.GetKey(PlayerKeySetting.DownKey))
                MyCharacter.Move(Vertical.Down);

            if (Input.GetKeyUp(PlayerKeySetting.UpKey) || Input.GetKeyUp(PlayerKeySetting.DownKey))
                MyCharacter.Move(Vertical.None);

            if (Input.GetKey(PlayerKeySetting.LeftKey))
                MyCharacter.Move(Horizontal.Left);
            else if (Input.GetKey(PlayerKeySetting.RightKey))
                MyCharacter.Move(Horizontal.Right);

            if (Input.GetKeyUp(PlayerKeySetting.LeftKey) || Input.GetKeyUp(PlayerKeySetting.RightKey))
                MyCharacter.Move(Horizontal.None);

            if (Input.GetKeyDown(PlayerKeySetting.LightAttack))
                MyCharacter.LightAttack();

            if (Input.GetKeyDown(PlayerKeySetting.HeavyAttack))
                MyCharacter.HeavyAttack();
            if (Input.GetKey(PlayerKeySetting.HeavyAttack))
                MyCharacter.HeavyAttack(true);
            if (!Input.GetKey(PlayerKeySetting.HeavyAttack))
                MyCharacter.HeavyAttack(false);

            if (Input.GetKey(PlayerKeySetting.Deffend))
                MyCharacter.Deffend(true);
            else if (Input.GetKeyUp(PlayerKeySetting.Deffend))
                MyCharacter.Deffend(false);
        }
    }
}
