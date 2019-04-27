//#define chunk_debug

using UnityEngine;
using System.Collections.Generic;
using SimplexNoise;

public class TerrainGen
{
    public BlockType[] blocks;
    public BiomeType[] biomes;
    FastNoise biomeNoise;
    FastNoise terrainNoise;
    BiomeBuilder[] builders;
    Dictionary<Vector2Int, BiomeBuilder.ColumnValues> trunkPositions;

    
    public TerrainGen()
    {
        blocks = new BlockType[Chunk.chunkSize * Chunk.chunkSize * Chunk.chunkSize];
        biomes = new BiomeType[Chunk.chunkSize * Chunk.chunkSize];

        FastNoise biomeLookupNoise = new FastNoise();
        biomeLookupNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

        biomeNoise = new FastNoise();
        biomeNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        biomeNoise.SetCellularReturnType(FastNoise.CellularReturnType.NoiseLookup);
        biomeNoise.SetCellularNoiseLookup(biomeLookupNoise);
        biomeNoise.SetCellularJitter(0.75f);
        
        trunkPositions = new Dictionary<Vector2Int, BiomeBuilder.ColumnValues>();

        builders = new BiomeBuilder[] { new BiomeBuilder(trunkPositions), new BiomeForestBuilder(trunkPositions), new BiomeDesertBuilder(trunkPositions) };

    }

    public void ChunkGen(WorldPos chunkWorldPos)
    {
        for (int x = chunkWorldPos.x; x < chunkWorldPos.x + Chunk.chunkSize; x++)
        {
            for (int z = chunkWorldPos.z; z < chunkWorldPos.z + Chunk.chunkSize; z++)
            {
                //Vector2 doubleNoise = biomeNoise.GetDoubleCellularNoise(x, z);
#if chunk_debug
                DebugGen(chunkWorldPos, x, z);
#else
                int ix = x - chunkWorldPos.x;
                int iz = z - chunkWorldPos.z;

                BiomeType type = (BiomeType)GetNoise(biomeNoise, 0.05f, 3, x, z);
                biomes[ix + iz * Chunk.chunkSize] = type;
                builders[(int)type].GenerateChunkColumn(chunkWorldPos, blocks, x, z);
#endif
            }
        }
    }


    public void DebugGen(WorldPos chunkWorldPos, int x, int z)
    {
        for(int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
        {
            {
                if(y == 15)
                {
                    SetBlock(x, y, z, BlockType.Bush, chunkWorldPos, blocks);
                }
                else
                {
                    SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, blocks);
                }
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

    protected static int GetNoise(FastNoise noise, float freq, int max, int x, int y)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y) + 1f) * (max / 2f));
    }
}