using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDeadbushBuilder : BlockBushBuilder
{
    public BlockDeadbushBuilder() : base()
    {
        type = BlockType.Deadbush;
    }

    public override TextureType TexturePosition(Direction direction)
    {
        return TextureType.deadbush;
    }

    public override bool OVERRIDE_GREEDY_MESHER_RENDERING
    {
        get { return true; }
    }
}
