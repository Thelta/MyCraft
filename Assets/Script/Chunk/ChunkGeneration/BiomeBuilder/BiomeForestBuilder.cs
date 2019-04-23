using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BiomeForestBuilder : BiomeBuilder
{
    public BiomeForestBuilder(Dictionary<Vector2Int, ColumnValues> trunkPositions) : base(trunkPositions) {}
    protected override BiomeType BIOME_TYPE { get { return BiomeType.Jungle; } }
    protected override float STONE_MOUNTAIN_FREQUENCY { get { return 0.001f; } }

    protected override float DIRT_NOISE { get { return 0.004f; } }

    protected override float TREE_FREQUENCY { get { return 0.09f; } }
    protected override float TRUNK_DIST { get { return 0.05f; } }
    protected override int TREE_AREA_MIN_VALUE { get { return 480; } }
    protected override int TRUNK_MAX_HEIGHT { get { return 7; } }
    protected override int LEAF_MAX_HEIGHT { get { return 7; } }

    protected override int BUSH_DENSITY { get { return 9; } }
    protected override int BUSH_MAX_DENSITY { get { return 20; } }

    protected override int CAVE_SIZE { get { return 1; } }

    protected override int ABOVE_SEA_SAND_HEIGHT{ get { return 0; } }
}
