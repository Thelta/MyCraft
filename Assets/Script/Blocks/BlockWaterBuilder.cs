﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWaterBuilder : BlockBuilder
{
    public BlockWaterBuilder() : base()
    {
        type = BlockType.Water;
    }

    public override TextureType TexturePosition(Direction direction)
    {
        return TextureType.water;
    }

    public override bool IsSolid(Direction direction)
    {
        return direction == Direction.up ? true : false;
    }

    public override MeshData GreedyDirectionData
    (int x, int y, int z, int width, int height, Direction direction, MeshData meshData)
    {
        if(direction == Direction.up)
        {
            meshData = GreedyFaceGroupDataUp(x, y, z, width, height, meshData);

            int uvTexture = (int)TexturePosition(direction);
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));

        }



        return meshData;
    }



}
