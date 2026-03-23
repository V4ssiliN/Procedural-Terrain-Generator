using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int size = heightMap.GetLength(0);

        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        MeshData meshData = new MeshData(size);
        int vertexIndex = 0;

        for (int y = 0; y < size; y += 1)
        {
            for (int x = 0; x < size; x += 1)
            {
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;

                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, height, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)size, y / (float)size);
                
                if(x< size - 1 && y< size - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                    meshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                }
                
                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int verticesPerLine)
    {
        vertices = new Vector3[verticesPerLine*verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}