using UnityEngine;
using System.Collections.Generic;
using SimplexNoise;

public class TerrainGen
{
    public Queue<TerrainGenData> dataQueue;
    FastNoise biomeNoise;
    FastNoise terrainNoise;

    BiomeBuilder builder;

    float stoneBaseHeight = -24;
    float stoneBaseNoise = 0.05f;
    float stoneBaseNoiseHeight = 4;

    float stoneMountainHeight = 48;
    float stoneMountainFrequency = 0.008f;
    float stoneMinHeight = -12;

    float dirtBaseHeight = 1;
    float dirtNoise = 0.04f;
    float dirtNoiseHeight = 3;

    float caveFrequency = 0.025f;
    int caveSize = 7;

    float treeFrequency = 0.1f;
    int treeDensity = 3 ;

    public TerrainGen()
    {
        dataQueue = new Queue<TerrainGenData>();

        biomeNoise = new FastNoise();

        FastNoise noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.Simplex);
        biomeNoise.SetCellularNoiseLookup(noise);

        terrainNoise = new FastNoise();
        terrainNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        terrainNoise.SetFractalType(FastNoise.FractalType.FBM);

        builder = new BiomeBuilder();
        
    }

    public void ChunkGen(WorldPos chunkWorldPos)
    {
        for (int x = chunkWorldPos.x - 3; x < chunkWorldPos.x + Chunk.chunkSize + 3; x++)
        {
            for (int z = chunkWorldPos.z - 3; z < chunkWorldPos.z + Chunk.chunkSize + 3; z++)
            {
                Vector2 doubleNoise = biomeNoise.GetDoubleCellularNoise(x, z);

                //ChunkColumnGen(chunkWorldPos, x, z);
                builder.GenerateChunkColumn(chunkWorldPos, terrainNoise, dataQueue, x, z);
            }
        }
    }

    public void ChunkGen(WorldPos chunkWorldPos, BlockType debugBlock)
    {
        for(int i = 0; i < 16; i++)
        {
            for(int j = 0; j < 16; j++)
            {
                for(int k = 0; k < 16; k++)
                {
                    if(i < 4 || j < 4 || k < 4)
                    {
                        SetBlock(i, j, k, debugBlock, chunkWorldPos, dataQueue);
                    }
                    else
                    {
                        SetBlock(i, j, k, BlockType.Air, chunkWorldPos, dataQueue);
                    }
                    
                }
            }
        }
    }

    public void ChunkColumnGen(WorldPos chunkWorldPos, int x, int z)
    {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));

        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);

        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));

        int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
        dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));

        for (int y = chunkWorldPos.y - 8; y < chunkWorldPos.y + Chunk.chunkSize; y++)
        {
            int caveChance = GetNoise(x, y, z, caveFrequency, 100);
            if (y <= stoneHeight && caveSize < caveChance)
            {
                SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, dataQueue);
            }
            else if (y <= dirtHeight && caveSize < caveChance)
            {
                SetBlock(x, y, z, BlockType.Grass, chunkWorldPos, dataQueue);
                if (y == dirtHeight && GetNoise(x, 0, z, treeFrequency, 100) < treeDensity)   
                    CreateTree(x, y + 1, z, chunkWorldPos);
            }
            else
            {
                SetBlock(x, y, z, BlockType.Air, chunkWorldPos, dataQueue);
            }

        }
    }

    public static int GetNoise(int x, int y, int z, float scale, int max)
    {
        return Mathf.FloorToInt((Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max / 2f));
    }


    public static void SetBlock(int x, int y, int z, 
                                BlockType block, WorldPos chunkWorldPos, Queue<TerrainGenData> dataQueue,
                                bool replaceBlocks = false)
    {
        x -= chunkWorldPos.x;
        y -= chunkWorldPos.y;
        z -= chunkWorldPos.z;

        if(Chunk.InRange(x) && Chunk.InRange(y) && Chunk.InRange(z))
        {
            lock(dataQueue)
            {
                dataQueue.Enqueue(new TerrainGenData(x, y, z, replaceBlocks, block));
            }
        }
    }

    void CreateTree(int x, int y, int z, WorldPos chunkWorldPos)
    {
        //create leaves
        for (int xi = -2; xi <= 2; xi++)
        {
            for (int yi = 4; yi <= 8; yi++)
            {
                for (int zi = -2; zi <= 2; zi++)
                {
                    SetBlock(x + xi, y + yi, z + zi, BlockType.Leaves, chunkWorldPos, dataQueue, true);
                }
            }
        }
        //create trunk
        for (int yt = 0; yt < 6; yt++)
        {
            SetBlock(x, y + yt, z, BlockType.Wood, chunkWorldPos, dataQueue, true);
        }
    }
}