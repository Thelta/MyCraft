using UnityEngine;
using System.Threading;

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
		rendered = true;
		if(update && !isGenerating)
		{
			update = false;
			UpdateChunk();
		}

		if(isGenerating)
		{
			UpdateGeneratedChunk();
			rendered = false;
		}
		//print(generatedBlocks);
		if(generatedBlocks >= 16 * 16 * 16 && isGenerating && !generatorThread.IsAlive && generator.dataQueue.Count == 0)
		{
			isGenerating = false;
			UpdateChunk();
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
}
