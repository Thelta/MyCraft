using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public class BiomeDesertBuilder : BiomeBuilder
{
    protected override float DIRT_NOISE { get { return 0.01f; } }
    protected override int BUSH_DENSITY { get { return 9; } }
    int CACTUS_DENSITY { get { return 5; } }

    protected override BlockType SECOND_LAND_LAYER_BLOCK { get { return BlockType.Sand; } }
    protected override BlockType BUSH_BLOCK { get { return BlockType.Deadbush; }}
    protected override BlockType TREE_TRUNK_BLOCK { get { return BlockType.Cactus; } }

    [MethodImplAttribute(256)]
    protected override BlockType CreateGreenery(int dirtHeight, int x, int y, int z)
    {
        int greenValue = GetNoise(terraNoise, BUSH_FREQUENCY, 40, x, dirtHeight + 1, z);
        if (y <= dirtHeight + greenValue  && greenValue < CACTUS_DENSITY)
        {
            return TREE_TRUNK_BLOCK;
        }

        if (dirtHeight == y - 1 && greenValue < BUSH_DENSITY)
        {
            return BUSH_BLOCK;
        }

        return BlockType.Air;
    }
}
