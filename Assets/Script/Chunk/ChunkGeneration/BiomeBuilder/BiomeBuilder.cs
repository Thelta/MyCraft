using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeBuilder
{
    int stoneBaseHeight = -120;
    float stoneBaseNoise = 0.01f;
    int stoneBaseNoiseHeight = 4;
    int stoneMinHeight = -22;

    int stoneMountainHeight = 10;
    float stoneMountainFrequency = 0.00075f;
    
    int dirtBaseHeight = 6;
    float dirtNoise = 0.04f;
    int dirtNoiseHeight = 4;

    int seaNoiseHeight = 30;
    int seaLevel = 0;
    float seaFrequency = 0.0015f;

    int maximumLandHeight = 300;

    float caveFrequency = 0.025f;
    int caveSize = 15;


    public void GenerateChunkColumn(WorldPos chunkWorldPos, FastNoise noise, BlockType[] blocks,
                                    int x, int z)
    {
        int stoneHeight = stoneBaseHeight;
        stoneHeight += GetNoise(noise, seaFrequency, maximumLandHeight, x, 0, z);
        //Debug.Log(stoneHeight);

        if (stoneHeight < seaLevel)
        {
            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                if (y <= stoneHeight)
                {
                    SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, blocks);
                }
                else if (y <= seaLevel)
                {
                    SetBlock(x, y, z, BlockType.Water, chunkWorldPos, blocks);

                }
            }
        }
        else
        {
            int dirtHeight = stoneHeight + dirtBaseHeight;
            dirtHeight += GetNoise(noise, dirtNoise, dirtNoiseHeight, x, 10, z);

            stoneHeight += GetNoise(noise, stoneMountainFrequency, stoneMountainHeight, x, 0, z);

            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {

                if (y <= stoneHeight)
                {
                    SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, blocks);
                }
                else if (y <= dirtHeight)
                {
                    SetBlock(x, y, z, BlockType.Grass, chunkWorldPos, blocks);
                }
                else
                {
                    SetBlock(x, y, z, BlockType.Air, chunkWorldPos, blocks);
                }
            }

        }



    }

    public static int GetNoise(FastNoise noise, float freq, int max, int x, int y, int z)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y, z) + 1f) * (max / 2f));
    }

    public static void SetBlock(int x, int y, int z,
                        BlockType block, WorldPos chunkWorldPos, BlockType[] blocks,
                        bool replaceBlocks = false)
    {
        x -= chunkWorldPos.x;
        y -= chunkWorldPos.y;
        z -= chunkWorldPos.z;

        if (Chunk.InRange(x) && Chunk.InRange(y) && Chunk.InRange(z))
        {
            blocks[x + (y * Chunk.chunkSize * Chunk.chunkSize) + (z * Chunk.chunkSize)] = block;
        }
    }




}
