using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

/// Run via: Peak Game ▶ Build City Scene
/// Procedurally creates Level01_City.unity — a rough medieval city with
/// outer walls, a main path, buildings, and a castle at the far end.
public static class CitySceneBuilder
{
    // ── Layout constants ──────────────────────────────────────────────────
    const float W           = 80f;   // city width  (x)
    const float L           = 120f;  // city length (z)
    const float WALL_H      = 10f;
    const float WALL_T      = 2f;
    const float GATE_W      = 8f;

    static readonly Color StoneLight  = new Color(0.62f, 0.54f, 0.44f);
    static readonly Color StoneDark   = new Color(0.48f, 0.41f, 0.34f);
    static readonly Color DirtGround  = new Color(0.36f, 0.28f, 0.20f);
    static readonly Color PathColor   = new Color(0.52f, 0.43f, 0.30f);

    // ── Entry point ───────────────────────────────────────────────────────
    [MenuItem("Peak Game/Build City Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateGround();
        CreateCityWalls();
        CreatePath();
        CreateBuildings();
        CreateCastle();
        CreateLighting();
        CreatePlayer();

        System.IO.Directory.CreateDirectory(Application.dataPath + "/Scenes");
        AssetDatabase.Refresh();

        string path = "Assets/Scenes/Level01_City.unity";
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"[CitySceneBuilder] Scene saved → {path}");
    }

    // ── Ground ────────────────────────────────────────────────────────────
    static void CreateGround()
    {
        var g = Box("Ground", new Vector3(0, -0.05f, L / 2f),
                    new Vector3(W + 20f, 0.1f, L + 30f), DirtGround);
        GameObjectUtility.SetStaticEditorFlags(g, StaticEditorFlags.BatchingStatic);
    }

    // ── City walls ────────────────────────────────────────────────────────
    static void CreateCityWalls()
    {
        var root = Empty("CityWalls");

        float halfOpen  = GATE_W / 2f;
        float sideW     = (W - GATE_W) / 2f;
        float sideCenter = halfOpen + sideW / 2f;

        // South wall — two halves flanking the gate
        WallPart("SouthWall_L", root, new Vector3(-sideCenter, WALL_H / 2f, 0),
                 new Vector3(sideW, WALL_H, WALL_T), StoneLight);
        WallPart("SouthWall_R", root, new Vector3( sideCenter, WALL_H / 2f, 0),
                 new Vector3(sideW, WALL_H, WALL_T), StoneLight);
        // Gate lintel
        WallPart("GateLintel", root, new Vector3(0, WALL_H - 1.5f, 0),
                 new Vector3(GATE_W, 3f, WALL_T), StoneDark);
        // Gate pillars
        WallPart("GatePillar_L", root, new Vector3(-halfOpen - 0.5f, WALL_H * 0.45f, 0),
                 new Vector3(1.5f, WALL_H * 0.9f, WALL_T * 1.5f), StoneDark);
        WallPart("GatePillar_R", root, new Vector3( halfOpen + 0.5f, WALL_H * 0.45f, 0),
                 new Vector3(1.5f, WALL_H * 0.9f, WALL_T * 1.5f), StoneDark);

        // North wall
        WallPart("NorthWall", root, new Vector3(0, WALL_H / 2f, L),
                 new Vector3(W, WALL_H, WALL_T), StoneLight);
        // East / West walls
        WallPart("EastWall",  root, new Vector3( W / 2f, WALL_H / 2f, L / 2f),
                 new Vector3(WALL_T, WALL_H, L), StoneLight);
        WallPart("WestWall",  root, new Vector3(-W / 2f, WALL_H / 2f, L / 2f),
                 new Vector3(WALL_T, WALL_H, L), StoneLight);

        // Corner towers
        Tower("Tower_SE", root, new Vector3( W / 2f, 0, 0));
        Tower("Tower_SW", root, new Vector3(-W / 2f, 0, 0));
        Tower("Tower_NE", root, new Vector3( W / 2f, 0, L));
        Tower("Tower_NW", root, new Vector3(-W / 2f, 0, L));
    }

    // ── Main path ─────────────────────────────────────────────────────────
    static void CreatePath()
    {
        Box("MainPath", new Vector3(0, 0.02f, L / 2f),
            new Vector3(GATE_W, 0.04f, L), PathColor);
    }

