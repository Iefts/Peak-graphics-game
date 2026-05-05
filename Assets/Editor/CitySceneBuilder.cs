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
    const float W      = 80f;   // city width  (x)
    const float L      = 120f;  // city length (z)
    const float WALL_H = 10f;
    const float WALL_T = 2f;
    const float GATE_W = 8f;

    // Fallback colors (used when pack material fails to load)
    static readonly Color StoneLight = new Color(0.62f, 0.54f, 0.44f);
    static readonly Color StoneDark  = new Color(0.48f, 0.41f, 0.34f);
    static readonly Color DirtGround = new Color(0.36f, 0.28f, 0.20f);
    static readonly Color PathColor  = new Color(0.52f, 0.43f, 0.30f);
    static readonly Color RoofDark   = new Color(0.28f, 0.20f, 0.14f);

    // ── Entry point ───────────────────────────────────────────────────────
    [MenuItem("Peak Game/Build City Scene")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateGround();
        CreateCityWalls();
        CreatePath();
        CreateBuildings();
        PlacePeasants();
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
        Box("Ground", new Vector3(0, -0.05f, L / 2f),
            new Vector3(W + 20f, 0.1f, L + 30f),
            PackMat("LooseRocks", DirtGround));
    }

    // ── City walls ────────────────────────────────────────────────────────
    static void CreateCityWalls()
    {
        var root     = Empty("CityWalls");
        var stone    = PackMat("StoneWallTile", StoneLight);
        var stoneDk  = PackMat("StoneWallTile", StoneDark);

        float halfOpen   = GATE_W / 2f;
        float sideW      = (W - GATE_W) / 2f;
        float sideCenter = halfOpen + sideW / 2f;

        // South wall — two halves flanking the gate
        WallPart("SouthWall_L", root, new Vector3(-sideCenter, WALL_H / 2f, 0),
                 new Vector3(sideW, WALL_H, WALL_T), stone);
        WallPart("SouthWall_R", root, new Vector3( sideCenter, WALL_H / 2f, 0),
                 new Vector3(sideW, WALL_H, WALL_T), stone);
        // Gate lintel
        WallPart("GateLintel", root, new Vector3(0, WALL_H - 1.5f, 0),
                 new Vector3(GATE_W, 3f, WALL_T), stoneDk);
        // Gate pillars
        WallPart("GatePillar_L", root, new Vector3(-halfOpen - 0.5f, WALL_H * 0.45f, 0),
                 new Vector3(1.5f, WALL_H * 0.9f, WALL_T * 1.5f), stoneDk);
        WallPart("GatePillar_R", root, new Vector3( halfOpen + 0.5f, WALL_H * 0.45f, 0),
                 new Vector3(1.5f, WALL_H * 0.9f, WALL_T * 1.5f), stoneDk);

        // North / East / West walls
        WallPart("NorthWall", root, new Vector3(0, WALL_H / 2f, L),
                 new Vector3(W, WALL_H, WALL_T), stone);
        WallPart("EastWall",  root, new Vector3( W / 2f, WALL_H / 2f, L / 2f),
                 new Vector3(WALL_T, WALL_H, L), stone);
        WallPart("WestWall",  root, new Vector3(-W / 2f, WALL_H / 2f, L / 2f),
                 new Vector3(WALL_T, WALL_H, L), stone);

        // Corner towers
        Tower("Tower_SE", root, new Vector3( W / 2f, 0, 0),  stone);
        Tower("Tower_SW", root, new Vector3(-W / 2f, 0, 0),  stone);
        Tower("Tower_NE", root, new Vector3( W / 2f, 0, L),  stone);
        Tower("Tower_NW", root, new Vector3(-W / 2f, 0, L),  stone);
    }

    // ── Main path ─────────────────────────────────────────────────────────
    static void CreatePath()
    {
        Box("MainPath", new Vector3(0, 0.02f, L / 2f),
            new Vector3(GATE_W, 0.04f, L),
            PackMat("Cobblestones", PathColor));
    }

    // ── City buildings ────────────────────────────────────────────────────
    static void CreateBuildings()
    {
        var root = Empty("Buildings");

        // Left side
        Building(root, "Bldg_L1", new Vector3(-15f, 0, 22f), new Vector3(10f,  8f, 12f), 0);
        Building(root, "Bldg_L2", new Vector3(-20f, 0, 45f), new Vector3(13f,  6f, 10f), 1);
        Building(root, "Bldg_L3", new Vector3(-14f, 0, 68f), new Vector3( 9f, 11f, 11f), 2);
        Building(root, "Bldg_L4", new Vector3(-22f, 0, 88f), new Vector3(11f,  7f,  9f), 3);

        // Right side
        Building(root, "Bldg_R1", new Vector3( 15f, 0, 28f), new Vector3(10f,  9f, 12f), 4);
        Building(root, "Bldg_R2", new Vector3( 22f, 0, 52f), new Vector3(14f,  6f, 10f), 5);
        Building(root, "Bldg_R3", new Vector3( 16f, 0, 74f), new Vector3( 9f, 12f, 10f), 6);
        Building(root, "Bldg_R4", new Vector3( 20f, 0, 93f), new Vector3(11f,  8f,  9f), 7);
    }

    // ── Peasant NPCs ──────────────────────────────────────────────────────
    static void PlacePeasants()
    {
        const string BASE = "Assets/Polytope Studio/Lowpoly_Characters/Prefabs/" +
                            "Modular_NPC/Peasants_Citizens/Sets/";

        var male    = AssetDatabase.LoadAssetAtPath<GameObject>(BASE + "PT_Male_Peasant_01.prefab");
        var femaleA = AssetDatabase.LoadAssetAtPath<GameObject>(BASE + "PT_Female_Peasant_01_a.prefab");
        var femaleB = AssetDatabase.LoadAssetAtPath<GameObject>(BASE + "PT_Female_Peasant_01_b.prefab");
        var boy     = AssetDatabase.LoadAssetAtPath<GameObject>(BASE + "PT_Boy_Peasant_01.prefab");

        var root = Empty("Peasants");

        // 7 peasants — scattered along the street, off the main path
        SpawnPeasant(root, male,    new Vector3(-10f, 0, 20f),  45f);
        SpawnPeasant(root, femaleA, new Vector3( 11f, 0, 32f), -30f);
        SpawnPeasant(root, boy,     new Vector3(-12f, 0, 48f),  90f);
        SpawnPeasant(root, femaleB, new Vector3( 10f, 0, 58f), 180f);
        SpawnPeasant(root, male,    new Vector3(-11f, 0, 72f), -90f);
        SpawnPeasant(root, femaleA, new Vector3( 13f, 0, 85f),  20f);
        SpawnPeasant(root, boy,     new Vector3( -9f, 0, 95f), 135f);
    }

    // ── Castle ────────────────────────────────────────────────────────────
    static void CreateCastle()
    {
        var root    = Empty("Castle");
        var stone   = PackMat("StoneWallTile", StoneDark);
        var roofCap = PackMat("WoodRoofTile",  RoofDark);

        // Outer courtyard walls
        WallPart("Court_L",    root, new Vector3(-20f, 6f, L - 8f),  new Vector3(WALL_T, 12f, 20f), stone);
        WallPart("Court_R",    root, new Vector3( 20f, 6f, L - 8f),  new Vector3(WALL_T, 12f, 20f), stone);
        WallPart("Court_Back", root, new Vector3(0, 6f, L - 18f),    new Vector3(40f, 12f, WALL_T), stone);

        // Castle gate pillars + lintel
        WallPart("Castle_GateL",   root, new Vector3(-5.5f, 5f,   L - 2f), new Vector3(3f, 10f, 3f),  stone);
        WallPart("Castle_GateR",   root, new Vector3( 5.5f, 5f,   L - 2f), new Vector3(3f, 10f, 3f),  stone);
        WallPart("Castle_GateTop", root, new Vector3( 0,    9.5f, L - 2f), new Vector3(14f, 3f, 3f),  stone);

        // Main keep
        WallPart("Keep", root, new Vector3(0, 15f, L - 10f), new Vector3(28f, 30f, 22f), stone);

        // Keep battlements
        for (int i = -2; i <= 2; i++)
            WallPart($"Merlon_F{i}", root, new Vector3(i * 4f, 31f, L - 1f), new Vector3(2f, 4f, 2f), stone);

        // Keep towers
        CastleTower("KTower_L", root, new Vector3(-15f, 0, L - 10f), stone, roofCap);
        CastleTower("KTower_R", root, new Vector3( 15f, 0, L - 10f), stone, roofCap);

        // Inner chamber trigger (destination)
        var chamber = Empty("InnerChamber", root.transform);
        chamber.transform.position = new Vector3(0, 1f, L - 10f);
        var col = chamber.AddComponent<BoxCollider>();
        col.size      = new Vector3(10f, 4f, 8f);
        col.isTrigger = true;
        chamber.AddComponent<InnerChamberTrigger>();
    }

    // ── Lighting ──────────────────────────────────────────────────────────
    static void CreateLighting()
    {
        var sunGo = new GameObject("Sun");
        var sun   = sunGo.AddComponent<Light>();
        sun.type      = LightType.Directional;
        sun.intensity = 1.3f;
        sun.color     = new Color(1f, 0.91f, 0.76f);
        sunGo.transform.rotation = Quaternion.Euler(36f, -28f, 0f);

        RenderSettings.ambientMode        = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor    = new Color(0.48f, 0.62f, 0.88f);
        RenderSettings.ambientEquatorColor = new Color(0.58f, 0.56f, 0.48f);
        RenderSettings.ambientGroundColor  = new Color(0.22f, 0.20f, 0.16f);

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
        player.transform.position = new Vector3(0, 0, -6f);

        var cc    = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.35f;
        cc.center = new Vector3(0, 0.9f, 0);

        var camHolder = new GameObject("CameraHolder");
        camHolder.transform.SetParent(player.transform);
        camHolder.transform.localPosition = new Vector3(0, 1.65f, 0);

        var camGo = new GameObject("MainCamera");
        camGo.transform.SetParent(camHolder.transform);
        camGo.transform.localPosition = Vector3.zero;
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.fieldOfView   = 75f;
        cam.nearClipPlane = 0.1f;
        camGo.AddComponent<AudioListener>();

        var fpc = player.AddComponent<FirstPersonController>();
        fpc.cameraHolder = camHolder.transform;

        var stats = player.AddComponent<CharacterStats>();
        stats.classData = CharacterClassData.Get(ClassType.Warrior);

        var modelRoot = new GameObject("CharacterModel");
        modelRoot.transform.SetParent(player.transform);
        modelRoot.transform.localPosition = Vector3.zero;
        var assembler = modelRoot.AddComponent<PeakCharacterAssembler>();
        assembler.Assemble();
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    static GameObject Box(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent = null)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = name;
        if (parent != null) g.transform.SetParent(parent);
        g.transform.position   = pos;
        g.transform.localScale = scale;
        if (mat != null) g.GetComponent<Renderer>().sharedMaterial = mat;
        return g;
    }

    static GameObject WallPart(string name, GameObject parent, Vector3 pos, Vector3 scale, Material mat)
    {
        return Box(name, pos, scale, mat, parent.transform);
    }

    static void Tower(string name, GameObject parent, Vector3 basePos, Material mat)
    {
        float h = WALL_H * 1.4f;
        WallPart(name, parent, basePos + new Vector3(0, h / 2f, 0), new Vector3(6f, h, 6f), mat);
    }

    static void CastleTower(string name, GameObject parent, Vector3 basePos, Material body, Material cap)
    {
        float h = 36f;
        WallPart(name,        parent, basePos + new Vector3(0, h / 2f, 0), new Vector3(8f, h, 8f),  body);
        WallPart(name + "_Cap", parent, basePos + new Vector3(0, h + 1f, 0), new Vector3(9f, 2f, 9f), cap);
    }

    // index drives material variation so adjacent buildings look different
    static void Building(GameObject parent, string name, Vector3 basePos, Vector3 size, int index)
    {
        var wallMat = (index % 2 == 0)
            ? PackMat("Plaster",     new Color(0.82f, 0.76f, 0.64f))
            : PackMat("BrickWallTile", new Color(0.60f, 0.38f, 0.26f));

        var roofMat = (index % 3 == 0)
            ? PackMat("StrawRoof",   new Color(0.72f, 0.60f, 0.28f))
            : PackMat("WoodRoofTile", RoofDark);

        var pos = basePos + new Vector3(0, size.y / 2f, 0);
        WallPart(name,          parent, pos,                                         size,                                wallMat);
        WallPart(name + "_Roof", parent, pos + new Vector3(0, size.y / 2f + 0.4f, 0),
                 new Vector3(size.x + 0.4f, 0.8f, size.z + 0.4f),                  roofMat);
    }

    static void SpawnPeasant(GameObject parent, GameObject prefab, Vector3 pos, float yRot)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[CitySceneBuilder] Peasant prefab not found — skipping.");
            return;
        }
        var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (go == null) return;
        go.transform.SetParent(parent.transform);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    static GameObject Empty(string name, Transform parent = null)
    {
        var g = new GameObject(name);
        if (parent != null) g.transform.SetParent(parent);
        return g;
    }

    // Creates a Standard shader material using the pack's basecolor texture.
    // Bypasses the pack's custom shaders (which are pipeline-incompatible).
    // Falls back to a plain-color material if the texture isn't found.
    static Material PackMat(string name, Color fallback)
    {
        string texFile = name switch
        {
            "StoneWallTile"  => "Stone_Wall_basecolor.png",
            "Cobblestones"   => "Cobblestones_basecolor.png",
            "LooseRocks"     => "LooseRocks_basecolor.png",
            "Plaster"        => "Plaster_Surface_basecolor.png",
            "BrickWallTile"  => "Brick_Wall_basecolor.png",
            "StrawRoof"      => "Straw_Roof_basecolor.png",
            "WoodRoofTile"   => "Roof_Wood_Tile_basecolor.png",
            "WoodTile"       => "Wood_Tile_basecolor.png",
            _                => null
        };

        var mat = new Material(Shader.Find("Standard") ?? Shader.Find("Diffuse"));

        if (texFile != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(
                $"Assets/Medieval Village Building Pack/Textures/{texFile}");
            if (tex != null)
            {
                mat.mainTexture = tex;
                mat.color = Color.white;
                return mat;
            }
        }

        Debug.LogWarning($"[CitySceneBuilder] Texture for '{name}' not found — using fallback color.");
        mat.color = fallback;
        return mat;
    }
}
