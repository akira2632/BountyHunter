using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem
{
    public class OrcCaptainActionProvider : ICharacterActionProvider
    {
        public AudioClip MoveSound;
        public Skill.HitEffect DefaultHitEffect;

        public override ICharacterAction GetDeadAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }

        public override ICharacterAction GetFallDownAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }

        public override ICharacterAction GetIdelAction(CharacterActionController manager)
        {
            throw new System.NotImplementedException();
        }
    }
}
