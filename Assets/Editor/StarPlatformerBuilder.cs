using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class StarPlatformerBuilder
{
    // Palette
    static readonly Color Sky      = new Color(0.07f, 0.07f, 0.14f);
    static readonly Color Ground   = new Color(0.22f, 0.52f, 0.22f);
    static readonly Color Notch    = new Color(0.65f, 0.38f, 0.18f);
    static readonly Color Platform = new Color(0.22f, 0.52f, 0.22f);

    [MenuItem("Star Game/Build Level 01")]
    public static void Build()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        BuildCamera();
        BuildPlayer();
        BuildLevel();

        System.IO.Directory.CreateDirectory(Application.dataPath + "/Scenes");
        AssetDatabase.Refresh();
        EditorSceneManager.SaveScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene(),
            "Assets/Scenes/Level01_StarPlatformer.unity");
        Debug.Log("[StarPlatformerBuilder] Scene saved.");
    }

    static void BuildCamera()
    {
        var go  = new GameObject("Main Camera");
        go.tag  = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = 7f;
        cam.backgroundColor  = Sky;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.nearClipPlane    = 0.1f;
        cam.farClipPlane     = 100f;
        go.transform.position = new Vector3(0f, 3f, -10f);
        go.AddComponent<AudioListener>();
        go.AddComponent<CameraFollow>();
    }

    static void BuildPlayer()
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(-2f, 1.5f, 0f);

        var rb                   = go.AddComponent<Rigidbody2D>();
        rb.mass                  = 1f;
        rb.linearDamping         = 0.05f;
        rb.angularDamping        = 0.4f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        go.AddComponent<PolygonCollider2D>();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();

        var star = go.AddComponent<StarMesh>();
        star.Rebuild();

        go.AddComponent<StarController>();
    }

    static void BuildLevel()
    {
        var root = new GameObject("Level");

        // Flat starting ground — rolls right for ~3 seconds at normal pace
        Slab("Ground", root.transform, new Vector2(0f, -0.3f), 12f, 0.6f, Ground);

        // Five notches rising left-to-right, each 1.3 units higher
        // Spaced 2.5 units apart horizontally so each jump needs momentum
        float[] nx = { 7.5f, 10.0f, 12.5f, 15.0f, 17.5f };
        float[] ny = { 2.2f,  3.5f,  4.8f,  6.1f,  7.4f };
        for (int i = 0; i < 5; i++)
            Pin($"Notch{i + 1}", root.transform, new Vector2(nx[i], ny[i]), 0.28f, Notch);

        // Upper ground — first safe resting zone after the notches
        Slab("EndPlatform", root.transform, new Vector2(23f, 8.1f), 9f, 0.6f, Platform);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    static void Slab(string name, Transform parent, Vector2 center, float w, float h, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position   = new Vector3(center.x, center.y, 0f);
        go.transform.localScale = new Vector3(w, h, 1f);
        Object.DestroyImmediate(go.GetComponent<BoxCollider>());
        go.AddComponent<BoxCollider2D>();
        go.GetComponent<Renderer>().sharedMaterial = UnlitMat(color);
    }

    static void Pin(string name, Transform parent, Vector2 center, float radius, Color color)
    {
        // Visual: sphere primitive looks circular in orthographic view
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position   = new Vector3(center.x, center.y, 0f);
        go.transform.localScale = new Vector3(radius * 2f, radius * 2f, 0.2f);
        Object.DestroyImmediate(go.GetComponent<SphereCollider>());
        var col    = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f; // matches scale * 0.5 = radius
        go.GetComponent<Renderer>().sharedMaterial = UnlitMat(color);
    }

    static Material UnlitMat(Color color)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", color);
        return mat;
    }
}
