using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAreaGenerater : IAreaGenerater
{


    public StartAreaGenerater(IBlockBuilder blockBuilder, Coordinate startPoint) : base(blockBuilder, startPoint) { }

    public override List<Coordinate> GenerateArea()
    {
        blockBuilder.MakeBlock(startPoint);

        if(Random.Range(0,1) == 0)

        Random.Range(1, 3);
    }
}
