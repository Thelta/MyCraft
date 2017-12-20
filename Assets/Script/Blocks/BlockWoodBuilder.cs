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
                return TextureType.WoodY;
            case Direction.down:
                return TextureType.WoodY;
        }
        return TextureType.WoodSurround;
    }
}