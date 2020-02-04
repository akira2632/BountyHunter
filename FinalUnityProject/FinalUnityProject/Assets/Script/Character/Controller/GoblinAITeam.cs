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

        public float BasicAttackTeamWeight = 1, SpacilAttackTeamWeight = 2;

        [SerializeField]
        private List<GoblinAIController> AITeam = new List<GoblinAIController>();
        private float basicAttackMemberCount, spacilAttackMemberCount;

        public void AddToTeam(GoblinAIController goblinAI)
        {
            if (!AITeam.Contains(goblinAI))
                AITeam.Add(goblinAI);

            SelectTeam(goblinAI);
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

            foreach (GoblinAIController member in AITeam)
                SelectTeam(member);
        }

        private void SelectTeam(GoblinAIController member)
        {
            if (basicAttackMemberCount / BasicAttackTeamWeight
                < spacilAttackMemberCount / SpacilAttackTeamWeight)
                SwitchToBasicTeam(member);
            else
                SwitchToSpacilTeam(member);
        }

        private void SwitchToBasicTeam(GoblinAIController member)
        {
            if (member.MemberType == MemberType.BasicAttacker)
                return;

            if (member.MemberType == MemberType.SpacilAttacker)
                spacilAttackMemberCount--;

            member.MemberType = MemberType.BasicAttacker;
            basicAttackMemberCount++;
        }

        private void SwitchToSpacilTeam(GoblinAIController member)
        {
            if (member.MemberType == MemberType.SpacilAttacker)
                return;

            if (member.MemberType == MemberType.BasicAttacker)
                basicAttackMemberCount--;

            member.MemberType = MemberType.SpacilAttacker;
            spacilAttackMemberCount++;
        }
    }
}
