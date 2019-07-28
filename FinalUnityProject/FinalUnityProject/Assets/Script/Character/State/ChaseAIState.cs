using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAIState : IAIState
{
    private ICharacter m_ChaseTarget = null;
    
    public ChaseAIState(ICharacter ChaseTarget)
    {
        m_ChaseTarget = ChaseTarget;
    }

    
    // Update is called once per frame
    public override void Update(List<ICharacter> Targets)
    {
        
    }

    public override void RemoveTarget(ICharacter Target)
    {

    }
}
