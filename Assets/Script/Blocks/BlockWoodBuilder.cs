﻿using UnityEngine;
using System.Collections;
using System;

public class BlockWoodBuilder : BlockBuilder
{
    public BlockWoodBuilder(): base()
    {
        type = BlockType.Wood;
    }
    public override Tile TexturePosition(Direction direction)
    {
        Tile tile = new Tile();
        switch (direction)
        {
            case Direction.up:
                tile.x = 2;
                tile.y = 1;
                return tile;
            case Direction.down:
                tile.x = 2;
                tile.y = 1;
                return tile;
        }
        tile.x = 1;
        tile.y = 1;
        return tile;
    }
}