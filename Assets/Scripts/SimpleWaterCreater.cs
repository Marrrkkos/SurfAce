using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleWaterCreator : MonoBehaviour
{
    public int xSize = 100; // Anzahl der Segmente in X
    public int zSize = 100; // Anzahl der Segmente in Z
    public float scale = 1f; // Abstand zwischen den Punkten

    private Vector3[] vertices;
    private Mesh mesh;

    void Awake()
    {
        Generate();
    }

    void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Water Grid";

        // 1. Vertices (Punkte) erstellen
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x * scale, 0, z * scale);
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        // 2. Triangles (Dreiecke) verbinden
        int[] triangles = new int[xSize * zSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
