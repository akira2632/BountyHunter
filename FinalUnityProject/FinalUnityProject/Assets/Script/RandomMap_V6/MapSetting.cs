using UnityEngine;
using UnityEngine.Tilemaps;

namespace RandomMap_V6
{
    [System.Serializable]
    public class MiniMapSetting
    {
        public Tilemap MiniMap;
        public Tile MiniMapWall;
    }

    [System.Serializable]
    public class GameMapSetting
    {
        public Tilemap GameMap_Wall;
        public Tilemap GameMap_Ground;
        public Tilemap GameMap_Collider;
        public IsometricRuleTile GameMapWall;
        public IsometricRuleTile GameMapGround;

        public Tilemap NavegateMap;
        public Tile NavegateBlock;
    }
}
