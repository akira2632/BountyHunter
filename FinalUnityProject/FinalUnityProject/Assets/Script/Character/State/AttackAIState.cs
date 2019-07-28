using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAIState : IAIState
{
    private ICharacter m_AttackTarget = null; //

    private AttackAIState(ICharacter AttackTarget)
    {
        m_AttackTarget = AttackTarget;
    }

    // Update is called once per frame
    public override void Update(List<ICharacter> Targets)
    {
        m_CharacterAI.ChangeAIState(new IdleAIState());
        m_CharacterAI.Attack(m_AttackTarget);
    }

    public override void RemoveTarget(ICharacter Target)
    {

    }
}
