using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    public void UpdateChunk(MeshData data) {
        RenderMesh(data);
    }

    public void UpdateChunk() {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData.chunkSize / 2f), new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize));
            }
        }
    }
#endif
}
