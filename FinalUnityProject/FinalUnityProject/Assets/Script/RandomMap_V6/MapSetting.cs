using System.Collections.Generic;
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
        public Tilemap GameMap_Decorate;

        public Tile TopLeftEntry;
        public Tile TopRightEntry;
        public Tile BottomLeftEntry;
        public Tile BottomRightEntry;

        public IsometricRuleTile GameMapWall;
        public IsometricRuleTile GameMapGround;

        public Tile[] GroundDecorates;
        public Tile[] WallDecotates;
        public Tile[] BoxDecotates;
        public Tile[] SkullDecotates;

        public Tilemap NavegateMap;
        public Tile NavegateBlock;
    }

    [System.Serializable]
    public class SpwanPointSetting
    {
        public SpwanPointPrafeb[] SpwanPoints;
    }

    [System.Serializable]
    public struct SpwanPointPrafeb
    {
        public int FromMapDeep, ToMapDeep;
        public GameObject SpwanPoint;
    }
}
