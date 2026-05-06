using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class StarMesh : MonoBehaviour
{
    public float outerRadius = 0.9f;
    public float innerRadius = 0.12f;
    public int   points      = 5;
    public Color starColor   = new Color(1f, 0.88f, 0.05f);

    void Awake() => Rebuild();

    public void Rebuild()
    {
        var verts2D = MakeStarVerts();
        GetComponent<PolygonCollider2D>().SetPath(0, verts2D);

        int      n    = verts2D.Length;
        var      v3   = new Vector3[n + 1];
        v3[0] = Vector3.zero;
        for (int i = 0; i < n; i++)
            v3[i + 1] = new Vector3(verts2D[i].x, verts2D[i].y, 0f);

        // CW winding from +Z so normal faces -Z (toward camera at z=-10)
        var tris = new int[n * 3];
        for (int i = 0; i < n; i++)
        {
            tris[i * 3]     = 0;
            tris[i * 3 + 1] = (i + 1) % n + 1;
            tris[i * 3 + 2] = i + 1;
        }

        var mesh = new Mesh { name = "StarMesh" };
        mesh.vertices  = v3;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", starColor);
        GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    Vector2[] MakeStarVerts()
    {
        int   total = points * 2;
        var   v     = new Vector2[total];
        float step  = Mathf.PI / points;
        float start = Mathf.PI / 2f;
        for (int i = 0; i < total; i++)
        {
            float a = start + i * step;
            float r = (i % 2 == 0) ? outerRadius : innerRadius;
            v[i] = new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r);
        }
        return v;
    }
}
