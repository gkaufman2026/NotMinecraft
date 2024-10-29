using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour {
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

    public void InitalizeChunk(ChunkData chunk) {
        ChunkData = chunk;
    }

    private void RenderMesh(MeshData meshData) {
        mesh.Clear();

        mesh.subMeshCount = 1;
        mesh.vertices = meshData.vertices.ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        mesh.uv = meshData.uv.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new() {
            vertices = meshData.collidierVertices.ToArray(),
            triangles = meshData.collidierTriangles.ToArray()
        };
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
    private void OnDrawGizmos() {
        if (showGizmo) {
            if (Application.isPlaying && ChunkData != null) {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(
                    transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData.chunkSize / 2f),
                    new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize)
                );
            }
        }
    }
#endif
}
