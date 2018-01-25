using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeBuilder
{
    const int stoneBaseHeight = -120;

    const float stoneMaxMountainHeight = maximumLandHeight + stoneBaseHeight;
    const float stoneMountainFrequency = 0.004f;

    const int belowSeaSandHeight = -5;
    const int aboveSeaSandHeight = 5;

    const int dirtBaseHeight = 3;
    const float dirtNoise = 0.04f;
    const int dirtNoiseHeight = 8;

    const int seaLevel = 0;
    const float seaFrequency = 0.0007f;

    const int maximumLandHeight = 300;

    const float caveFrequency = 0.008f;
    const int caveSize = 5;

    const float treeFrequency = 0.1f;
    const int treeDensity = 3;

    const int bushDensity = 9;


    public virtual void GenerateChunkColumn(WorldPos chunkWorldPos, FastNoise noise, BlockType[] blocks,
                                    int x, int z)
    {
        int stoneHeight = stoneBaseHeight;
        stoneHeight += GetNoise(noise, seaFrequency, maximumLandHeight, x, stoneBaseHeight, z);

        if (stoneHeight <= seaLevel)
        {
            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                if (y <= stoneHeight)
                {
                    if(y <= belowSeaSandHeight)
                    {
                        SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, blocks);
                    }
                    else
                    {
                        SetBlock(x, y, z, BlockType.Sand, chunkWorldPos, blocks);
                    }
                    
                }
                else if (y <= seaLevel)
                {
                    SetBlock(x, y, z, BlockType.Water, chunkWorldPos, blocks);

                }
            }
        }
        else
        {

            int stoneMountainHeight = Mathf.RoundToInt(ReverseSmooth(stoneHeight / stoneMaxMountainHeight) * stoneMaxMountainHeight);

            stoneHeight = GetNoise(noise, stoneMountainFrequency, stoneMountainHeight, x, 0, z);

            int dirtHeight = stoneHeight;
            dirtHeight += GetNoise(noise, dirtNoise, 4, x, stoneHeight, z);

            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                int caveChance = GetNoise(noise, caveFrequency, 25, x, y, z);

                if(caveSize > caveChance)
                {
                    SetBlock(x, y, z, BlockType.Air, chunkWorldPos, blocks);
                }
                else
                {
                    if (y <= stoneHeight)
                    {
                        if(stoneHeight < aboveSeaSandHeight && stoneHeight > belowSeaSandHeight)
                        {
                            SetBlock(x, y, z, BlockType.Sand, chunkWorldPos, blocks);
                        }
                        else
                        {
                            SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, blocks);
                        }
                        
                    }
                    else if (y <= dirtHeight && stoneHeight > aboveSeaSandHeight)
                    {
                        SetBlock(x, y, z, BlockType.Grass, chunkWorldPos, blocks);

                        int greenValue = GetNoise(noise, treeFrequency, 20, x, y + 1, z);

                        if (dirtHeight == y)
                        {
                            if(greenValue < treeDensity)
                            {
                                CreateTree(x, y + 1, z, blocks, chunkWorldPos);
                            }
                            else if(greenValue < bushDensity)
                            {
                                SetBlock(x, y + 1, z, BlockType.Bush, chunkWorldPos, blocks);
                            }
                            
                        }
                    }
                    else
                    {
                        SetBlock(x, y, z, BlockType.Air, chunkWorldPos, blocks);
                    }

                }
            }

        }
    }

    void CreateTree(int x, int y, int z, BlockType[] blocks, WorldPos chunkWorldPos)
    {
        //create leaves
        for (int xi = -2; xi <= 2; xi++)
        {
            for (int yi = 4; yi <= 8; yi++)
            {
                for (int zi = -2; zi <= 2; zi++)
                {
                    SetBlock(x + xi, y + yi, z + zi, BlockType.Leaves, chunkWorldPos, blocks, true);
                }
            }
        }
        //create trunk
        for (int yt = 0; yt < 4; yt++)
        {
            SetBlock(x, y + yt, z, BlockType.Wood, chunkWorldPos, blocks, true);
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
            int blockValue = (int)blocks[x + (y * Chunk.chunkSize * Chunk.chunkSize) + (z * Chunk.chunkSize)];

            if(replaceBlocks || blockValue == 0)
            {
                blocks[x + (y * Chunk.chunkSize * Chunk.chunkSize) + (z * Chunk.chunkSize)] = block;
            }
        }
    }

    static float ReverseSmooth(float x)
    {
        return x + (x - (x * x * (3.0f - 2.0f * x)));
    }

}
