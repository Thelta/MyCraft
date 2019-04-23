using UnityEngine;
using System.Threading;
using UnityEngine.Profiling;
using System.Runtime.CompilerServices;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour, IBlockHelper, IBiomeHelper
{
    [HideInInspector]
	public BlockType[] blocks;
    [HideInInspector]
    public BiomeType[] biomes;
	public static int chunkSize = 16;
	[HideInInspector]
	public bool update = false;
	[HideInInspector]
	public World world;
	[HideInInspector]
	public WorldPos pos;

	MeshFilter filter;
	MeshCollider coll;

	public bool isGenerating;   
	TerrainGen generator;
	Thread generatorThread;
   
	static readonly BlockBuilder[] blockBuilders = { null, new BlockAirBuilder(), new BlockBuilder(), new BlockGrassBuilder(),
        new BlockLeavesBuilder(), new BlockWoodBuilder(), new BlockWaterBuilder(), new BlockBushBuilder(), new BlockSandBuilder(),
        new BlockDeadbushBuilder(), new BlockCactusBuilder()
	};



	void Awake()
	{
		filter = GetComponent<MeshFilter>();
		coll = GetComponent<MeshCollider>();
        //filter.mesh.MarkDynamic();
        //coll.sharedMesh.MarkDynamic(); //TODO: Activate these when block put is implemented.
	    isGenerating = false;
    }

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*if(update && !isGenerating)
		{
            print(transform.position);
			update = false;
			GreedyMesher();
		}*/
		

		//print(generatedBlocks);

		if(isGenerating && !generatorThread.IsAlive)
		{
            UpdateGeneratedChunk();
            isGenerating = false;
			Profiler.BeginSample("Mesher");
			GreedyMesher();
			Profiler.EndSample();
            update = false;
        }
	}

	public void StartGeneration()
	{
		generator = new TerrainGen();
		ThreadStart starter = delegate 
		{
			generator.ChunkGen(pos);
		};

		generatorThread = new Thread(starter);
		generatorThread.Start();

		isGenerating = true;
	}

	void UpdateGeneratedChunk()
	{
        blocks = generator.blocks;
	    biomes = generator.biomes;
	}

	public BlockType GetBlock(int x, int y, int z)
	{
		if (InRange(x) && InRange(y) && InRange(z))
		{
			return blocks[x + (y * chunkSize * chunkSize) + (z * chunkSize)];
		}
			
		return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
	}

    public BiomeType GetBlockBiome(int x, int z)
    {
        return biomes[x + (z * chunkSize)];
    }

    [MethodImplAttribute(256)]
    public static bool InRange(int index)
	{
		return !(index < 0 || index >= chunkSize);
	}

	public void SetBlock(int x, int y, int z, BlockType block)
	{
		if (InRange(x) && InRange(y) && InRange(z))
		{
			blocks[x + (y * chunkSize * chunkSize) + (z * chunkSize)] = block;
		}
		else
		{
			//world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
		}
	}

	void RenderMesh(MeshData meshData)
	{
		filter.mesh.Clear();
		filter.mesh.vertices = meshData.vertices.ToArray();
		filter.mesh.triangles = meshData.triangles.ToArray();

		filter.mesh.SetUVs(0, meshData.uv);
		filter.mesh.RecalculateNormals();
        filter.mesh.SetUVs(1, meshData.texType);

        coll.sharedMesh = null;
		Mesh mesh = new Mesh();
		mesh.vertices = meshData.colVertices.ToArray();
		mesh.triangles = meshData.colTriangles.ToArray();
		mesh.RecalculateNormals();

		coll.sharedMesh = mesh;
	}

	void GreedyMesher()
	{
		MeshData meshdata = new MeshData();

		BlockType tempBlock1, tempBlock2;

		int i, j, k, l, u, v, n;

		Direction direction = Direction.down;
		int[] blockPos = new int[] { 0, 0, 0 };
		int[] blockOffset = new int[] { 0, 0, 0 };

		int[] mask = new int[Chunk.chunkSize * Chunk.chunkSize];
		//First pass is for front face, second is back face
		for(bool frontFace = true, backFace = false; backFace != frontFace; frontFace = frontFace && backFace, backFace = !backFace)
		{
			//Loop over 3 dimension
			for(int dim = 0; dim < 3; dim++)
			{
				u = (dim + 1) % 3;
				v = (dim + 2) % 3;

				blockPos[0] = 0; blockPos[1] = 0; blockPos[2] = 0;
				blockOffset[0] = 0; blockOffset[1] = 0; blockOffset[2] = 0; blockOffset[dim] = 1;

				switch(dim)
				{
					case 0:
						{
							direction = frontFace ? Direction.west : Direction.east;
						}
						break;
					case 1:
						{
							direction = frontFace ? Direction.down : Direction.up;
						}
						break;
					case 2:
						{
							direction = frontFace ? Direction.south : Direction.north;
						}
						break;
				}

				for(blockPos[dim] = frontFace ? -1 : 0; blockPos[dim] < (frontFace ? Chunk.chunkSize - 1 : Chunk.chunkSize);)
				{
					n = 0;

					for(blockPos[v] = 0; blockPos[v] < Chunk.chunkSize; blockPos[v]++)
					{
						for(blockPos[u] = 0; blockPos[u] < Chunk.chunkSize; blockPos[u]++)
						{
						    int tempX1 = blockPos[0], tempY1 = blockPos[1], tempZ1 = blockPos[2];
						    int tempX2 = blockPos[0] + blockOffset[0],
						        tempY2 = blockPos[1] + blockOffset[1],
						        tempZ2 = blockPos[2] + blockOffset[2];

							tempBlock1 = blockPos[dim] >= 0 ? GetBlock(tempX1, tempY1, tempZ1) : BlockType.Air;
							tempBlock2 = blockPos[dim] < Chunk.chunkSize - 1 ? GetBlock(tempX2, tempY2, tempZ2) : BlockType.Air;

						    BlockBuilder tempBlockBuilder1 = blockBuilders[(int)tempBlock1];
						    BlockBuilder tempBlockBuilder2 = blockBuilders[(int)tempBlock2];

						    mask[n++] = tempBlockBuilder1.RenderingEquality(tempBlockBuilder2, frontFace,
						        frontFace ? GetBlockBiome(tempX2, tempZ2) : GetBlockBiome(tempX1, tempZ1), direction);
						}
					}

					blockPos[dim]++;
					n = 0;

					int width = 0, height = 0, widthControlForHeight = 0;
					for(j = 0; j < Chunk.chunkSize; j++)
					{
						for(i = 0; i < Chunk.chunkSize; )
						{
							if (mask[n] != 0)
							{
								int maskType = mask[n];
								for (width = 1; width + i < Chunk.chunkSize &&
												mask[n + width] != 0 &&
												mask[n + width] == maskType; width++) ;

								bool done = false;

								for (height = 1; j + height < Chunk.chunkSize; height++)
								{
									for (widthControlForHeight = 0; widthControlForHeight < width; widthControlForHeight++)
									{
										int maskIndex = n + widthControlForHeight + height * Chunk.chunkSize;
										if (mask[maskIndex] == 0 || mask[maskIndex] != maskType)
										{
											done = true;
											break;
										}
									}

									if (done) { break; }
								}


								blockPos[u] = i;
								blockPos[v] = j;

								int x, y, z;
							    int blockTypeInt = maskType & 0x00FF;
							    BiomeType biome = (BiomeType) ((maskType & 0xFF00) >> 16);
							    BlockBuilder necessaryBuilder = blockBuilders[blockTypeInt];


                                if (!frontFace || necessaryBuilder.OVERRIDE_OFFSET) //NOTE: We want both water surfaces at same coord. To do that we cannot let block pos to use offset.
								{                                                  
									x = blockPos[0] - blockOffset[0];
									y = blockPos[1] - blockOffset[1];
									z = blockPos[2] - blockOffset[2];
								}
								else
								{
									x = blockPos[0];
									y = blockPos[1];
									z = blockPos[2];

								}

							    meshdata = necessaryBuilder.GreedyDirectionData(x, y, z, width, height, direction, meshdata, biome);

								for (l = 0; l < height; ++l)
								{
									for (k = 0; k < width; ++k)
									{
										mask[n + k + l * Chunk.chunkSize] = 0;
									}
								}

								n += width;
								i += width;
							}
							else
							{
								i++;
								n++;
							}
						}
					}
				}
			}
		}
		RenderMesh(meshdata);
	}

}
