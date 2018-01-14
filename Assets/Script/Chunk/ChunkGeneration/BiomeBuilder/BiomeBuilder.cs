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

    float caveFrequency = 0.008f;
    int caveSize = 5;

    float treeFrequency = 0.1f;
    int treeDensity = 3;


    public void GenerateChunkColumn(WorldPos chunkWorldPos, FastNoise noise, BlockType[] blocks,
                                    int x, int z)
    {
        int stoneHeight = stoneBaseHeight;
        stoneHeight += GetNoise(noise, seaFrequency, maximumLandHeight, x, 0, z);

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
                int caveChance = GetNoise(noise, caveFrequency, 25, x, y, z);

                if(caveSize > caveChance)
                {
                    SetBlock(x, y, z, BlockType.Air, chunkWorldPos, blocks);
                }
                else
                {
                    if (y <= stoneHeight)
                    {
                        SetBlock(x, y, z, BlockType.Rock, chunkWorldPos, blocks);
                    }
                    else if (y <= dirtHeight)
                    {
                        SetBlock(x, y, z, BlockType.Grass, chunkWorldPos, blocks);

                        int greenValue = GetNoise(noise, treeFrequency, 20, x, 0, z);

                        if (dirtHeight == y)
                        {
                            if(greenValue < treeDensity)
                            {
                                CreateTree(x, y + 1, z, blocks, chunkWorldPos);
                            }
                            else if(greenValue < 9)
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




}
