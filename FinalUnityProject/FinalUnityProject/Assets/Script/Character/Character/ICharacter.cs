using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacter : MonoBehaviour
{
    protected ICharacterAI m_AI = null;
    protected IAIState m_AIState = null;

    public void SetAI (ICharacterAI CharacterAI)
    {
        m_AI = CharacterAI;
    }


    // Update is called once per frame
    public void UpdateAI(List<ICharacter> Targets)
    {
        m_AI.Update(Targets);
    }

    public void RemoveAITarget(ICharacter Targets)
    {
        m_AI.RemoveAITarget(Targets);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="Target"></param>
    public void Attack (ICharacter Target)
    {

    }

    public class CharacterAttribute : ScriptableObject
    {
        [Header("角色名稱")]
        public string Name;

        [Header("體型")]
        public string BodyType;

        [Header("血量")]
        public float HP;

        [Header("攻擊數值")]
        public float Attack;

        [Header("防禦數值")]
        public float Defence;

        [Header("移動速度")]
        public float MoveSpeed;

        [Header("攻擊速度")]
        public float AttackSpeed;

        [Header("攻擊距離")]
        public float AttackRange;


    }

    public class CharacterData : ScriptableObject
    {
        public void Attack(ICharacter Target)
        {

        }
         
    }
}
