using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

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

    const float treeFrequency = 0.2f;
    const int treeAreaMaxValue = 1600;
    const int treeAreaMinValue = 24;
    const float trunkDist = 0.1f;
    const int trunkMaxHeight = 5;
    const int leafMaxHeight = 5;

    const float bushFrequency = 0.1f;
    const int bushDensity = 9;

    FastNoise terraNoise;
    FastNoise treeNoise;
    FastNoise trunkNoise;

    public BiomeBuilder()
    {
        terraNoise = new FastNoise();
        terraNoise.SetFractalType(FastNoise.FractalType.FBM);
        terraNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

        treeNoise = new FastNoise();
        treeNoise.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);
        treeNoise.SetCellularJitter(0.2f);
        treeNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
        treeNoise.SetNoiseType(FastNoise.NoiseType.Cellular);

        trunkNoise = new FastNoise();
        trunkNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Div);
        trunkNoise.SetCellularJitter(0.2f);
        trunkNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
        trunkNoise.SetNoiseType(FastNoise.NoiseType.Cellular);

    }

    public virtual void GenerateChunkColumn(WorldPos chunkWorldPos, BlockType[] blocks,
                                    int x, int z)
    {
        int stoneHeight = stoneBaseHeight;
        stoneHeight += GetNoise(terraNoise, seaFrequency, maximumLandHeight, x, stoneBaseHeight, z);

        if (stoneHeight <= seaLevel)
        {
            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                BlockType seaBlockType = CreateSea(stoneHeight, y);
                SetBlock(x, y, z, seaBlockType, chunkWorldPos, blocks);
            }
        }
        else
        {
            int stoneMountainHeight = Mathf.RoundToInt(ReverseSmooth(stoneHeight / stoneMaxMountainHeight) * stoneMaxMountainHeight);

            stoneHeight = GetNoise(terraNoise, stoneMountainFrequency, stoneMountainHeight, x, 0, z);

            int dirtHeight = stoneHeight;
            dirtHeight += GetNoise(terraNoise, dirtNoise, 4, x, stoneHeight, z);

            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                BlockType landBlockType = CreateLand(stoneHeight, dirtHeight, x, y, z);
                SetBlock(x, y, z, landBlockType, chunkWorldPos, blocks);
            }

        }
    }

    [MethodImplAttribute(256)]
    BlockType CreateSea(int stoneHeight, int y)
    {
        if (y <= stoneHeight)
        {
            if (y <= belowSeaSandHeight)
            {
                return BlockType.Rock;
            }

            return BlockType.Sand;

        }

        if (y <= seaLevel)
        {
            return BlockType.Water;
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    BlockType CreateLand(int stoneHeight, int dirtHeight, int x, int y, int z)
    {
        int caveChance = GetNoise(terraNoise, caveFrequency, 25, x, y, z);

        if (caveSize <= caveChance)
        {
            if (y <= stoneHeight)
            {
                return DecideRockOrSand(stoneHeight);
            }
            else if (y <= dirtHeight && stoneHeight > aboveSeaSandHeight)
            {
                return BlockType.Grass;
            }
            else if (y > dirtHeight && stoneHeight <= dirtHeight && stoneHeight > aboveSeaSandHeight)
            {
                return CreateGreenery(dirtHeight, x, y, z);
            }
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    BlockType DecideRockOrSand(int stoneHeight)
    {
        if (stoneHeight < aboveSeaSandHeight && stoneHeight > belowSeaSandHeight)
        {
            return BlockType.Sand;
        }

        return BlockType.Rock;
    }

    [MethodImplAttribute(256)]
    BlockType CreateGreenery(int dirtHeight, int x, int y, int z)
    {
        int treeAreaValue = GetNoise(treeNoise, treeFrequency, treeAreaMaxValue, x, z);
        int dirtCaveChance = GetNoise(terraNoise, caveFrequency, 25, x, dirtHeight, z);

        if (treeAreaValue < treeAreaMinValue && caveSize <= dirtCaveChance)
        {
            return CreateTree(dirtHeight, x, y, z);
        }

        int greenValue = GetNoise(terraNoise, bushFrequency, 20, x, y + 1, z);
        if (dirtHeight == y - 1 && greenValue < bushDensity)
        {
            return BlockType.Bush;
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    BlockType CreateTree(int dirtHeight, int x, int y, int z)
    {
        if (y <= dirtHeight + trunkMaxHeight)
        {
            trunkNoise.SetFrequency(treeFrequency);
            float treeCenterDist = trunkNoise.GetCellular(x, z);
            if (treeCenterDist * treeCenterDist < trunkDist)
            {
                return BlockType.Wood;
            }
        }
        else if (y > dirtHeight + trunkMaxHeight && y <= dirtHeight + leafMaxHeight + trunkMaxHeight)
        {
            return BlockType.Leaves;
        }

        return BlockType.Air;

    }

    public static int GetNoise(FastNoise noise, float freq, int max, int x, int y, int z)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y, z) + 1f) * (max / 2f));
    }

    public static int GetNoise(FastNoise noise, float freq, int max, int x, int y)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y) + 1f) * (max / 2f));
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
