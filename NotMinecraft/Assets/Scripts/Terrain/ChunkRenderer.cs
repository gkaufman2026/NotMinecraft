using System.Linq;

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour {
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;

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
        mesh.subMeshCount = 2;
        Vector3[] vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();
        mesh.vertices = vertices;
        int[] landTriangles = meshData.triangles.ToArray();
        int[] waterTriangles = meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray();

        mesh.SetTriangles(landTriangles, 0);
        mesh.SetTriangles(waterTriangles, 1);

        mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = null; 
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.collidierVertices.ToArray();
        collisionMesh.triangles = meshData.collidierTriangles.ToArray();
        collisionMesh.RecalculateNormals();
        meshCollider.sharedMesh = collisionMesh; 
    }

    public void UpdateChunk(MeshData data) {
        RenderMesh(data);
    }

    public void UpdateChunk() {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    // For Scene Viewing, Will Draw Cube For Chunk Visual
    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (showGizmo) {
            if (Application.isPlaying && ChunkData != null) {
                Gizmos.color = (UnityEditor.Selection.activeObject == gameObject) ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 1, 0.4f);
                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData.chunkSize / 2f), new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize));
            }
        }
#endif
    }
}
