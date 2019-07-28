using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAIState : IAIState
{
    bool m_SetAttackPosition = false;

    public WanderAIState() { }

    // Update is called once per frame
    public override void SetAttackPosition(Vector3 Att_pos)
    {
        base.SetAttackPosition(Att_pos);
        m_SetAttPosition = true;
    }

    public override void Update(List<ICharacter> Targets)
    {

    }
}
