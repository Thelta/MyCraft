//#define chunk_debug

using UnityEngine;
using System.Collections.Generic;
using SimplexNoise;

public class TerrainGen
{
    public BlockType[] blocks;
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
        blocks = new BlockType[Chunk.chunkSize * Chunk.chunkSize * Chunk.chunkSize];

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
        for (int x = chunkWorldPos.x; x < chunkWorldPos.x + Chunk.chunkSize; x++)
        {
            for (int z = chunkWorldPos.z; z < chunkWorldPos.z + Chunk.chunkSize; z++)
            {
                Vector2 doubleNoise = biomeNoise.GetDoubleCellularNoise(x, z);
#if chunk_debug
                DebugGen(chunkWorldPos, x, z);
#else
                builder.GenerateChunkColumn(chunkWorldPos, terrainNoise, blocks, x, z);
#endif
            }
        }
    }


    public void DebugGen(WorldPos chunkWorldPos, int x, int z)
    {
        for(int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
        {
            {
                SetBlock(x, y, z, BlockType.Water, chunkWorldPos, blocks);
            }
            
        }
    }



    public static void SetBlock(int x, int y, int z, 
                                BlockType block, WorldPos chunkWorldPos, BlockType[] blocks,
                                bool replaceBlocks = false)
    {
        x -= chunkWorldPos.x;
        y -= chunkWorldPos.y;
        z -= chunkWorldPos.z;

        if(Chunk.InRange(x) && Chunk.InRange(y) && Chunk.InRange(z))
        {
            blocks[x + (y * Chunk.chunkSize * Chunk.chunkSize) + (z * Chunk.chunkSize)] = block;
        }
    }

}