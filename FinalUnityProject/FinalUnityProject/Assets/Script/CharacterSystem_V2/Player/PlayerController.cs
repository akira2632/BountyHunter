using CharacterSystem_V2.InterfaceAndData;
using UnityEngine;

namespace CharacterSystem_V2
{
    /// <summary>
    /// 角色設定與資訊面板、並負責角色的組裝與更新
    /// </summary>
    public class PlayerController : MonoBehaviour, ICharacterControll
    {
        //public KeySetting PlayerKeySetting;
        public PlayerCharacter Character;

        public void Deffend(bool deffend) => Character.Deffend(deffend);
        public void Dodge() => Character.Dodge();
        public void HeavyAttack() => Character.HeavyAttack();
        public void HeavyAttack(bool hold) => Character.HeavyAttack(hold);
        public void LightAttack() => Character.LightAttack();

        public void Move(Vertical direction) => Character.Move(direction);
        public void Move(Horizontal direction) => Character.Move(direction);
        public void OnHit(Damage damage) => Character.OnHit(damage);

        // Start is called before the first frame update
        void Start()
        {
            //GameManager.Instance.SetPlayer(PlayerCharacter);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
                Character.Move(Vertical.Top);
            else if (Input.GetKey(KeyCode.S))
                Character.Move(Vertical.Down);
            else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                Character.Move(Vertical.None);

            if (Input.GetKey(KeyCode.A))
                Character.Move(Horizontal.Left);
            else if (Input.GetKey(KeyCode.D))
                Character.Move(Horizontal.Right);
            else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                Character.Move(Horizontal.None);

            if (Input.GetKeyDown(KeyCode.J))
                Character.LightAttack();

            if (Input.GetKeyDown(KeyCode.Space))
                Character.HeavyAttack();
            if (Input.GetKey(KeyCode.Space))
                Character.HeavyAttack(true);

            if (Input.GetKey(KeyCode.K))
                Character.Deffend(true);
            else if (Input.GetKeyUp(KeyCode.K))
                Character.Deffend(false);
        }
    }

    [CreateAssetMenu(fileName = "按鍵設定", menuName = "賞金獵人_角色系統V2/按鍵設定", order = 1)]
    public class KeySetting : ScriptableObject
    {
        public KeyCode UpKey, DownKey, LeftKey, RightKey, LightAttack, HeavyAttack, Deffend;
    }
}