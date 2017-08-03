using UnityEngine;
using System.Collections;

public class BlockAirBuilder : BlockBuilder
{
    public BlockAirBuilder() : base()
    {
        type = BlockType.Air;
    }

    public override MeshData GreedyDirectionData
        (int x, int y, int z, int width, int height, Direction direction, MeshData meshData)
    { 
        return meshData;
    }


    public override bool IsSolid(Direction direction)
    {
        return false;
    }



}
