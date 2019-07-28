using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FledAIState : IAIState
{
    public override void Update(List<ICharacter> Targets)
    {
        m_CharacterAI.ChangeAIState(new IdleAIState());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
