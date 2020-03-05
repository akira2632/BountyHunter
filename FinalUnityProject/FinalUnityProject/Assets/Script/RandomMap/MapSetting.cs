using UnityEngine;
using UnityEngine.Tilemaps;

namespace RandomMap
{
    [System.Serializable]
    public class MiniMapSetting
    {
        public Tilemap MiniMap;
        public Tile NormalTile;
        public Tile EntryTile;
        public Tile EntryAreaTile;
        public Tile BossAreaTile;
    }

    [System.Serializable]
    public class GameMapSetting
    {
        public Tilemap GameMapWall;
        public Tilemap GameMapGround;
        public Tilemap GameMapDecorate;

        public Tile TopLeftEntryTile;
        public Tile TopRightEntryTile;
        public Tile BottomLeftEntryTile;
        public Tile BottomRightEntryTile;

        public IsometricRuleTile GameMapWallTile;
        public IsometricRuleTile GameMapGroundTile;
        public IsometricRuleTile GameMapGroundTile_Trigger;

        public Tile[] GroundDecorates;
        public Tile[] WallDecotates;
        public Tile[] BoxDecotates;
        public Tile[] SkullDecotates;
    }

    [System.Serializable]
    public class SpwanPointSetting
    {
        public SpwanPointPrafeb[] SpwanPoints;
        public SpwanPointPrafeb BossSpwanPoint;
    }

    [System.Serializable]
    public struct SpwanPointPrafeb
    {
        public int FromMapDeep, ToMapDeep;
        public GameObject SpwanPoint;
    }
}
