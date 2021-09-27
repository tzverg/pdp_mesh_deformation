using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCubeGenerator : MonoBehaviour
{
    [SerializeField] private int sizeX, sizeY, sizeZ;
    [SerializeField] private int roundness;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;

    private void Awake() 
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Sphere";

        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = (sizeX + sizeY + sizeZ - 3) * 4;
        int faceVertices = (
            (sizeX - 1) * (sizeY - 1) +
            (sizeX - 1) * (sizeZ - 1) +
            (sizeY - 1) * (sizeZ - 1)) * 2;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];

        int v = 0;
        for (int y = 0; y <= sizeY; y++)
        {
            for (int x = 0; x <= sizeX; x++)
            {
                SetVertex(v++, x, y, 0);
            }
            for (int z = 1; z <= sizeZ; z++)
            {
                SetVertex(v++, sizeX, y, z);
            }
            for (int x = sizeX - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, sizeZ);
            }
            for (int z = sizeZ - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);
            }
        }
        for (int z = 1; z < sizeZ; z++)
        {
            for (int x = 1; x < sizeX; x++)
            {
                SetVertex(v++, x, sizeY, z);
            }
        }
        for (int z = 1; z < sizeZ; z++)
        {
            for (int x = 1; x < sizeX; x++)
            {
                SetVertex(v++, x, 0, z);
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    private void CreateTriangles()
    {
        int[] trianglesX = new int[(sizeY * sizeZ) * 12];
        int[] trianglesY = new int[(sizeX * sizeZ) * 12];
        int[] trianglesZ = new int[(sizeX * sizeY) * 12];

        int ring = (sizeX + sizeZ) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;

        for (int y = 0; y < sizeY; y++, v++)
        {
            for (int q = 0; q < sizeX; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < sizeZ; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < sizeX; q++, v++)
            {
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
            }
            for (int q = 0; q < sizeZ - 1; q++, v++)
            {
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
            }
            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesX, 0);
        mesh.SetTriangles(trianglesY, 1);
        mesh.SetTriangles(trianglesZ, 2);
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * sizeY;
        for (int x = 0; x < sizeX - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (sizeY + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < sizeZ - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + sizeX - 1);
            for (int x = 1; x < sizeX - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + sizeX - 1, vMid + sizeX);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + sizeX - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < sizeX - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (sizeX - 1) * (sizeZ - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < sizeX - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= sizeX - 2;
        int vMax = v + 2;

        for (int z = 1; z < sizeZ - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + sizeX - 1, vMin + 1, vMid);
            for (int x = 1; x < sizeX - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + sizeX - 1, vMid + sizeX, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + sizeX - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < sizeX - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        if (x < roundness)
        {
            inner.x = roundness;
        }
        else if (x > sizeX - roundness)
        {
            inner.x = sizeX - roundness;
        }
        if (y < roundness)
        {
            inner.y = roundness;
        }
        else if (y > sizeY - roundness)
        {
            inner.y = sizeY - roundness;
        }
        if (z < roundness)
        {
            inner.z = roundness;
        }
        else if (z > sizeZ - roundness)
        {
            inner.z = sizeZ - roundness;
        }

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = inner + normals[i] * roundness;
    }

    private void OnDrawGizmos() 
    {
        if(vertices == null)
            return;

        for (int cnt = 0; cnt < vertices.Length; cnt++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertices[cnt], 0.1F);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(vertices[cnt], normals[cnt]);
        }
    }
}
