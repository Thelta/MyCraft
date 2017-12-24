using UnityEngine;
using System.Collections;
using System;

public class BlockWoodBuilder : BlockBuilder
{
    public BlockWoodBuilder(): base()
    {
        type = BlockType.Wood;
    }
    public override TextureType TexturePosition(Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                return TextureType.log_oak_top;
            case Direction.down:
                return TextureType.log_oak_top;
        }
        return TextureType.log_oak;
    }
}