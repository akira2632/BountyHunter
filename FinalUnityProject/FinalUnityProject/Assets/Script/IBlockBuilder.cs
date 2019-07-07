using UnityEngine;

namespace MapGenerater
{
    //區塊建造者介面
    interface IBlockBuilder
    {
        bool HasCompleteBlock(Coordinate coordinate);
        bool HasBoundary(Coordinate coordinate, Direction direction);

        void ConectBoundary(Coordinate coordinate, Direction direction);

        void MakeWall(Coordinate coordinate, Direction direction);
        void MakePath(Coordinate coordinate, Direction direction);
        void MakeBlock(Coordinate coordinate);
    }
}