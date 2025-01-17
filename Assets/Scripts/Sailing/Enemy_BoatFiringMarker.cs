using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Enemy_BoatFiringMarker : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    public void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>(); // gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        meshFilter = gameObject.GetComponent<MeshFilter>();

        mesh = new Mesh();
    }

    public void SetMarkerDetails(float newOpacity, float newOffset)
    {
        meshRenderer.sharedMaterial.SetFloat("_Opacity", newOpacity);
        meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(0f, newOffset));
    }


    public void SetFiringZone(Vector3[] Corners) {
        //Don't operate on our original array as it'll change it when it needs to be used again (why? That's a C++ function not a C# function!)
        //so essentially we need to set the corners to the verticies
        List<Vector3> vertices = new List<Vector3>(); // new Vector3[Corners.Length];
        for (int i = 0; i < Corners.Length; i++)
        {
            vertices.Add(gameObject.transform.InverseTransformPoint(Corners[i]));
        }
 
        mesh.SetVertices(vertices);// .vertices = vertices;

        //We could do with a general equation to handle the possibility of multiple firing zones...
        int ZoneCount = Corners.Length / 4;
        int[] tris = new int[6 * ZoneCount];
        for (int i = 0; i < ZoneCount; i++)
        {
            // lower left triangle
            tris[i * 6 + 0] = i * 4 + 0;
            tris[i * 6 + 1] = i * 4 + 2;
            tris[i * 6 + 2] = i * 4 + 1;
            // upper right triangle
            tris[i * 6 + 3] = i * 4 + 2;
            tris[i * 6 + 4] = i * 4 + 3;
            tris[i * 6 + 5] = i * 4 + 1;
        }

        mesh.SetTriangles(tris, 0);// .triangles = tris;

        //Vector3[] normals = new Vector3[Corners.Length];
        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < Corners.Length; i++)
        {
            //vertices[i] = Vector3.up;
            normals.Add(Vector3.up);
        }

        //mesh.normals = normals;
        mesh.SetNormals(normals);
        /*
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        */
        //Vector2[] uv = new Vector2[Corners.Length];
        List<Vector2> uv = new List<Vector2>();
        for (int i = 0; i < ZoneCount; i++)
        {
            /*
            uv[i * 4 + 0] = new Vector2(0, 0);
            uv[i * 4 + 1] = new Vector2(1, 0);
            uv[i * 4 + 2] = new Vector2(0, 1);
            uv[i * 4 + 3] = new Vector2(1, 1);
            */
            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(1, 0));
            uv.Add(new Vector2(0, 1));
            uv.Add(new Vector2(1, 1));

        }
        //mesh.uv = uv;
        mesh.SetUVs(0, uv);
        meshFilter.mesh = mesh;
    }
}
