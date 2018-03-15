using UnityEngine;
using System.Collections;
using System;

public class BlockLeavesBuilder : BlockBuilder
{
    public BlockLeavesBuilder() : base()
    {
        type = BlockType.Leaves;
    }
    public override TextureType TexturePosition(Direction direction)
    {
        return TextureType.leaves_oak;
    }
    public override bool IsSolid(Direction direction)
    {
        return true;
    }

    public override bool OVERRIDE_GREEDY_MESHER_RENDERING
    {
        get { return true; }
    }
}