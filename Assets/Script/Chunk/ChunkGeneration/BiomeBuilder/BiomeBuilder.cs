using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BiomeBuilder
{
    public struct ColumnValues
    {
        public BiomeType biomeType;
        public int stoneHeight;
        public int dirtHeight;
        public bool isCenterTrunkPossible;

        public ColumnValues(BiomeType biomeType, int stoneHeight, int dirtHeight, bool isCenterTrunkPossible)
        {
            this.biomeType = biomeType;
            this.stoneHeight = stoneHeight;
            this.dirtHeight = dirtHeight;
            this.isCenterTrunkPossible = isCenterTrunkPossible;
        }
    }

    protected static BiomeBuilder[] builders;


    protected virtual BiomeType BIOME_TYPE { get { return BiomeType.Steppe; } }

    protected const int STONE_BASE_HEIGHT = -120;

    protected const int MAXIMUM_LAND_HEIGHT = 300;

    protected const int BELOW_SEA_SAND_HEIGHT = -5;
    protected virtual int ABOVE_SEA_SAND_HEIGHT { get { return 5; } }

    protected virtual float STONE_MAX_MOUNTAIN_HEIGHT { get { return MAXIMUM_LAND_HEIGHT + STONE_BASE_HEIGHT; } } 
    protected virtual float STONE_MOUNTAIN_FREQUENCY { get { return 0.004f; } } 

    protected virtual int DIRT_BASE_HEIGHT { get { return 3; } } 
    protected virtual float DIRT_NOISE { get { return 0.04f; } } 
    protected virtual int DIRT_NOISE_HEIGHT { get { return 8; } } 

    protected virtual int SEA_LEVEL { get { return 0; } } 
    protected virtual float SEA_FREQUENCY { get { return 0.0007f; } } 

    protected virtual float CAVE_FREQUENCY { get { return 0.008f; } } 
    protected virtual int CAVE_SIZE { get { return 5; } } 
    protected virtual int CAVE_MAX_SIZE { get { return 25; } }

    protected virtual float TREE_FREQUENCY { get { return 0.2f; } } 
    protected virtual int TREE_AREA_MAX_VALUE { get { return 1600; } } 
    protected virtual int TREE_AREA_MIN_VALUE { get { return 24; } } 
    protected virtual float TRUNK_DIST { get { return 0.1f; } } 
    protected virtual int TRUNK_MAX_HEIGHT { get { return 5; } } 
    protected virtual int LEAF_MAX_HEIGHT { get { return 5; } } 

    protected virtual float BUSH_FREQUENCY { get { return 0.1f; } } 
    protected virtual int BUSH_DENSITY { get { return 9; } } 
    protected virtual int BUSH_MAX_DENSITY { get { return 20; } }

    protected virtual BlockType SECOND_LAND_LAYER_BLOCK { get { return BlockType.Grass; } } 
    protected virtual BlockType BUSH_BLOCK { get { return BlockType.Bush; } }
    protected virtual BlockType TREE_TRUNK_BLOCK { get { return BlockType.Wood; } }
    protected virtual BlockType TREE_LEAVES_BLOCK { get { return BlockType.Leaves; } }

    protected delegate float MountainHeightMultiplier(float x);
    protected virtual MountainHeightMultiplier MultiplierFunc { get { return Mathf2.ReverseSmooth; } }

    protected readonly BlockType[] belowTrunkBlockTypes;

    static BiomeBuilder()
    {
        builders = new BiomeBuilder[System.Enum.GetValues(typeof(BiomeType)).Length];
        builders[(int)BiomeType.Steppe - 1] = new BiomeBuilder();
    }


    public virtual void GenerateChunkColumn(WorldPos chunkWorldPos, BlockType[] blocks,
                                    int x, int z)
    {
        BiomeNoises noises = NoiseFactory.GetBiomeNoises();
        int stoneHeight = STONE_BASE_HEIGHT;
        stoneHeight += GetNoise(noises.terraNoise, SEA_FREQUENCY, MAXIMUM_LAND_HEIGHT, x, STONE_BASE_HEIGHT, z);

        if (stoneHeight <= SEA_LEVEL)
        {
            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                BlockType seaBlockType = CreateSea(stoneHeight, y, noises);
                SetBlock(x, y, z, seaBlockType, chunkWorldPos, blocks);
            }
        }
        else
        {
            //int stoneMountainHeight = Mathf.RoundToInt(MultiplierFunc(stoneHeight / STONE_MAX_MOUNTAIN_HEIGHT) * STONE_MAX_MOUNTAIN_HEIGHT);
            int stoneMountainHeight = stoneHeight;

            stoneHeight = GetNoise(noises.terraNoise, STONE_MOUNTAIN_FREQUENCY, stoneMountainHeight, x, z);

            int dirtHeight = stoneHeight;
            dirtHeight += GetNoise(noises.terraNoise, DIRT_NOISE, 4, x, z);

            for (int y = chunkWorldPos.y; y < chunkWorldPos.y + Chunk.chunkSize; y++)
            {
                BlockType landBlockType = CreateLand(stoneHeight, dirtHeight, x, y, z, noises);
                SetBlock(x, y, z, landBlockType, chunkWorldPos, blocks);
            }

        }

        NoiseFactory.PutBiomeNoisesInstance(noises);
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateSea(int stoneHeight, int y, BiomeNoises noises)
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
    protected virtual BlockType CreateLand(int stoneHeight, int dirtHeight, int x, int y, int z, BiomeNoises noises)
    {
        // Normally we only need to calculate caveChance when y is lower than dirtHeight. 
        // However if dirt has been selected as cave, then there is a chance above of that dirt, there can be either 
        // trunk or grass, so we need to check that if there is dirt in dirtHeight.
        int caveChance = GetNoise(noises.terraNoise, CAVE_FREQUENCY, CAVE_MAX_SIZE, x, y > dirtHeight ? dirtHeight : y, z);

        bool isCave = CAVE_SIZE > caveChance;

        if (!isCave)
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
        }

        if (y > dirtHeight)
        {
            noises.treeNoise.cellularCenterOffset = new Vector2();
            int treeAreaValue = GetNoise(noises.treeNoise, TREE_FREQUENCY, TREE_AREA_MAX_VALUE, x, z);

            if (treeAreaValue < TREE_AREA_MIN_VALUE)
            {
                BlockType block = CreateTree(stoneHeight, dirtHeight, isCave, x, y, z, noises);
                if (block != BlockType.Air)
                {
                    return block;
                }
            }
        }

        if (!isCave)
        {
            if (y > dirtHeight && stoneHeight < dirtHeight && stoneHeight > ABOVE_SEA_SAND_HEIGHT)
            {
                return CreateGreenery(dirtHeight, x, y, z, noises);
            }

        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateGreenery(int dirtHeight, int x, int y, int z, BiomeNoises noises)
    {
        int greenValue = GetNoise(noises.terraNoise, BUSH_FREQUENCY, BUSH_MAX_DENSITY, x, y + 1, z);
        if (dirtHeight == y - 1 && greenValue < BUSH_DENSITY)
        {
            return BlockType.Bush;
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    protected virtual BlockType CreateTree(int stoneHeight, int dirtHeight, bool isDirtCave, int x, int y, int z, BiomeNoises noises)
    {
        Vector2 treeCenterOffset = noises.treeNoise.cellularCenterOffset / TREE_FREQUENCY;
        Vector2Int treeCenter = new Vector2Int(
            Mathf.FloorToInt(x + treeCenterOffset.x), 
            Mathf.FloorToInt(z + treeCenterOffset.y));

        if (treeCenter.x == x && treeCenter.y == z)
        {
            bool trunkCondition = y > dirtHeight && stoneHeight < dirtHeight && y <= dirtHeight + TRUNK_MAX_HEIGHT
                        && stoneHeight > ABOVE_SEA_SAND_HEIGHT && !isDirtCave;

            if (trunkCondition)
            {
                if (!TerrainGen.trunkPositions.ContainsKey(treeCenter))
                {
                    ColumnValues values = new ColumnValues(BIOME_TYPE, stoneHeight, dirtHeight, true);
                    TerrainGen.trunkPositions.TryAdd(treeCenter, values);
                }

                return TREE_TRUNK_BLOCK;
            }
            else
            {
                if (!TerrainGen.trunkPositions.ContainsKey(treeCenter))
                {
                    ColumnValues values = new ColumnValues(BIOME_TYPE, stoneHeight, dirtHeight, false);
                    TerrainGen.trunkPositions.TryAdd(treeCenter, values);
                }

                return BlockType.Air;
            }

        }

        int centerTrunkStoneHeight, centerTrunkDirtHeight;
        bool canTreeExist = FindCenterTrunkBlock(treeCenter, out centerTrunkStoneHeight, out centerTrunkDirtHeight, noises);

        if (canTreeExist)
        {
            bool trunkCondition = y > dirtHeight && stoneHeight < dirtHeight && y <= centerTrunkDirtHeight + TRUNK_MAX_HEIGHT
                                    && stoneHeight > ABOVE_SEA_SAND_HEIGHT && !isDirtCave;

            bool leafCondition = y > centerTrunkDirtHeight + TRUNK_MAX_HEIGHT && y <= centerTrunkDirtHeight + LEAF_MAX_HEIGHT + TRUNK_MAX_HEIGHT;

            if (trunkCondition)
            {
                float treeCenterDist = treeCenterOffset.magnitude;
                if (treeCenterDist < 1.2f)
                {
                    return TREE_TRUNK_BLOCK;
                }
            }
            else if (leafCondition)
            {
                return TREE_LEAVES_BLOCK;
            }
        }

        return BlockType.Air;
    }

    [MethodImplAttribute(256)]
    protected bool FindCenterTrunkBlock(Vector2Int centerTrunkPos, out int centerTrunkStoneHeight, out int centerTrunkDirtHeight, BiomeNoises noises)
    {
        bool isTrunkAdded = TerrainGen.trunkPositions.TryGetValue(centerTrunkPos, out ColumnValues columnValues);
        bool isTrunkPossible = columnValues.isCenterTrunkPossible;

        if (!isTrunkAdded)
        {
            centerTrunkStoneHeight = STONE_BASE_HEIGHT;
            centerTrunkStoneHeight += GetNoise(noises.terraNoise, SEA_FREQUENCY, MAXIMUM_LAND_HEIGHT, centerTrunkPos.x, STONE_BASE_HEIGHT, centerTrunkPos.y);

            int stoneMountainHeight = Mathf.RoundToInt(MultiplierFunc(centerTrunkStoneHeight / STONE_MAX_MOUNTAIN_HEIGHT) * STONE_MAX_MOUNTAIN_HEIGHT);

            centerTrunkStoneHeight = GetNoise(noises.terraNoise, STONE_MOUNTAIN_FREQUENCY, stoneMountainHeight, centerTrunkPos.x, centerTrunkPos.y);

            centerTrunkDirtHeight = centerTrunkStoneHeight;
            centerTrunkDirtHeight += GetNoise(noises.terraNoise, DIRT_NOISE, 4, centerTrunkPos.x, centerTrunkPos.y);

            int type = GetNoise(noises.biomeSelectNoise, 0.05f, 3, centerTrunkPos.x, centerTrunkPos.y);
            BlockType block = builders[type].CreateLand(centerTrunkStoneHeight, centerTrunkDirtHeight, centerTrunkPos.x, centerTrunkDirtHeight + 1, centerTrunkPos.y, noises);
            //BlockType block = CreateLand(centerTrunkStoneHeight, centerTrunkDirtHeight, centerTrunkPos.x, centerTrunkDirtHeight + 1, centerTrunkPos.y, noises);

            isTrunkPossible = block == TREE_TRUNK_BLOCK;

            columnValues.dirtHeight = centerTrunkDirtHeight;
            columnValues.stoneHeight = centerTrunkStoneHeight;
            columnValues.isCenterTrunkPossible = isTrunkPossible;
            columnValues.biomeType = (BiomeType)type;

            TerrainGen.trunkPositions.TryAdd(centerTrunkPos, columnValues);

            Debug.Log($"{BIOME_TYPE}, {builders[type].BIOME_TYPE}");
        }
        else
        {
            centerTrunkStoneHeight = columnValues.stoneHeight;
            centerTrunkDirtHeight = columnValues.dirtHeight;
        }

        return isTrunkPossible;
    }

    [MethodImplAttribute(256)]
    protected static int GetNoise(FastNoise noise, float freq, int max, int x, int y, int z)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y, z) + 1f) * (max / 2f));
    }

    [MethodImplAttribute(256)]
    protected static int GetNoise(FastNoise noise, float freq, int max, int x, int y)
    {
        noise.SetFrequency(freq);
        return Mathf.FloorToInt((noise.GetNoise(x, y) + 1f) * (max / 2f));
    }

    [MethodImplAttribute(256)]
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

    [MethodImplAttribute(256)]
    protected static BlockType GetBlockInChunk(int x, int y, int z, BlockType[] blocks)
    {
        return blocks[x + (y * Chunk.chunkSize * Chunk.chunkSize) + (z * Chunk.chunkSize)];
    }
}
