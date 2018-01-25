using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Priority_Queue;

public class LoadChunk : MonoBehaviour
{
	public World world;

    WorldPos playerPos;
	
	SimplePriorityQueue<WorldPos> buildQueue = new SimplePriorityQueue<WorldPos>();
	
	Queue<WorldPos> chunksToDelete = new Queue<WorldPos>();

	int timer = 0;

	const int PRIORITY_CHUNK_POS_COUNT = 45;
	const int PRIORITY_CHUNK_LOOKUP_LIMIT = 5;
	int priorityChunkPosIndex = 0;

	const int OTHER_CHUNK_LOOKUP_MIN_LIMIT = 3;
    const int OTHER_CHUNK_POS_COUNT = 193 - PRIORITY_CHUNK_LOOKUP_LIMIT;
	int otherChunkPosIndex = PRIORITY_CHUNK_POS_COUNT;

	const int MAXIMUM_DISTANCE = 512;
    #region chunkPosition
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
    #endregion

    // Use this for initialization
    void Start ()
	{
        playerPos = new WorldPos(Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize,
                                        Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize,
                                        Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize);
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
		WorldPos currentPlayerPos = new WorldPos(Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize,
										Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize,
										Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize);

        if(currentPlayerPos != playerPos)
        {
            playerPos = currentPlayerPos;
            UpdatePriority();
        }

		int prevQueueCount = buildQueue.Count;
		for(int i = 0; i < PRIORITY_CHUNK_LOOKUP_LIMIT; i++, 
			priorityChunkPosIndex = (priorityChunkPosIndex + 1) % PRIORITY_CHUNK_POS_COUNT)
		{
			CreateChunkRow(playerPos, priorityChunkPosIndex);
		}

        int leftover = buildQueue.Count - prevQueueCount;
		for(int i = 0; i < OTHER_CHUNK_LOOKUP_MIN_LIMIT + leftover; i++,
            otherChunkPosIndex = otherChunkPosIndex < chunkPositions.Length - 1 ? otherChunkPosIndex + 1 : PRIORITY_CHUNK_POS_COUNT)
        {
            CreateChunkRow(playerPos, otherChunkPosIndex);
        }
	}

	[MethodImplAttribute(256)]
	void CreateChunkRow(WorldPos playerPos, int index)
	{
		for (int y = playerPos.y - Chunk.chunkSize * 4; y <= playerPos.y + Chunk.chunkSize * 4; y += Chunk.chunkSize)
		{
			WorldPos newChunkPos = new WorldPos(chunkPositions[index].x * Chunk.chunkSize + playerPos.x,
												y,
												chunkPositions[index].z * Chunk.chunkSize + playerPos.z);

			Chunk newChunk = world.GetChunk(newChunkPos.x, y, newChunkPos.z);

			if (newChunk != null || buildQueue.Contains(newChunkPos))
			{
				continue;
			}

			float priority = WorldPos.EuclideanDistance(playerPos, newChunkPos);
			buildQueue.Enqueue(newChunkPos, priority);
		}

	}

    void UpdatePriority()
    {
        foreach(WorldPos pos in buildQueue)
        {
            buildQueue.UpdatePriority(pos, WorldPos.EuclideanDistance(playerPos, pos));
        }
    }

	void LoadAndRenderChunks()
	{
		int i;
		if(buildQueue.Count != 0)
		{
			for(i = 0; i < buildQueue.Count && i < 5; i++)
			{
				WorldPos chunkPos = buildQueue.Dequeue();
				if (world.GetChunk(chunkPos.x, chunkPos.y, chunkPos.z) == null)
				{
					world.CreateChunk(chunkPos.x, chunkPos.y, chunkPos.z);
				}

				//print(chunkPos);
			}
		}		
	}

	bool DeleteChunks()
	{
		if (timer == 10)
		{
			foreach (var chunk in world.chunks)
			{
				//Player position is used so it is better to use Vector3 dist instead of WorldPos dist.
				float distance = Vector3.Distance(
					new Vector3(chunk.Value.pos.x, 0, chunk.Value.pos.z),
					new Vector3(transform.position.x, 0, transform.position.z));
				if (distance > MAXIMUM_DISTANCE)
				{
					chunksToDelete.Enqueue(chunk.Key);
				}
					
			}
			foreach (var chunk in chunksToDelete)
			{
				world.DestroyChunk(chunk.x, chunk.y, chunk.z);
			}

			timer = 0;
			chunksToDelete.Clear();
			return true;
		}
		timer++;
		return false;
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
