using UnityEngine;
using System.Collections;


public class Block
{
    const float tileSize = 0.25f;

    //public delegate MeshData GreedyFaceGroupDataDirection(int x, int y, int z, int width, int height, MeshData meshData);

    //static protected GreedyFaceGroupDataDirection[] GFDDArray = new GreedyFaceGroupDataDirection[] { GreedyFaceGroupDataNorth, GreedyFaceGroupDataEast, GreedyFaceGroupDataSouth, GreedyFaceGroupDataWest, GreedyFaceGroupDataUp, GreedyFaceGroupDataDown };;
    
    public BlockType type;

    public Block()
    {
        type = BlockType.rock;
    }


    public struct Tile { public int x; public int y; }

    public virtual Tile TexturePosition(Direction direction)
    {
        Tile tile = new Tile();
        tile.x = 0; tile.y = 0;

        return tile;
    }

    public virtual Vector2[] FaceUVs(Direction direction)
    {
        Vector2[] UVs = new Vector2[4];
        Tile tilePos = TexturePosition(direction);

        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x,  tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x,  tileSize * tilePos.y);

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


        //GFDDArray[(int)direction](x, y, z, width, height, meshData);

        return meshData;
    }

    protected virtual MeshData GreedyFaceGroupDataUp
        (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f + width));
        meshData.AddVertex(new Vector3(x + 0.5f + height, y + 0.5f, z + 0.5f + width));
        meshData.AddVertex(new Vector3(x + 0.5f + height, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.up));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataDown
  (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f + height, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f + height, y - 0.5f, z + 0.5f + width));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f + width));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.down));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataNorth
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f + width, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f + width, y + 0.5f + height, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f + height, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.north));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataEast
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f + width, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f + width, z + 0.5f + height));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f + height));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.east));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataSouth
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f + height, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f + width, y + 0.5f + height, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f + width, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.south));

        return meshData;
    }


    protected virtual MeshData GreedyFaceGroupDataWest
    (int x, int y, int z, int width, int height, MeshData meshData)
    {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f + height));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f + width, z + 0.5f + height));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f + width, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();

        meshData.uv.AddRange(FaceUVs(Direction.west));

        return meshData;
    }

}
