using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBushBuilder : BlockBuilder
{
    public BlockBushBuilder(): base()
    {
        type = BlockType.Bush;
    }

    public override TextureType TexturePosition(Direction direction)
    {
        return TextureType.tallgrass;
    }

    public override bool IsSolid(Direction direction)
    {
        return false;
    }

    public override bool OVERRIDE_GREEDY_MESHER_RENDERING
    {
        get { return true; }
    }

    public override MeshData GreedyDirectionData
    (int x, int y, int z, int width, int height, Direction direction, MeshData meshData, BiomeType biome)
    {
        int uvTexture = (int)TexturePosition(direction);

        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                switch (direction)
                {
                    case Direction.north:
                        meshData = GreedyFaceGroupDataNorth(x + j, y + i, z, 1, 1, meshData, IsSolid(direction));
                        meshData.AddUV2Info(uvTexture, (int)biome);
                        break;
                    case Direction.east:
                        meshData = GreedyFaceGroupDataEast(x, y + j, z + i, 1, 1, meshData, IsSolid(direction));
                        meshData.AddUV2Info(uvTexture, (int)biome);
                        break;
                    case Direction.south:
                        meshData = GreedyFaceGroupDataSouth(x + j, y + i, z, 1, 1, meshData, IsSolid(direction));
                        meshData.AddUV2Info(uvTexture, (int)biome);
                        break;
                    case Direction.west:
                        meshData = GreedyFaceGroupDataWest(x, y + j, z + i, 1, 1, meshData, IsSolid(direction));
                        meshData.AddUV2Info(uvTexture, (int)biome);
                        break;
                }
                
            }
        }

        //GFDDArray[(int)direction](x, y, z, width, height, meshData);

        return meshData;
    }

    protected override MeshData GreedyFaceGroupDataNorth
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + width, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y + height, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + height, z + 1), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y, z + 1), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.north, width, height));

        return meshData;
    }

    protected override MeshData GreedyFaceGroupDataSouth
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y, z + 1), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + height, z + 1), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y + height, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y, z), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.south, width, height));

        return meshData;
    }



    protected override MeshData GreedyFaceGroupDataEast
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + width, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + 1, y + width, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + 1, y, z + height), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.east, height, width));

        return meshData;
    }




    protected override MeshData GreedyFaceGroupDataWest
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + 1, y, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + 1, y + width, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + width, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y, z), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.west, height, width));

        return meshData;
    }



}
