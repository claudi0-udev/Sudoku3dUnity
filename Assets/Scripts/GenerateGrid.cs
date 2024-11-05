using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateGrid : MonoBehaviour
{
    public float quadScale = 1;
    public float quadPivot = .5f;
    public Vector2 gridSize = new Vector2(9, 9);
    private Mesh mesh;
    int[] triangles;
    Vector3[] vertices;

    private void Start()
    {
        GenerateMeshData();
        CreateMesh();
    }

    void GenerateMeshData()
    {
        int gridX = (int)gridSize.x;
        int gridY = (int)gridSize.y;

        int v = 0;
        int t = 0;

        vertices = new Vector3[(gridX + 1) * (gridY + 1)];

        for(int y = 0; y <= gridY; y++)
        {
            for(int x = 0; x <= gridX; x++)
            {
                vertices[v] = new Vector3((x - (gridX * quadPivot)) * quadScale, (y - (gridY * quadPivot)) * quadScale);
                v++;
            }
        }

        triangles = new int[gridX * gridY * 6];
        v = 0;

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                triangles[t + 0] = v + 0;
                triangles[t + 1] = v + gridX + 1;
                triangles[t + 2] = v + 1;
                triangles[t + 3] = v + 1;
                triangles[t + 4] = v + gridY + 1;
                triangles[t + 5] = v + gridY + 2;

                v++;
                t += 6;
            }
            v++;
        }
       
    }

    void CreateMesh()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
