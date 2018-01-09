using UnityEngine;
using System.Collections.Generic;

public class ObjectChunk
{
    WorldPos originBlockPos;
    List<WorldPos> relativeBlocksPos;

    public ObjectChunk(RaycastHit hit)
    {
        originBlockPos = Terrain.GetBlockPos(hit, true);
        relativeBlocksPos = new List<WorldPos>();
        relativeBlocksPos.Add(new WorldPos());
    }

    public void addNewBlock(RaycastHit hit)
    {
        WorldPos relativeBlockPos = Terrain.GetBlockPos(hit, true) - originBlockPos;

        relativeBlocksPos.Add(relativeBlockPos);
    }

    public void copyNewObject(RaycastHit hit)
    {
        WorldPos origin = Terrain.GetBlockPos(hit, true);

        Chunk chunk = hit.collider.GetComponent<Chunk>();

        for(int i = 0; i < relativeBlocksPos.Count; i++)
        {
            Terrain.SetBlock(relativeBlocksPos[i] + origin, chunk, BlockType.Rock);
        }
    }

}
