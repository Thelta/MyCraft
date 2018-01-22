using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSandBuilder : BlockBuilder
{
    public BlockSandBuilder(): base()
    {
        type = BlockType.Sand;
    }

    public override TextureType TexturePosition(Direction direction)
    {
        return TextureType.sand;
    }


}
