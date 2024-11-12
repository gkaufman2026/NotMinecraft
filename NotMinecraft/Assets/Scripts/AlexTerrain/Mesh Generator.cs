using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    // Generates the terrain mesh based upon the passed in parameters
    public static MeshData generateTerrainMesh(float[,] heightMap, float heightScalar, AnimationCurve _heightCurve, int lod)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int lodIncrement = (lod == 0) ? 1 : lod * 2;
        int verticesPerLine = (width - 1) / lodIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += lodIncrement)
        {
            for (int x = 0; x < width; x += lodIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMap[x, y] * heightScalar, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width, y/(float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.addTri(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.addTri(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        // Returns MeshData so threading can be used later
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] tris;
    public Vector2[] uvs;

    int triIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        tris = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    public void addTri(int a, int b, int c)
    {
        tris[triIndex] = a;
        tris[triIndex + 1] = b;
        tris[triIndex + 2] = c;
        triIndex += 3;
    }

    public Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
