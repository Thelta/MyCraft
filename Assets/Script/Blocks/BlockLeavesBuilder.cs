using UnityEngine;
using System.Collections;
using System;

public class BlockLeavesBuilder : BlockBuilder
{
    public BlockLeavesBuilder() : base()
    {
        type = BlockType.Leaves;

    }
    public override Tile TexturePosition(Direction direction)
    {
        Tile tile = new Tile();
        tile.x = 0;
        tile.y = 1;
        return tile;
    }
    public override bool IsSolid(Direction direction)
    {
        return false;
    }
}