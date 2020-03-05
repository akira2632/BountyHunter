using UnityEngine;

namespace RandomMap
{
    public class MapGenerateManager : MonoBehaviour
    {
        [Header("玩家角色"), Tooltip("重設玩家角色初始位置")]
        public Transform Player;
        public GameObject PlayerController;

        [Header("地圖深度"), Range(5, 100), Tooltip("自起點至最深處的區塊數量")]
        public int MapScale;
        [Header("地圖種子"), Tooltip("0由電腦自動產生種子")]
        public int seed;

        public Grid grid;
        public UI.UIManager UIManager;

        public MiniMapSetting MiniMapSetting = new MiniMapSetting();
        public GameMapSetting GameMapSetting = new GameMapSetting();
        public SpwanPointSetting SpwanPointSetting = new SpwanPointSetting();

        private GeneraterFactry generaterFactry;
        #region 生成頻率管理
        IGenerater generater;
        bool hasInitail;
        int ticks;

        private void Start()
        {
            SetSeed();
            generaterFactry = new GeneraterFactry(
                MiniMapSetting, GameMapSetting, SpwanPointSetting, this);

            //初始化狀態機
            hasInitail = false;
            generater = new AreaGenerateInitail(MapScale, this);

            //Debug.Log("MapGenerateManager Start");
        }


        private void Update()
        {
            ticks = 0;

            while (ticks < 100)
            {
                GeneraterUpdate();
                //Debug.Log("FinalTicks : " + ticks);
            }
        }

        private void GeneraterUpdate()
        {
            if (!hasInitail)
            {
                hasInitail = true;
                generater.Initail();
            }
            else
                generater.Update();
        }

        public void SetNextGenerater(IGenerater nextGenerter)
        {
            generater.End();
            generater = nextGenerter;
            hasInitail = false;
        }

        public void AddTicks(int ticks = 1)
        {
            this.ticks += ticks;
        }
        #endregion

        private void SetSeed()
        {
            if (seed != 0)
                Random.InitState(seed);
            else
                seed = Random.seed;
        }

        internal GeneraterFactry GetGeneraterFactry() => generaterFactry;
        internal void SetPlayerPosition(float x, float y)
        {
            Player.position = new Vector3(x, y);
            Player.gameObject.SetActive(true);
            PlayerController.SetActive(true);
        }
    }
}