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
                return TextureType.GrassUp;
            case Direction.down:
                return TextureType.GrassDown;
        }

        return TextureType.GrassSurround;
    }


}
