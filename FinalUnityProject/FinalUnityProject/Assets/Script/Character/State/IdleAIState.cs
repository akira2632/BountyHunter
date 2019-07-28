using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAIState : IAIState
{
    bool m_SetAttPosition = false;       // 是否設定攻擊目標

    public IdleAIState() { }

    // Update is called once per frame
    public override void SetAttackPosition ( Vector3 Att_pos )
    {
        base.SetAttackPosition (Att_pos);
        m_SetAttPosition = true;
    }
    
    public override void Update ( List<ICharacter> Targets )
    {

    }
}