    // ── City buildings ────────────────────────────────────────────────────
    static void CreateBuildings()
    {
        var root = Empty("Buildings");

        // Left side
        Building(root, "Bldg_L1", new Vector3(-15f, 0, 22f),  new Vector3(10f,  8f, 12f));
        Building(root, "Bldg_L2", new Vector3(-20f, 0, 45f),  new Vector3(13f,  6f, 10f));
        Building(root, "Bldg_L3", new Vector3(-14f, 0, 68f),  new Vector3( 9f, 11f, 11f));
        Building(root, "Bldg_L4", new Vector3(-22f, 0, 88f),  new Vector3(11f,  7f,  9f));

        // Right side
        Building(root, "Bldg_R1", new Vector3( 15f, 0, 28f),  new Vector3(10f,  9f, 12f));
        Building(root, "Bldg_R2", new Vector3( 22f, 0, 52f),  new Vector3(14f,  6f, 10f));
        Building(root, "Bldg_R3", new Vector3( 16f, 0, 74f),  new Vector3( 9f, 12f, 10f));
        Building(root, "Bldg_R4", new Vector3( 20f, 0, 93f),  new Vector3(11f,  8f,  9f));
    }

    // ── Castle ────────────────────────────────────────────────────────────
    static void CreateCastle()
    {
        var root = Empty("Castle");

        // Outer courtyard walls (connect to city north wall)
        WallPart("Court_L", root, new Vector3(-20f, 6f, L - 8f),
                 new Vector3(WALL_T, 12f, 20f), StoneDark);
        WallPart("Court_R", root, new Vector3( 20f, 6f, L - 8f),
                 new Vector3(WALL_T, 12f, 20f), StoneDark);
        WallPart("Court_Back", root, new Vector3(0, 6f, L - 18f),
                 new Vector3(40f, 12f, WALL_T), StoneDark);

        // Castle gate arch pillars (entrance from city path)
        WallPart("Castle_GateL", root, new Vector3(-5.5f, 5f, L - 2f),
                 new Vector3(3f, 10f, 3f), StoneDark);
        WallPart("Castle_GateR", root, new Vector3( 5.5f, 5f, L - 2f),
                 new Vector3(3f, 10f, 3f), StoneDark);
        WallPart("Castle_GateTop", root, new Vector3(0, 9.5f, L - 2f),
                 new Vector3(14f, 3f, 3f), StoneDark);

        // Main keep
        WallPart("Keep", root, new Vector3(0, 15f, L - 10f),
                 new Vector3(28f, 30f, 22f), StoneDark);

        // Keep battlements
        for (int i = -2; i <= 2; i++)
        {
            WallPart($"Merlon_F{i}", root,
                     new Vector3(i * 4f, 31f, L - 1f),
                     new Vector3(2f, 4f, 2f), StoneDark);
        }

        // Keep towers (tall, thin)
        CastleTower("KTower_L", root, new Vector3(-15f, 0, L - 10f));
        CastleTower("KTower_R", root, new Vector3( 15f, 0, L - 10f));

        // Inner chamber marker (trigger zone — destination)
        var chamber = Empty("InnerChamber", root.transform);
        chamber.transform.position = new Vector3(0, 1f, L - 10f);
        var col = chamber.AddComponent<BoxCollider>();
        col.size    = new Vector3(10f, 4f, 8f);
        col.isTrigger = true;
        chamber.AddComponent<InnerChamberTrigger>();
    }

    // ── Lighting ──────────────────────────────────────────────────────────
    static void CreateLighting()
    {
        // Sun — warm, low-angle (Peak aesthetic)
        var sunGo = new GameObject("Sun");
        var sun   = sunGo.AddComponent<Light>();
        sun.type      = LightType.Directional;
        sun.intensity = 1.3f;
        sun.color     = new Color(1f, 0.91f, 0.76f);
        sunGo.transform.rotation = Quaternion.Euler(36f, -28f, 0f);

        // Ambient — cool sky, warm ground
        RenderSettings.ambientMode       = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor   = new Color(0.48f, 0.62f, 0.88f);
        RenderSettings.ambientEquatorColor = new Color(0.58f, 0.56f, 0.48f);
        RenderSettings.ambientGroundColor  = new Color(0.22f, 0.20f, 0.16f);

        // Atmospheric fog
        RenderSettings.fog             = true;
        RenderSettings.fogColor        = new Color(0.68f, 0.73f, 0.83f);
        RenderSettings.fogMode         = FogMode.Linear;
        RenderSettings.fogStartDistance = 50f;
        RenderSettings.fogEndDistance   = 180f;
    }

