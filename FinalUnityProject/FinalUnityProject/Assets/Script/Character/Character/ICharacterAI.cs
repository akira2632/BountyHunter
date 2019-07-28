using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacterAI
{
    protected ICharacter m_Character = null;
    protected IAIState m_AIState = null;

    public ICharacterAI(ICharacter Character)
    {
        m_Character = Character;

    }

    /// <summary>
    /// 更換 AI 狀態
    /// </summary>
    /// <param name="NewAIState"></param>
    public virtual void ChangeAIState (IAIState NewAIState)
    {
        m_AIState = NewAIState;
        m_AIState.SetCharacterAI(this);
    }

    /// <summary>
    /// 攻擊目標
    /// </summary>
    /// <param name="Target"></param>
    public virtual void Attack( ICharacter Target )
    {
        // 時間到了才攻擊
        m_Character.Attack(Target);
    }

    /// <summary>
    /// 是否在攻擊距離內
    /// </summary>
    /// <param name="Target"></param>
    /// <returns></returns>
    public bool TargetInAttackRange (ICharacter Target)
    {
        return true;
    }

    /// <summary>
    /// 目前位置
    /// </summary>
    /// <returns></returns>
    /*public Vector3 GetPosition()
    {
        return m_Character.GetGameObject().transform.position;
    }*/

    public void MoveTo(Vector3 Position)
    {
        m_Character.MoveTo(Position);
    }

    public void StopMove()
    {
        m_Character.StopMove();
    }

    public void Killed()
    {
        m_Character.Killed();
    }

    public bool IsKilled()
    {
        return m_Character.IsKilled();
    }

    public void RemoveAITarget(ICharacter Target)
    {
        m_AIState.RemoveTarget(Target);
    }



    // Update is called once per frame
    public void Update(List<ICharacter> Targets)
    {

    }

}
