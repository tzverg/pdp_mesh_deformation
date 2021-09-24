using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomSphereGenerator : MonoBehaviour
{
    [SerializeField] private int sizeX, sizeY, sizeZ;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake() 
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Sphere";

        WaitForSeconds wait = new WaitForSeconds(0.05F);

        int cornerVertices = 8;
        int edgeVertices = (sizeX + sizeY + sizeZ - 3) * 4;
        int faceVertices = (
            (sizeX - 1) * (sizeY - 1) +
            (sizeX - 1) * (sizeZ - 1) +
            (sizeY - 1) * (sizeZ - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int y = 0; y <= sizeY; y++)
        {
            for (int x = 0; x <= sizeX; x++)
            {
                vertices[v++] = new Vector3(x, y, 0);
                yield return wait;
            }
            for (int z = 1; z <= sizeZ; z++)
            {
                vertices[v++] = new Vector3(sizeX, y, z);
                yield return wait;
            }
            for (int x = sizeX - 1; x >= 0; x--)
            {
                vertices[v++] = new Vector3(x, y, sizeZ);
                yield return wait;
            }
            for (int z = sizeZ - 1; z > 0; z--)
            {
                vertices[v++] = new Vector3(0, y, z);
                yield return wait;
            }
        }
        for (int z = 1; z < sizeZ; z++)
        {
            for (int x = 1; x < sizeX; x++)
            {
                vertices[v++] = new Vector3(x, sizeY, z);
                yield return wait;
            }
        }
        for (int z = 1; z < sizeZ; z++)
        {
            for (int x = 1; x < sizeX; x++)
            {
                vertices[v++] = new Vector3(x, 0, z);
                yield return wait;
            }
        }

        yield return wait;
    }

    private void OnDrawGizmos() 
    {
        if(vertices == null)
            return;

        Gizmos.color = Color.black;

        for (int cnt = 0; cnt < vertices.Length; cnt++)
        {
            Gizmos.DrawSphere(vertices[cnt], 0.1F);
        }
    }
}
