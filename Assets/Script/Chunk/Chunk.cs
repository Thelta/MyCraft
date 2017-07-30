using UnityEngine;
using System.Threading;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour {

	public Block[,,] blocks;
	public static int chunkSize = 16;
	[HideInInspector]
	public bool update = false;
	[HideInInspector]
	public World world;
	[HideInInspector]
	public WorldPos pos;
	[HideInInspector]
	public bool rendered;

	MeshFilter filter;
	MeshCollider coll;

	public bool isGenerating;   
	TerrainGen generator;
	Thread generatorThread;

	int generatedBlocks;



	void Awake()
	{
		filter = GetComponent<MeshFilter>();
		coll = GetComponent<MeshCollider>();

		blocks = new Block[chunkSize, chunkSize, chunkSize];
		isGenerating = false;
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
        Profiler.BeginSample("Updater");
		rendered = true;
		if(update && !isGenerating)
		{
			update = false;
			GreedyMesher();
		}
        Profiler.EndSample();


        Profiler.BeginSample("Generator");
		if(isGenerating)
		{
			UpdateGeneratedChunk();
			rendered = false;
		}
        Profiler.EndSample();
        //print(generatedBlocks);

        Profiler.BeginSample("Builder");
		if(isGenerating && !generatorThread.IsAlive && generator.dataQueue.Count == 0)
		{
			isGenerating = false;
			GreedyMesher();
			update = false;
		}
        Profiler.EndSample();


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
		generatedBlocks = 0;
	}

	void UpdateGeneratedChunk()
	{
		lock(generator.dataQueue)
		{
			if(generator.dataQueue.Count > 0)
			{
				for(int i = 0; i < generator.dataQueue.Count; i++)
				{
					TerrainGenData data;
					data = generator.dataQueue.Dequeue();
					int x = data.x, y = data.y, z = data.z;
					bool replaceBlock = data.replace;

					if (blocks[x, y, z] == null || replaceBlock)
					{
						blocks[x, y, z] = data.block;
						generatedBlocks++;
					}
				}
			}
		}
	}

	public Block GetBlock(int x, int y, int z)
	{
		if (InRange(x) && InRange(y) && InRange(z))
			return blocks[x, y, z];
		return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
	}

	public static bool InRange(int index)
	{
		if (index < 0 || index >= chunkSize)
			return false;

		return true;
	}

	public void SetBlock(int x, int y, int z, Block block)
	{
		if (InRange(x) && InRange(y) && InRange(z))
		{
			blocks[x, y, z] = block;
		}
		else
		{
			world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
		}
	}
	/*
	void UpdateChunk()  //Bir chunk'a update yapıldığında meshdata'yı yeniden oluşturur.
	{
		MeshData meshData = new MeshData();

		for(int x = 0; x < chunkSize; x++)
		{
			for(int y = 0; y < chunkSize; y++)
			{
				for(int z = 0; z < chunkSize; z++)
				{
						meshData = blocks[x, y, z].BlockData(this, x, y, z, meshData);
				}
			}
		}

		RenderMesh(meshData);
	}
	*/
	void RenderMesh(MeshData meshData)
	{
		filter.mesh.Clear();
		filter.mesh.vertices = meshData.vertices.ToArray();
		filter.mesh.triangles = meshData.triangles.ToArray();

		filter.mesh.uv = meshData.uv.ToArray();
		filter.mesh.RecalculateNormals();

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

		Block tempBlock1, tempBlock2;
        BlockAir tempAir = new BlockAir();

		int i, j, k, l, u, v, n;

		Direction direction = Direction.down;
		int[] blockPos = new int[] { 0, 0, 0 };
		int[] blockOffset = new int[] { 0, 0, 0 };

		int[] mask = new int[Chunk.chunkSize * Chunk.chunkSize];
		Profiler.BeginSample("mainLoop");
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
							tempBlock1 = blockPos[dim] >= 0 ? blocks[blockPos[0], blockPos[1], blockPos[2]] : tempAir;
							tempBlock2 = blockPos[dim] < Chunk.chunkSize - 1 ? blocks[blockPos[0] + blockOffset[0],
																				blockPos[1] + blockOffset[1],
																				blockPos[2] + blockOffset[2]] : tempAir;

							mask[n++] = tempBlock1.type == tempBlock2.type ? 0 : (frontFace ? (int)tempBlock2.type : (int)tempBlock1.type);

						}
					}

					blockPos[dim]++;
					n = 0;

					int width = 0, height = 0, widthControlForHeight = 0;
					Profiler.BeginSample("masker");
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

								if(!frontFace)
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

								//direction = Mathf.Sign(maskType) > 0 ? direction : (Direction)((int)direction * -1);

								meshdata = blocks[x, y, z].GreedyDirectionData(x, y, z, width - 1, height - 1, direction, meshdata);

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
					Profiler.EndSample();
				}
			}
		}
		Profiler.EndSample();
		RenderMesh(meshdata);
	}


}
