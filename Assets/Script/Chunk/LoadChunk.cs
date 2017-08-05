using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadChunk : MonoBehaviour
{
	public World world;
	
	List<WorldPos> updateList = new List<WorldPos>();
	List<WorldPos> buildList = new List<WorldPos>();
	List<WorldPos> chunksToDelete = new List<WorldPos>();

	int timer = 0;

	static WorldPos[] chunkPositions = {   new WorldPos( 0, 0,  0), new WorldPos(-1, 0,  0), new WorldPos( 0, 0, -1), new WorldPos( 0, 0,  1), new WorldPos( 1, 0,  0),
							 new WorldPos(-1, 0, -1), new WorldPos(-1, 0,  1), new WorldPos( 1, 0, -1), new WorldPos( 1, 0,  1), new WorldPos(-2, 0,  0),
							 new WorldPos( 0, 0, -2), new WorldPos( 0, 0,  2), new WorldPos( 2, 0,  0), new WorldPos(-2, 0, -1), new WorldPos(-2, 0,  1),
							 new WorldPos(-1, 0, -2), new WorldPos(-1, 0,  2), new WorldPos( 1, 0, -2), new WorldPos( 1, 0,  2), new WorldPos( 2, 0, -1),
							 new WorldPos( 2, 0,  1), new WorldPos(-2, 0, -2), new WorldPos(-2, 0,  2), new WorldPos( 2, 0, -2), new WorldPos( 2, 0,  2),
							 new WorldPos(-3, 0,  0), new WorldPos( 0, 0, -3), new WorldPos( 0, 0,  3), new WorldPos( 3, 0,  0), new WorldPos(-3, 0, -1),
							 new WorldPos(-3, 0,  1), new WorldPos(-1, 0, -3), new WorldPos(-1, 0,  3), new WorldPos( 1, 0, -3), new WorldPos( 1, 0,  3),
							 new WorldPos( 3, 0, -1), new WorldPos( 3, 0,  1), new WorldPos(-3, 0, -2), new WorldPos(-3, 0,  2), new WorldPos(-2, 0, -3),
							 new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2), new WorldPos( 3, 0,  2),
							 new WorldPos(-4, 0,  0), new WorldPos( 0, 0, -4), new WorldPos( 0, 0,  4), new WorldPos( 4, 0,  0), new WorldPos(-4, 0, -1),
							 new WorldPos(-4, 0,  1), new WorldPos(-1, 0, -4), new WorldPos(-1, 0,  4), new WorldPos( 1, 0, -4), new WorldPos( 1, 0,  4),
							 new WorldPos( 4, 0, -1), new WorldPos( 4, 0,  1), new WorldPos(-3, 0, -3), new WorldPos(-3, 0,  3), new WorldPos( 3, 0, -3),
							 new WorldPos( 3, 0,  3), new WorldPos(-4, 0, -2), new WorldPos(-4, 0,  2), new WorldPos(-2, 0, -4), new WorldPos(-2, 0,  4),
							 new WorldPos( 2, 0, -4), new WorldPos( 2, 0,  4), new WorldPos( 4, 0, -2), new WorldPos( 4, 0,  2), new WorldPos(-5, 0,  0),
							 new WorldPos(-4, 0, -3), new WorldPos(-4, 0,  3), new WorldPos(-3, 0, -4), new WorldPos(-3, 0,  4), new WorldPos( 0, 0, -5),
							 new WorldPos( 0, 0,  5), new WorldPos( 3, 0, -4), new WorldPos( 3, 0,  4), new WorldPos( 4, 0, -3), new WorldPos( 4, 0,  3),
							 new WorldPos( 5, 0,  0), new WorldPos(-5, 0, -1), new WorldPos(-5, 0,  1), new WorldPos(-1, 0, -5), new WorldPos(-1, 0,  5),
							 new WorldPos( 1, 0, -5), new WorldPos( 1, 0,  5), new WorldPos( 5, 0, -1), new WorldPos( 5, 0,  1), new WorldPos(-5, 0, -2),
							 new WorldPos(-5, 0,  2), new WorldPos(-2, 0, -5), new WorldPos(-2, 0,  5), new WorldPos( 2, 0, -5), new WorldPos( 2, 0,  5),
							 new WorldPos( 5, 0, -2), new WorldPos( 5, 0,  2), new WorldPos(-4, 0, -4), new WorldPos(-4, 0,  4), new WorldPos( 4, 0, -4),
							 new WorldPos( 4, 0,  4), new WorldPos(-5, 0, -3), new WorldPos(-5, 0,  3), new WorldPos(-3, 0, -5), new WorldPos(-3, 0,  5),
							 new WorldPos( 3, 0, -5), new WorldPos( 3, 0,  5), new WorldPos( 5, 0, -3), new WorldPos( 5, 0,  3), new WorldPos(-6, 0,  0),
							 new WorldPos( 0, 0, -6), new WorldPos( 0, 0,  6), new WorldPos( 6, 0,  0), new WorldPos(-6, 0, -1), new WorldPos(-6, 0,  1),
							 new WorldPos(-1, 0, -6), new WorldPos(-1, 0,  6), new WorldPos( 1, 0, -6), new WorldPos( 1, 0,  6), new WorldPos( 6, 0, -1),
							 new WorldPos( 6, 0,  1), new WorldPos(-6, 0, -2), new WorldPos(-6, 0,  2), new WorldPos(-2, 0, -6), new WorldPos(-2, 0,  6),
							 new WorldPos( 2, 0, -6), new WorldPos( 2, 0,  6), new WorldPos( 6, 0, -2), new WorldPos( 6, 0,  2), new WorldPos(-5, 0, -4),
							 new WorldPos(-5, 0,  4), new WorldPos(-4, 0, -5), new WorldPos(-4, 0,  5), new WorldPos( 4, 0, -5), new WorldPos( 4, 0,  5),
							 new WorldPos( 5, 0, -4), new WorldPos( 5, 0,  4), new WorldPos(-6, 0, -3), new WorldPos(-6, 0,  3), new WorldPos(-3, 0, -6),
							 new WorldPos(-3, 0,  6), new WorldPos( 3, 0, -6), new WorldPos( 3, 0,  6), new WorldPos( 6, 0, -3), new WorldPos( 6, 0,  3),
							 new WorldPos(-7, 0,  0), new WorldPos( 0, 0, -7), new WorldPos( 0, 0,  7), new WorldPos( 7, 0,  0), new WorldPos(-7, 0, -1),
							 new WorldPos(-7, 0,  1), new WorldPos(-5, 0, -5), new WorldPos(-5, 0,  5), new WorldPos(-1, 0, -7), new WorldPos(-1, 0,  7),
							 new WorldPos( 1, 0, -7), new WorldPos( 1, 0,  7), new WorldPos( 5, 0, -5), new WorldPos( 5, 0,  5), new WorldPos( 7, 0, -1),
							 new WorldPos( 7, 0,  1), new WorldPos(-6, 0, -4), new WorldPos(-6, 0,  4), new WorldPos(-4, 0, -6), new WorldPos(-4, 0,  6),
							 new WorldPos( 4, 0, -6), new WorldPos( 4, 0,  6), new WorldPos( 6, 0, -4), new WorldPos( 6, 0,  4), new WorldPos(-7, 0, -2),
							 new WorldPos(-7, 0,  2), new WorldPos(-2, 0, -7), new WorldPos(-2, 0,  7), new WorldPos( 2, 0, -7), new WorldPos( 2, 0,  7),
							 new WorldPos( 7, 0, -2), new WorldPos( 7, 0,  2), new WorldPos(-7, 0, -3), new WorldPos(-7, 0,  3), new WorldPos(-3, 0, -7),
							 new WorldPos(-3, 0,  7), new WorldPos( 3, 0, -7), new WorldPos( 3, 0,  7), new WorldPos( 7, 0, -3), new WorldPos( 7, 0,  3),
							 new WorldPos(-6, 0, -5), new WorldPos(-6, 0,  5), new WorldPos(-5, 0, -6), new WorldPos(-5, 0,  6), new WorldPos( 5, 0, -6),
							 new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5) };

	// Use this for initialization
	void Start ()
	{
		//world.CreateChunk(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(DeleteChunks())
		{
			return; 
		}
		FindChunksToLoad();
		LoadAndRenderChunks();
	}

	void FindChunksToLoad()
	{
		WorldPos playerPos = new WorldPos(Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize,
							Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize,
							Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize);

		if(updateList.Count == 0)
		{
			for(int i = 0; i  < chunkPositions.Length; i++)
			{
				WorldPos newChunkPos = new WorldPos(chunkPositions[i].x * Chunk.chunkSize + playerPos.x,
					 0,
					 chunkPositions[i].z * Chunk.chunkSize + playerPos.z);

				Chunk newChunk = world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z);

				if(newChunk != null && (newChunk.rendered || updateList.Contains(newChunkPos)))
				{
					continue;
				}
				for(int y = -4; y < 4; y++)
				{
					for(int x = newChunkPos.x - Chunk.chunkSize; x <= newChunkPos.x + Chunk.chunkSize; x += Chunk.chunkSize)
					{
						for(int z = newChunkPos.z - Chunk.chunkSize; z <= newChunkPos.z; z += Chunk.chunkSize)
						{
							buildList.Add(new WorldPos(x, y * Chunk.chunkSize, z));
						}
					}

					updateList.Add(new WorldPos(newChunkPos.x, y * Chunk.chunkSize, newChunkPos.z)); ;
				}
				return;
			}
		}
	}

	void BuildChunk(WorldPos pos)
	{
	   if(world.GetChunk(pos.x, pos.y, pos.z) == null)
		{
			world.CreateChunk(pos.x, pos.y, pos.z);
		}
	}
	
	void LoadAndRenderChunks()
	{
		int i;
		if(buildList.Count != 0)
		{
			for(i = 0; i < buildList.Count && i < 8; i++)
			{
				BuildChunk(buildList[0]);
				buildList.RemoveAt(0);
			}
			return;
		}
		
		if(updateList.Count != 0)
		{
			Chunk chunk = world.GetChunk(updateList[0].x, updateList[0].y, updateList[0].z);
			if(chunk != null && !chunk.isGenerating)
			{
				chunk.update = true;
			}

			updateList.RemoveAt(0);
		}
	}

	bool DeleteChunks()    //Change the void on this line to bool
	{
		if (timer == 10)
		{
			foreach (var chunk in world.chunks)
			{
				float distance = Vector3.Distance(
					new Vector3(chunk.Value.pos.x, 0, chunk.Value.pos.z),
					new Vector3(transform.position.x, 0, transform.position.z));
				if (distance > 512)
					chunksToDelete.Add(chunk.Key);
			}
			foreach (var chunk in chunksToDelete)
			{
				world.DestroyChunk(chunk.x, chunk.y, chunk.z);
			}

			timer = 0;
			chunksToDelete.Clear();
			return true;    //Add this line
		}
		timer++;
		return false;    //Add this line
	}
	/*
	void Start()
	{
		for(int x = -3; x < 4; x++)
		{
			for (int y = -3; y < 4; y++)
			{
				for (int z = -3; z < 4; z++)
				{
					world.CreateChunk(x * 16, y * 16, z * 16);
				}
			}
		}

		//world.CreateChunk(-48, -16, -48);
	}*/
	
}
