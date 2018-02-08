# MyCraft
This game is currently in development.

In this project, my aim is developing a performant and robust sandbox procedural generation game akin to Minecraft in Unity3D. 

For terrain generation, this game uses simplex noise. Each chunk's generation is done in seperate threads. 
Textures are stored in texture arrays instead of tile maps, so new textures are added easily and avoid box border problem. 
While chunk meshing aren't multithreaded, after biome generation is finished, they will be next. 
For chunk meshing, I currently using Mikola Lysenko's greedy meshing algorithm.

Note: Latest commit is in biome_generation branch.
