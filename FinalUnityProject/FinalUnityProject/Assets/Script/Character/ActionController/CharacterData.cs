using UnityEngine;

namespace Character
{
    public class CharacterData : MonoBehaviour , ICharacterData, ICharacterProperty
    {
        public CharacterRunTimeData RunTimeData;
        public IScriptableCharacterProperty Property;

        private void Start()
        {
            RunTimeData = new CharacterRunTimeData();
            RunTimeData.SetData(Property, transform);
        }

        public float MoveSpeed => Property.MoveSpeed;

        public float DodgeSpeed => Property.DodgeSpeed;

        public int MaxHealth => Property.MaxHealth;

        public float RegenSpeed => Property.RegenSpeed;

        public int RegenHealth => Property.RegenHealth;

        public float BasicAttackSpeed => Property.BasicAttackSpeed;

        public float SpacilAttackSpeed => Property.SpacilAttackSpeed;

        public int Attack => Property.Attack;

        public int AttackFloating => Property.AttackFloating;

        public float CriticalMagnifiction => Property.CriticalMagnifiction;

        public float CriticalRate => Property.CriticalRate;

        public Vector2 Direction { get => RunTimeData.Direction; set => RunTimeData.Direction = value; }
        public Vector3 TargetPosition { get => RunTimeData.TargetPosition; set => RunTimeData.TargetPosition = value; }
        public float BasicAttackTimer { get => RunTimeData.BasicAttackTimer; set => RunTimeData.BasicAttackTimer = value; }
        public float SpacilAttackTimer { get => RunTimeData.SpacilAttackTimer; set => RunTimeData.SpacilAttackTimer = value; }
        public float RegenTimer { get => RunTimeData.RegenTimer; set => RunTimeData.RegenTimer = value; }
        public int Health { get => RunTimeData.Health; set => RunTimeData.Health = value; }
        public float VertigoConter { get => RunTimeData.VertigoConter; set => RunTimeData.VertigoConter = value; }

        public void SetData(ICharacterProperty characterProperty, Transform characterTransform)
        {
            RunTimeData.SetData(characterProperty, characterTransform);
        }

        public void Update()
        {
            RunTimeData.Update();
        }
    }
}