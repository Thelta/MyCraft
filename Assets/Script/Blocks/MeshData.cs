using UnityEngine;
using System.Collections.Generic;

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();
    public List<Vector2> texType = new List<Vector2>();

    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();

    bool newCollisionData = false;

    public MeshData() { }

    private void AddQuadTriangles()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 1);

        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if(newCollisionData)
        {
            colTriangles.Add(colVertices.Count - 4);
            colTriangles.Add(colVertices.Count - 3);
            colTriangles.Add(colVertices.Count - 1);
            colTriangles.Add(colVertices.Count - 3);
            colTriangles.Add(colVertices.Count - 2);
            colTriangles.Add(colVertices.Count - 1);
        }
    }

    public void AddVertex(Vector3 vertex, bool useRenderDataForColl) 
    {
        vertices.Add(vertex);

        if (useRenderDataForColl)
        {
            colVertices.Add(vertex);

            newCollisionData = true;
        }

        if(vertices.Count % 4 == 0)
        {
            AddQuadTriangles();
        }

    }

}
