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
        return false;
    }
}