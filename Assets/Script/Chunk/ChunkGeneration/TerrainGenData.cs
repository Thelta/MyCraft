﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenData
{
	public int x;
	public int y;
	public int z;

	public BlockType block;

	public bool replace;

	public TerrainGenData(int x, int y, int z, bool replace, BlockType block)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.replace = replace;
		this.block = block;
	}
}