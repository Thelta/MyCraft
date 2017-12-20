using UnityEngine;
using System.Collections;


public class BlockBuilder
{    
    public BlockType type;

    public BlockBuilder()
    {
        type = BlockType.Rock;
    }


    public struct Tile { public int x; public int y; }

    public virtual TextureType TexturePosition(Direction direction)
    {
        return TextureType.Rock;
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
        (int x, int y, int z, int width, int height, Direction direction, MeshData meshData)
    {
        meshData.useRenderDataForColl = true;

        switch (direction)
        {
            case Direction.north:
                meshData = GreedyFaceGroupDataNorth(x, y, z, width, height, meshData);
                break;
            case Direction.east:
                meshData = GreedyFaceGroupDataEast(x, y, z, width, height, meshData);
                break;
            case Direction.south:
                meshData = GreedyFaceGroupDataSouth(x, y, z, width, height, meshData);
                break;
            case Direction.west:
                meshData = GreedyFaceGroupDataWest(x, y, z, width, height, meshData);
                break;
            case Direction.up:
                meshData = GreedyFaceGroupDataUp(x, y, z, width, height, meshData);
                break;
            case Direction.down:
                meshData = GreedyFaceGroupDataDown(x, y, z, width, height, meshData);
                break;
        }

        //add texture index to uv2
        int uvTexture = (int)TexturePosition(direction);
        meshData.texType.Add(new Vector2(uvTexture, 0));
        meshData.texType.Add(new Vector2(uvTexture, 0));
        meshData.texType.Add(new Vector2(uvTexture, 0));
        meshData.texType.Add(new Vector2(uvTexture, 0));

        //GFDDArray[(int)direction](x, y, z, width, height, meshData);

        return meshData;
    }

    protected virtual MeshData GreedyFaceGroupDataUp
        (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x, y + 1, z + width));
        meshData.AddVertex(new Vector3(x + height, y + 1, z + width));
        meshData.AddVertex(new Vector3(x + height, y + 1, z));
        meshData.AddVertex(new Vector3(x, y + 1, z));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.up, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataDown
  (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x, y, z));
        meshData.AddVertex(new Vector3(x + height, y, z));
        meshData.AddVertex(new Vector3(x + height, y, z + width));
        meshData.AddVertex(new Vector3(x, y, z + width));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.down, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataNorth
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + width, y, z + 1));
        meshData.AddVertex(new Vector3(x + width, y + height, z + 1));
        meshData.AddVertex(new Vector3(x, y + height, z + 1));
        meshData.AddVertex(new Vector3(x, y, z + 1));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.north, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataEast
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 1, y, z));
        meshData.AddVertex(new Vector3(x + 1, y + width, z));
        meshData.AddVertex(new Vector3(x + 1, y + width, z + height));
        meshData.AddVertex(new Vector3(x + 1, y, z + height));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.east, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataSouth
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x, y, z));
        meshData.AddVertex(new Vector3(x, y + height, z));
        meshData.AddVertex(new Vector3(x + width, y + height, z));
        meshData.AddVertex(new Vector3(x + width, y, z));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.south, width, height));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataWest
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x, y, z + height));
        meshData.AddVertex(new Vector3(x, y + width, z + height));
        meshData.AddVertex(new Vector3(x, y + width, z));
        meshData.AddVertex(new Vector3(x, y, z));


        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.west, width, height));

        return meshData;
    }

}
