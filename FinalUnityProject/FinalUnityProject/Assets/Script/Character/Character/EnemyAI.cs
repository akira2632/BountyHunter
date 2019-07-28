using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : ICharacterAI
{
    private Vector3 m_AttackPosition = Vector3.zero;

    public EnemyAI (ICharacter Character, Vector3 AttackPosition) : base(Character)
    {

    }
    
    public override void ChangeAIState (IAIState NewAIState)
    {
        ChangeAIState(NewAIState);
        NewAIState.SetAttackPosition(m_AttackPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
