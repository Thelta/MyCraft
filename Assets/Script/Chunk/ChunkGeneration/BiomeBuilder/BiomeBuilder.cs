using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using UnityEngine;

public class BiomeBuilder
{
    protected const int STONE_BASE_HEIGHT = -120;

    protected const int MAXIMUM_LAND_HEIGHT = 300;

    protected const int BELOW_SEA_SAND_HEIGHT = -5;
    protected const int ABOVE_SEA_SAND_HEIGHT = 5;

    protected virtual float STONE_MAX_MOUNTAIN_HEIGHT { get { return MAXIMUM_LAND_HEIGHT + STONE_BASE_HEIGHT; } } 
    protected virtual float STONE_MOUNTAIN_FREQUENCY { get { return 0.004f; } } 

    protected virtual int DIRT_BASE_HEIGHT { get { return 3; } } 
    protected virtual float DIRT_NOISE { get { return 0.04f; } } 
    protected virtual int DIRT_NOISE_HEIGHT { get { return 8; } } 

    protected virtual int SEA_LEVEL { get { return 0; } } 
    protected virtual float SEA_FREQUENCY { get { return 0.0007f; } } 

    protected virtual float CAVE_FREQUENCY { get { return 0.008f; } } 
    protected virtual int CAVE_SIZE { get { return 5; } } 

    protected virtual float TREE_FREQUENCY { get { return 0.2f; } } 
    protected virtual int TREE_AREA_MAX_VALUE { get { return 1600; } } 
    protected virtual int TREE_AREA_MIN_VALUE { get { return 24; } } 
    protected virtual float TRUNK_DIST { get { return 0.1f; } } 
    protected virtual int TRUNK_MAX_HEIGHT { get { return 5; } } 
    protected virtual int LEAF_MAX_HEIGHT { get { return 5; } } 

    protected virtual float BUSH_FREQUENCY { get { return 0.1f; } } 
    protected virtual int BUSH_DENSITY { get { return 9; } } 

    protected virtual BlockType SECOND_LAND_LAYER_BLOCK { get { return BlockType.Grass; } } 
    protected virtual BlockType BUSH_BLOCK { get { return BlockType.Bush; } }
    protected virtual BlockType TREE_TRUNK_BLOCK { get { return BlockType.Wood; } }
    protected virtual BlockType TREE_LEAVES_BLOCK { get { return BlockType.Leaves; } }

    protected FastNoise terraNoise;
    protected FastNoise treeNoise;
    protected FastNoise trunkNoise;

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
        int stoneHeight = STONE_BASE_HEIGHT;
        stoneHeight += GetNoise(terraNoise, SEA_FREQUENCY, MAXIMUM_LAND_HEIGHT, x, STONE_BASE_HEIGHT, z);

        if (stoneHeight <= SEA_LEVEL)
        {
            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                BlockType seaBlockType = CreateSea(stoneHeight, y);
                SetBlock(x, y, z, seaBlockType, chunkWorldPos, blocks);
            }
        }
        else
        {
            int stoneMountainHeight = Mathf.RoundToInt(Mathf2.ReverseSmooth(stoneHeight / STONE_MAX_MOUNTAIN_HEIGHT) * STONE_MAX_MOUNTAIN_HEIGHT);

            stoneHeight = GetNoise(terraNoise, STONE_MOUNTAIN_FREQUENCY, stoneMountainHeight, x, 0, z);

            int dirtHeight = stoneHeight;
            dirtHeight += GetNoise(terraNoise, DIRT_NOISE, 4, x, stoneHeight, z);

            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                BlockType landBlockType = CreateLand(stoneHeight, dirtHeight, x, y, z);
                SetBlock(x, y, z, landBlockType, chunkWorldPos, blocks);
            }

        }
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateSea(int stoneHeight, int y)
    {
        if (y <= stoneHeight)
        {
            if (y <= BELOW_SEA_SAND_HEIGHT)
            {
                return BlockType.Rock;
            }

            return BlockType.Sand;

        }

        if (y <= SEA_LEVEL)
        {
            return BlockType.Water;
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateLand(int stoneHeight, int dirtHeight, int x, int y, int z)
    {
        int caveChance = GetNoise(terraNoise, CAVE_FREQUENCY, 25, x, y, z);

        if (CAVE_SIZE <= caveChance)
        {
            if (y <= stoneHeight)
            {
                if (stoneHeight < ABOVE_SEA_SAND_HEIGHT && stoneHeight > BELOW_SEA_SAND_HEIGHT)
                {
                    return BlockType.Sand;
                }

                return BlockType.Rock;

            }
            else if (y <= dirtHeight && stoneHeight > ABOVE_SEA_SAND_HEIGHT)
            {
                return SECOND_LAND_LAYER_BLOCK;
            }
            else if (y > dirtHeight && stoneHeight <= dirtHeight && stoneHeight > ABOVE_SEA_SAND_HEIGHT)
            {
                return CreateGreenery(dirtHeight, x, y, z);
            }
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateGreenery(int dirtHeight, int x, int y, int z)
    {
        int treeAreaValue = GetNoise(treeNoise, TREE_FREQUENCY, TREE_AREA_MAX_VALUE, x, z);
        int dirtCaveChance = GetNoise(terraNoise, CAVE_FREQUENCY, 25, x, dirtHeight, z);

        if (treeAreaValue < TREE_AREA_MIN_VALUE && CAVE_SIZE <= dirtCaveChance)
        {
            return CreateTree(dirtHeight, x, y, z);
        }

        int greenValue = GetNoise(terraNoise, BUSH_FREQUENCY, 20, x, y + 1, z);
        if (dirtHeight == y - 1 && greenValue < BUSH_DENSITY)
        {
            return BlockType.Bush;
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateTree(int dirtHeight, int x, int y, int z)
    {
        if (y <= dirtHeight + TRUNK_MAX_HEIGHT)
        {
            trunkNoise.SetFrequency(TREE_FREQUENCY);
            float treeCenterDist = trunkNoise.GetCellular(x, z);
            if (treeCenterDist * treeCenterDist < TRUNK_DIST)
            {
                return TREE_TRUNK_BLOCK;
            }
        }
        else if (y > dirtHeight + TRUNK_MAX_HEIGHT && y <= dirtHeight + LEAF_MAX_HEIGHT + TRUNK_MAX_HEIGHT)
        {
            return TREE_LEAVES_BLOCK;
        }

        return BlockType.Air;

    }

    protected static int GetNoise(FastNoise noise, float freq, int max, int x, int y, int z)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y, z) + 1f) * (max / 2f));
    }

    protected static int GetNoise(FastNoise noise, float freq, int max, int x, int y)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y) + 1f) * (max / 2f));
    }


    protected static void SetBlock(int x, int y, int z,
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
