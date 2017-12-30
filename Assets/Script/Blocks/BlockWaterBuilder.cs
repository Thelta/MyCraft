using System.Collections;
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
        return false;
    }

    public override MeshData GreedyDirectionData
    (int x, int y, int z, int width, int height, Direction direction, MeshData meshData)
    {
        if(direction == Direction.up)
        {
            meshData = GreedyFaceGroupDataUp(x, y, z, width, height, meshData, IsSolid(direction));

            int uvTexture = (int)TexturePosition(direction);
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
        }
        else if(direction == Direction.down)
        {
            meshData = GreedyFaceGroupDataDown(x, y, z, width, height, meshData, IsSolid(direction));

            int uvTexture = (int)TexturePosition(direction);
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
            meshData.texType.Add(new Vector2(uvTexture, 0));
        }
        
        return meshData;
    }

    protected override MeshData GreedyFaceGroupDataDown
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y + 1, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + height, y + 1, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + height, y + 1, z + width), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + 1, z + width), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.down, width, height));

        return meshData;
    }

}
