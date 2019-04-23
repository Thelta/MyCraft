﻿//#define debug_chunk

using UnityEngine;
using System.Collections.Generic;


public class World : MonoBehaviour, IBlockHelper
{

    [HideInInspector]
	public Dictionary<WorldPos, Chunk> chunks;
	public GameObject chunkPrefab;

    void Awake()
	{
        chunks = new Dictionary<WorldPos, Chunk>();
#if debug_chunk
        Debug.LogWarning("YOU ARE IN CHUNK DEBUG MODE");
#endif
	}



    void Start ()
	{
#if debug_chunk
        CreateChunk(0, 0, 0);


#endif

    }
	
	void Update ()
	{

	}

	public void CreateChunk(int x, int y, int z)
	{
		WorldPos worldPos = new WorldPos(x, y, z);

		//Instantiate the chunk at the coordinates using the chunk prefab
		GameObject newChunkObject = Instantiate(
						chunkPrefab, new Vector3(x, y, z),
						Quaternion.identity
					) as GameObject;

		Chunk newChunk = newChunkObject.GetComponent<Chunk>();

		newChunk.pos = worldPos;
		newChunk.world = this;

		//Add it to the chunks dictionary with the position as the key
		chunks.Add(worldPos, newChunk);
		newChunk.StartGeneration();

		//var terrainGen = new TerrainGen();
		//newChunk = terrainGen.ChunkGen(newChunk);

		//newChunk.SetBlocksUnmodified();

		//bool loaded = Serialization.Load(newChunk);
	}

	public Chunk GetChunk(int x, int y, int z)
	{
		WorldPos pos = new WorldPos();
		float multiple = Chunk.chunkSize;
		pos.x = Mathf.FloorToInt(x / multiple) * Chunk.chunkSize;
		pos.y = Mathf.FloorToInt(y / multiple) * Chunk.chunkSize;
		pos.z = Mathf.FloorToInt(z / multiple) * Chunk.chunkSize;

		Chunk containerChunk = null;

		chunks.TryGetValue(pos, out containerChunk); 

		return containerChunk;
	}

	public BlockType GetBlock(int x, int y, int z)
	{
		Chunk containerChunk = GetChunk(x, y, z);

		if (containerChunk != null)
		{
			BlockType block = containerChunk.GetBlock(
				x - containerChunk.pos.x,
				y - containerChunk.pos.y,
				z - containerChunk.pos.z);

			return block;
		}
		else
		{
			return BlockType.Air;
		}

	}

	public void SetBlock(int x, int y, int z, BlockType block)
	{
		Chunk chunk = GetChunk(x, y, z);

		if (chunk != null)
		{
			chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
			chunk.update = true;

			UpdateIfEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z));
			UpdateIfEqual(x - chunk.pos.x, Chunk.chunkSize - 1, new WorldPos(x + 1, y, z));
			UpdateIfEqual(y - chunk.pos.y, 0, new WorldPos(x, y - 1, z));
			UpdateIfEqual(y - chunk.pos.y, Chunk.chunkSize - 1, new WorldPos(x, y + 1, z));
			UpdateIfEqual(z - chunk.pos.z, 0, new WorldPos(x, y, z - 1));
			UpdateIfEqual(z - chunk.pos.z, Chunk.chunkSize - 1, new WorldPos(x, y, z + 1));
		}
	}

	public void DestroyChunk(int x, int y, int z)
	{
		Chunk chunk = null;
		if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
		{
			Object.Destroy(chunk.gameObject);
			chunks.Remove(new WorldPos(x, y, z));
		}
	}

	void UpdateIfEqual(int value1, int value2, WorldPos pos)
	{
		if (value1 == value2)
		{
			Chunk chunk = GetChunk(pos.x, pos.y, pos.z);
			if (chunk != null)
				chunk.update = true;
		}
	}
}
