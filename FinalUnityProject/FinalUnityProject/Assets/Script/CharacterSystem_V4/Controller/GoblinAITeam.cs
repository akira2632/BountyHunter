using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem.Controller
{
    public enum MemberType
    {
        None, BasicAttacker, SpacilAttacker
    }

    public class GoblinAITeam : MonoBehaviour
    {
        #region Singleton
        private static GoblinAITeam _instace;

        public static GoblinAITeam Instance
        {
            get
            {
                if (_instace != null)
                    return _instace;

                _instace = FindObjectOfType<GoblinAITeam>();

                if (_instace == null)
                    _instace = new GameObject("GoblinTeam").AddComponent<GoblinAITeam>();

                return _instace;
            }
        }

        private void Awake()
        {
            if (_instace == null)
                _instace = this;

            if (_instace != this)
                Destroy(gameObject);
        }
        #endregion

        public int BasicAttackTeamWeight = 1, SpacilAttackTeamWeight = 2;

        [SerializeField]
        private List<GoblinAIController> AITeam = new List<GoblinAIController>();
        private int basicAttackMemberCount, spacilAttackMemberCount;

        public void AddToTeam(GoblinAIController goblinAI)
        {
            if (!AITeam.Contains(goblinAI))
                AITeam.Add(goblinAI);
        }

        public void RemoveFromTeam(GoblinAIController goblinAI)
        {
            if (AITeam.Contains(goblinAI))
            {
                if (goblinAI.MemberType == MemberType.BasicAttacker)
                    basicAttackMemberCount--;

                if (goblinAI.MemberType == MemberType.SpacilAttacker)
                    spacilAttackMemberCount--;

                AITeam.Remove(goblinAI);
            }
        }

        private void Update()
        {
            foreach (GoblinAIController member in AITeam)
            {
                if (member.MemberType != MemberType.BasicAttacker
                    && basicAttackMemberCount * BasicAttackTeamWeight
                    <= spacilAttackMemberCount * SpacilAttackTeamWeight)
                    SwitchToBasicTeam(member);

                if (member.MemberType != MemberType.SpacilAttacker
                    && basicAttackMemberCount > 0
                    && basicAttackMemberCount * BasicAttackTeamWeight
                    <= spacilAttackMemberCount * SpacilAttackTeamWeight)
                    SwitchToSpacilTeam(member);
            }
        }

        private void SwitchToBasicTeam(GoblinAIController member)
        {
            member.MemberType = MemberType.BasicAttacker;
            basicAttackMemberCount++;
            if (member.MemberType == MemberType.SpacilAttacker)
                spacilAttackMemberCount--;
        }

        private void SwitchToSpacilTeam(GoblinAIController member)
        {
            member.MemberType = MemberType.SpacilAttacker;
            spacilAttackMemberCount++;
            if (member.MemberType == MemberType.BasicAttacker)
                basicAttackMemberCount--;
        }
    }
}
