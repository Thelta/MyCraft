using UnityEngine;
using System.Threading;
using UnityEngine.Profiling;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    [HideInInspector]
	public BlockType[] blocks;
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
   
	static BlockBuilder[] blockBuilders = { null, new BlockAirBuilder(), new BlockBuilder(), new BlockGrassBuilder(),
        new BlockLeavesBuilder(), new BlockWoodBuilder(), new BlockWaterBuilder(), new BlockBushBuilder(), new BlockSandBuilder() };



	void Awake()
	{
		filter = GetComponent<MeshFilter>();
		coll = GetComponent<MeshCollider>();

		blocks = new BlockType[chunkSize * chunkSize * chunkSize];
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
            Profiler.BeginSample("Builder");

            UpdateGeneratedChunk();
            isGenerating = false;
			GreedyMesher();
            update = false;
            Profiler.EndSample();
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
	}

	public BlockType GetBlock(int x, int y, int z)
	{
		if (InRange(x) && InRange(y) && InRange(z))
		{
			return blocks[x + (y * chunkSize * chunkSize) + (z * chunkSize)];
		}
			
		return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
	}

	public static bool InRange(int index)
	{
		if (index < 0 || index >= chunkSize)
			return false;

		return true;
	}

	public void SetBlock(int x, int y, int z, BlockType block)
	{
		if (InRange(x) && InRange(y) && InRange(z))
		{
			blocks[x + (y * chunkSize * chunkSize) + (z * chunkSize)] = block;
		}
		else
		{
			world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
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

				for(blockPos[dim] = -1; blockPos[dim] < Chunk.chunkSize;)
				{
					n = 0;

					for(blockPos[v] = 0; blockPos[v] < Chunk.chunkSize; blockPos[v]++)
					{
						for(blockPos[u] = 0; blockPos[u] < Chunk.chunkSize; blockPos[u]++)
						{
							tempBlock1 = blockPos[dim] >= 0 ? GetBlock(blockPos[0], blockPos[1], blockPos[2]) : BlockType.Air;
							tempBlock2 = blockPos[dim] < Chunk.chunkSize - 1 ? GetBlock(blockPos[0] + blockOffset[0],
																				blockPos[1] + blockOffset[1],
																				blockPos[2] + blockOffset[2]) : BlockType.Air;

							mask[n++] = (tempBlock1 == tempBlock2 && (frontFace ? tempBlock2 != BlockType.Bush : tempBlock1 != BlockType.Bush))
                                ? 1 : (frontFace ? (int)tempBlock2 : (int)tempBlock1); //Bushes are exception. We need to render all of them.
                                                                                        //TODO : I really need to carry some stuff to block builders.

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

								if(!frontFace || maskType == (int)BlockType.Water) //NOTE: We want both water surfaces at same coord. To do that we cannot let block pos to use offset.
								{                                                  // TODO: It would be great to seperate some parts of greedy mesher to block builders like calculatng surface positions etc. 
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

								//direction = Mathf.Sign(maskType) > 0 ? direction : (Direction)((int)direction * -1);

								meshdata = blockBuilders[maskType].GreedyDirectionData(x, y, z, width, height, direction, meshdata);

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
