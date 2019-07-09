using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAreaGenerater : IAreaGenerater
{
    public StartAreaGenerater(IBlockBuilder blockBuilder, Coordinate startPoint) : base(blockBuilder, startPoint) { }

    public override List<Coordinate> GenerateArea()
    {
        throw new System.NotImplementedException();
    }
}
