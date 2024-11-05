using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class GenerateQuad : MonoBehaviour
{
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
        vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 1, 0);
        vertices[2] = new Vector3(1, 1, 0);
        vertices[3] = new Vector3(1, 0, 0);

        triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;
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
