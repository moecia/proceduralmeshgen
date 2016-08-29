﻿using UnityEngine;
using System.Collections.Generic;

public class MeshBuilder : MonoBehaviour
{

    public Material material;
    public int recursionLevel;

    private struct triangle
    {
        public int A;
        public int B;
        public int C;

        public triangle(int a, int b, int c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", A, B, C);
        }
    }

    // Use this for initialization
    void Start()
    {
        BuildGameObject();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private GameObject BuildGameObject()
    {
        var planet = new GameObject("planet", typeof(MeshFilter), typeof(MeshRenderer), typeof(RotatePlanet));

        var meshFilter = planet.GetComponent<MeshFilter>();
        var meshRenderer = planet.GetComponent<MeshRenderer>();



        var mesh = BuildMesh();

        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        return planet;
    }

    private Vector3 SmoothCurve(Vector3 v)
    {
        return SmoothCurve(v.x, v.y, v.z);
    }

    private Vector3 SmoothCurve(float x, float y, float z)
    {
        var length = Mathf.Sqrt((x * x) + (y * y) + (z * z));
        var smoothPoint = new Vector3(x / length, y / length, z / length);

        return smoothPoint;
    }

    private Vector3 GetMiddlePoint(Vector3 p1, Vector3 p2)
    {
        return new Vector3((p1.x + p2.x) / 2f,
                           (p1.y + p2.y) / 2f,
                           (p1.z + p2.z) / 2f);
    }

    private int[] ConvertToMeshFilterTriangles(triangle[] triangles)
    {
        var result = new List<int>();

        for (int i = 0; i < triangles.Length; i++)
        {
            result.Add(triangles[i].A);
            result.Add(triangles[i].B);
            result.Add(triangles[i].C);
        }

        return result.ToArray();
    }

    private Mesh BuildMesh()
    {
        var mesh = new Mesh();

        // create 12 vertices of a icosahedron
        var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        var vertices = new List<Vector3>(12);
        var uvs = new List<Vector2>(12);

        vertices.Add(SmoothCurve(-1, t, 0));
        vertices.Add(SmoothCurve(1, t, 0));
        vertices.Add(SmoothCurve(-1, -t, 0));
        vertices.Add(SmoothCurve(1, -t, 0));

        vertices.Add(SmoothCurve(0, -1, t));
        vertices.Add(SmoothCurve(0, 1, t));
        vertices.Add(SmoothCurve(0, -1, -t));
        vertices.Add(SmoothCurve(0, 1, -t));

        vertices.Add(SmoothCurve(t, 0, -1));
        vertices.Add(SmoothCurve(t, 0, 1));
        vertices.Add(SmoothCurve(-t, 0, -1));
        vertices.Add(SmoothCurve(-t, 0, 1));

        uvs.Add(new Vector2(0.5f, 0.5f));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0));

        uvs.Add(new Vector2(0f, 0.2f));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0));

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0.5f, 0.2f));

        triangle[] triangles = {
            // 5 faces around point 0
            new triangle(0, 11, 5),
            new triangle(0,  5,  1),
            new triangle(0,  1,  7),
            new triangle(0,  7, 10),
            new triangle(0, 10, 11),

            // 5 adjacent faces
            new triangle( 1,  5,  9),
            new triangle( 5, 11,  4),
            new triangle(11, 10,  2),
            new triangle(10,  7,  6),
            new triangle( 7,  1,  8),

            // 5 faces around point 3
            new triangle(3, 9, 4),
            new triangle(3, 4, 2),
            new triangle(3, 2, 6),
            new triangle(3, 6, 8),
            new triangle(3, 8, 9),

            // 5 adjacent faces
            new triangle(4, 9, 5),
            new triangle(2, 4, 11),
            new triangle(6, 2, 10),
            new triangle(8, 6, 7),
            new triangle(9, 8, 1)
        };

        for (int i = 0; i < recursionLevel; i++)
        {
            var triangles2 = new List<triangle>();

            foreach (var face in triangles)
            {
                var AB = GetMiddlePoint(vertices[face.A], vertices[face.B]);
                vertices.Add(SmoothCurve(AB));
                int ab = vertices.Count - 1;
                //uvs.Add(new Vector2(0.5f, 1f));

                var BC = GetMiddlePoint(vertices[face.B], vertices[face.C]);
                vertices.Add(SmoothCurve(BC));
                int bc = vertices.Count - 1;
                //uvs.Add(new Vector2(0f, 0f));

                var CA = GetMiddlePoint(vertices[face.C], vertices[face.A]);
                vertices.Add(SmoothCurve(CA));
                int ca = vertices.Count - 1;
                //uvs.Add(new Vector2(1f, 1f));

                var tri = new triangle(face.A, ab, ca);
                //Debug.Log(tri);
                triangles2.Add(tri);
                
                triangles2.Add(new triangle(face.B, bc, ab));
                triangles2.Add(new triangle(face.C, ca, bc));
                triangles2.Add(new triangle(ab, bc, ca));
            }

            triangles = triangles2.ToArray();
        }

        //Vector2[] uvs = {
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),

        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),

        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0),
        //    new Vector2(0, 0)
        //};

        //Vector3[] normals = {
        //    Vector3.up,
        //    Vector3.up,
        //    Vector3.up,
        //    Vector3.up,

        //    Vector3.up,
        //    Vector3.up,
        //    Vector3.up,
        //    Vector3.up,

        //    Vector3.up,
        //    Vector3.up,
        //    Vector3.up,
        //    Vector3.up
        //};

        mesh.vertices = vertices.ToArray();
        mesh.triangles = ConvertToMeshFilterTriangles(triangles);
        //mesh.uv = uvs.ToArray();
        //mesh.normals = normals;

        return mesh;
    }

}