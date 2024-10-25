using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
    public List<Vector3> vertices = new();
    public List<Vector2> uv = new();
    public List<int> triangles = new();

    public List<Vector3> collidierVertices = new();
    public List<int> collidierTriangles = new();

    public MeshData waterMesh;
    private bool isMainMesh = true;

    public MeshData(bool isMainMesh) {
        if (isMainMesh) {
            waterMesh = new MeshData(false);
        }
    }
    
    public void AddQuadTriangles(bool canQuadGenerate) {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if (canQuadGenerate) {
            collidierTriangles.Add(triangles.Count - 4);
            collidierTriangles.Add(triangles.Count - 3);
            collidierTriangles.Add(triangles.Count - 2);
            collidierTriangles.Add(triangles.Count - 4);
            collidierTriangles.Add(triangles.Count - 2);
            collidierTriangles.Add(triangles.Count - 1);
        }
    }
}
