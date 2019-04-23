using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class BlockBuilder
{    
    public BlockType type;

    public virtual bool OVERRIDE_GREEDY_MESHER_RENDERING { get { return false; } }
    public virtual bool OVERRIDE_OFFSET { get { return false; } }

    public BlockBuilder()
    {
        type = BlockType.Rock;
    }

    public virtual TextureType TexturePosition(Direction direction)
    {
        return TextureType.cobblestone;
    }

    public virtual Vector2[] FaceUVs(Direction direction, int width, int height)
    {
        Vector2[] UVs = new Vector2[4];

        UVs[3] = new Vector2(0, 0);
        UVs[0] = new Vector2(width, 0);
        UVs[2] = new Vector2(0,  height);
        UVs[1] = new Vector2(width, height);

        return UVs;
    }

    public virtual bool IsSolid(Direction direction)
    {
        switch (direction)  
        {
            case Direction.north:
                return true;
            case Direction.east:
                return true;
            case Direction.south:
                return true;
            case Direction.west:
                return true;
            case Direction.up:
                return true;
            case Direction.down:
                return true;
        }

        return false;
    }

    public virtual MeshData GreedyDirectionData
        (int x, int y, int z, int width, int height, Direction direction, MeshData meshData, BiomeType biome)
    {
        switch (direction)
        {
            case Direction.north:
                meshData = GreedyFaceGroupDataNorth(x, y, z, width, height, meshData, IsSolid(direction));
                break;
            case Direction.east:
                meshData = GreedyFaceGroupDataEast(x, y, z, width, height, meshData, IsSolid(direction));
                break;
            case Direction.south:
                meshData = GreedyFaceGroupDataSouth(x, y, z, width, height, meshData, IsSolid(direction));
                break;
            case Direction.west:
                meshData = GreedyFaceGroupDataWest(x, y, z, width, height, meshData, IsSolid(direction));
                break;
            case Direction.up:
                meshData = GreedyFaceGroupDataUp(x, y, z, width, height, meshData, IsSolid(direction));
                break;
            case Direction.down:
                meshData = GreedyFaceGroupDataDown(x, y, z, width, height, meshData, IsSolid(direction));
                break;
        }

        //add texture index to uv2
        int uvTexture = (int)TexturePosition(direction);
        meshData.AddUV2Info(uvTexture, (int) biome);

        //GFDDArray[(int)direction](x, y, z, width, height, meshData);

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataUp
        (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl = true)
    {
        meshData.AddVertex(new Vector3(x, y + 1, z + width), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + height, y + 1, z + width), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + height, y + 1, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + 1, z), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.up, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataDown
  (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + height, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + height, y, z + width), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y, z + width), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.down, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataNorth
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + width, y, z + 1), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y + height, z + 1), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + height, z + 1), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y, z + 1), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.north, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataEast
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x + 1, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + 1, y + width, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + 1, y + width, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + 1, y, z + height), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.east, height, width));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataSouth
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + height, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y + height, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x + width, y, z), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.south, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataWest
    (int x, int y, int z, int width, int height, MeshData meshData, bool useRenderDataForColl)
    {
        meshData.AddVertex(new Vector3(x, y, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + width, z + height), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y + width, z), useRenderDataForColl);
        meshData.AddVertex(new Vector3(x, y, z), useRenderDataForColl);

        meshData.uv.AddRange(FaceUVs(Direction.west, height, width));

        return meshData;
    }

    public int RenderingEquality(BlockBuilder other, bool frontFace, BiomeType necessaryBiome, Direction direction)
    {
        BlockBuilder chosenBuilder = frontFace ? other : this;

        ushort type = (ushort) ((this.type == other.type && (!chosenBuilder.OVERRIDE_GREEDY_MESHER_RENDERING))
            ? 1 : (int) chosenBuilder.type);

        ushort biomeMask = (ushort) ((int)TexturePosition(direction) < TextureValues.ALPHA_TEXTURE_COUNT ? (int) necessaryBiome : 0);

        return (biomeMask << 16) | type;
    }
}