    // ── Player spawn ──────────────────────────────────────────────────────
    static void CreatePlayer()
    {
        var player = new GameObject("Player");
        player.transform.position = new Vector3(0, 0, -6f); // just outside south gate

        var cc    = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.35f;
        cc.center = new Vector3(0, 0.9f, 0);

        // Camera rig
        var camHolder = new GameObject("CameraHolder");
        camHolder.transform.SetParent(player.transform);
        camHolder.transform.localPosition = new Vector3(0, 1.65f, 0);

        var camGo = new GameObject("MainCamera");
        camGo.transform.SetParent(camHolder.transform);
        camGo.transform.localPosition = Vector3.zero;
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.fieldOfView  = 75f;
        cam.nearClipPlane = 0.1f;
        camGo.AddComponent<AudioListener>();

        // Controller
        var fpc = player.AddComponent<FirstPersonController>();
        fpc.cameraHolder = camHolder.transform;

        // Stats — Warrior by default (swap in CharacterSelect later)
        var stats = player.AddComponent<CharacterStats>();
        stats.classData = CharacterClassData.Get(ClassType.Warrior);

        // Character model (lives behind the player pivot; invisible in FP view)
        var modelRoot = new GameObject("CharacterModel");
        modelRoot.transform.SetParent(player.transform);
        modelRoot.transform.localPosition = Vector3.zero;
        var assembler = modelRoot.AddComponent<PeakCharacterAssembler>();
        assembler.Assemble(); // build immediately so it's visible in scene view

        // Hide model from player's own camera via layer (set up layer "PlayerModel"
        // in Project Settings and assign cam.cullingMask to exclude it)
        // — left as a manual step for now; model is behind the camera anyway.
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    static GameObject Box(string name, Vector3 pos, Vector3 scale, Color color, Transform parent = null)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = name;
        if (parent != null) g.transform.SetParent(parent);
        g.transform.position   = pos;
        g.transform.localScale = scale;
        ApplyMaterial(g, color);
        return g;
    }

    static GameObject WallPart(string name, GameObject parent, Vector3 pos, Vector3 scale, Color color)
    {
        var g = Box(name, pos, scale, color, parent.transform);
        GameObjectUtility.SetStaticEditorFlags(g, StaticEditorFlags.BatchingStatic);
        return g;
    }

    static void Tower(string name, GameObject parent, Vector3 basePos)
    {
        float h = WALL_H * 1.4f;
        WallPart(name, parent, basePos + new Vector3(0, h / 2f, 0),
                 new Vector3(6f, h, 6f), StoneLight);
    }

    static void CastleTower(string name, GameObject parent, Vector3 basePos)
    {
        float h = 36f;
        WallPart(name, parent, basePos + new Vector3(0, h / 2f, 0),
                 new Vector3(8f, h, 8f), StoneDark);
        // Simple conical cap (flattened cube)
        WallPart(name + "_Cap", parent, basePos + new Vector3(0, h + 1f, 0),
                 new Vector3(9f, 2f, 9f), new Color(0.28f, 0.22f, 0.18f));
    }

    static void Building(GameObject parent, string name, Vector3 basePos, Vector3 size)
    {
        float shade = Random.Range(0.44f, 0.62f);
        var   color = new Color(shade, shade * 0.86f, shade * 0.70f);
        var   pos   = basePos + new Vector3(0, size.y / 2f, 0);
        WallPart(name, parent, pos, size, color);
        // Roof
        WallPart(name + "_Roof", parent, pos + new Vector3(0, size.y / 2f + 0.4f, 0),
                 new Vector3(size.x + 0.4f, 0.8f, size.z + 0.4f),
                 new Color(0.30f, 0.22f, 0.15f));
    }

    static GameObject Empty(string name, Transform parent = null)
    {
        var g = new GameObject(name);
        if (parent != null) g.transform.SetParent(parent);
        return g;
    }

    static void ApplyMaterial(GameObject g, Color color)
    {
        var r = g.GetComponent<Renderer>();
        if (r == null) return;
        var mat = new Material(Shader.Find("Standard") ?? Shader.Find("Diffuse"));
        mat.color = color;
        r.sharedMaterial = mat;
    }
}
