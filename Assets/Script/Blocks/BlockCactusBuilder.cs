using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCactusBuilder : BlockBuilder
{
    const float POS_OFFSET_MIN = 0.0625f;
    const float POS_OFFSET_MAX = 0.9375f;

    public BlockCactusBuilder()
    {
        type = BlockType.Cactus;
    }

    public override TextureType TexturePosition(Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                return TextureType.cactus_top;
            case Direction.down:
                return TextureType.cactus_bottom;
        }
        return TextureType.cactus_side;
    }

    public override bool OVERRIDE_GREEDY_MESHER_RENDERING
    {
        get { return true; }
    }

    protected override MeshData GreedyFaceGroupDataNorth
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + width, y, z + POS_OFFSET_MAX), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y + height, z + POS_OFFSET_MAX), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + height, z + POS_OFFSET_MAX), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y, z + POS_OFFSET_MAX), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.north, width, height));

        return meshData;
    }


    protected override MeshData GreedyFaceGroupDataEast
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MAX, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MAX, y + width, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MAX, y + width, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MAX, y, z + height), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.east, height, width));

        return meshData;
    }


    protected override MeshData GreedyFaceGroupDataSouth
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y, z + POS_OFFSET_MIN), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + height, z + POS_OFFSET_MIN), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y + height, z + POS_OFFSET_MIN), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y, z + POS_OFFSET_MIN), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.south, width, height));

        return meshData;
    }


    protected override MeshData GreedyFaceGroupDataWest
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MIN, y, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MIN, y + width, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MIN, y + width, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + POS_OFFSET_MIN, y, z), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.west, height, width));

        return meshData;
    }

}
