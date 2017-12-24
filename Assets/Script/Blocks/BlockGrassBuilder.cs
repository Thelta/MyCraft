using UnityEngine;
using System.Collections;

public class BlockGrassBuilder : BlockBuilder
{
    public BlockGrassBuilder() : base ()
    {
        type = BlockType.Grass;

    }

    public override TextureType TexturePosition(Direction direction)
    {
        switch(direction)
        {
            case Direction.up:
                return TextureType.grass_top;
            case Direction.down:
                return TextureType.grass_top;
        }

        return TextureType.grass_side;
    }


}
