using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mesh;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;

    public bool showGizmo = false;

    public ChunkData ChunkData { get; private set; }

    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    public void InitalzieChunk(ChunkData chunk) {
        ChunkData = chunk;
    }

    private void RenderMesh(MeshData data) {
        mesh.Clear();

        mesh.subMeshCount = 2;
        mesh.vertices = data.vertices.Concat(data.waterMesh.vertices).ToArray();

        mesh.SetTriangles(data.triangles.ToArray(), 0);
        mesh.SetTriangles(data.waterMesh.triangles.Select(val => val + data.vertices.Count).ToArray(), 1);

        mesh.uv = data.uv.Concat(data.waterMesh.uv).ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new();
        collisionMesh.vertices = data.collidierVertices.ToArray();
        collisionMesh.triangles = data.collidierTriangles.ToArray();
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh;

    }
}
