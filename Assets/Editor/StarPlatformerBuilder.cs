using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class StarPlatformerBuilder
{
    // Palette
    static readonly Color Sky      = new Color(0.07f, 0.07f, 0.14f);
    static readonly Color Ground   = new Color(0.22f, 0.52f, 0.22f);
    static readonly Color Notch    = new Color(0.65f, 0.38f, 0.18f);
    static readonly Color Platform = new Color(0.22f, 0.52f, 0.22f);
    static readonly Color GoalGold = new Color(1f, 0.85f, 0.10f);
    static readonly Color UiPanel  = new Color(0.10f, 0.10f, 0.18f, 0.85f);
    static readonly Color UiBtn    = new Color(0.18f, 0.20f, 0.32f, 1f);
    static readonly Color UiBtnHi  = new Color(0.30f, 0.34f, 0.55f, 1f);

    const string ScenesDir   = "Assets/Scenes";
    const string MenuScene   = "Assets/Scenes/MainMenu.unity";
    const string Level1Scene = "Assets/Scenes/Level01_StarPlatformer.unity";
    const string Level2Scene = "Assets/Scenes/Level02_StarPlatformer.unity";
    const string Level3Scene = "Assets/Scenes/Level03_StarPlatformer.unity";

    // ── Menu items ───────────────────────────────────────────────────────────

    [MenuItem("Star Game/Build All Scenes")]
    public static void BuildAll()
    {
        BuildMenuInternal();
        BuildLevel1Internal();
        BuildLevel2Internal();
        BuildLevel3Internal();
        RegisterScenes();
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(MenuScene);
        Debug.Log("[StarPlatformerBuilder] All scenes built and registered.");
    }

    [MenuItem("Star Game/Build Main Menu")]
    public static void BuildMenu() { BuildMenuInternal(); RegisterScenes(); }

    [MenuItem("Star Game/Build Level 01")]
    public static void BuildLevel1() { BuildLevel1Internal(); RegisterScenes(); }

    [MenuItem("Star Game/Build Level 02")]
    public static void BuildLevel2() { BuildLevel2Internal(); RegisterScenes(); }

    [MenuItem("Star Game/Build Level 03")]
    public static void BuildLevel3() { BuildLevel3Internal(); RegisterScenes(); }

    // ── Scene builders ───────────────────────────────────────────────────────

    static void BuildMenuInternal()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGo = new GameObject("Main Camera");
        camGo.tag  = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic    = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = Sky;
        cam.clearFlags      = CameraClearFlags.SolidColor;
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        camGo.AddComponent<AudioListener>();

        var menuGo = new GameObject("MainMenu");
        var ctrl   = menuGo.AddComponent<MainMenuController>();

        var canvasGo = new GameObject("Canvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        new GameObject("EventSystem").AddComponent<EventSystem>()
            .gameObject.AddComponent<StandaloneInputModule>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Title
        var title = UiText(canvasGo.transform, "Title", "STAR PLATFORMER", 120, GoalGold, font);
        SetAnchored(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                    new Vector2(0f, -180f), new Vector2(1400f, 200f));

        // Subtitle
        var sub = UiText(canvasGo.transform, "Subtitle", "Select a level", 48, Color.white, font);
        SetAnchored(sub.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                    new Vector2(0f, -320f), new Vector2(800f, 80f));

        // Level buttons
        AddLevelButton(canvasGo.transform, ctrl, font, "LEVEL 1", "Level01_StarPlatformer", new Vector2(-360f, -60f));
        AddLevelButton(canvasGo.transform, ctrl, font, "LEVEL 2", "Level02_StarPlatformer", new Vector2(   0f, -60f));
        AddLevelButton(canvasGo.transform, ctrl, font, "LEVEL 3", "Level03_StarPlatformer", new Vector2( 360f, -60f));

        // Quit button
        var quit = AddBasicButton(canvasGo.transform, "QuitButton", "QUIT", font);
        SetAnchored(quit.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                    new Vector2(0f, 120f), new Vector2(260f, 80f));
        quit.onClick.AddListener(() => ctrl.Quit());

        // Footer hint
        var hint = UiText(canvasGo.transform, "Hint", "A/D = roll  •  Space/W = jump", 30, new Color(1f,1f,1f,0.6f), font);
        SetAnchored(hint.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                    new Vector2(0f, 40f), new Vector2(900f, 50f));

        SaveScene(MenuScene);
    }

    static void AddLevelButton(Transform parent, MainMenuController ctrl, Font font, string label,
                               string sceneName, Vector2 anchoredPos)
    {
        var btn = AddBasicButton(parent, label + "Button", label, font);
        SetAnchored(btn.GetComponent<RectTransform>(),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    anchoredPos, new Vector2(300f, 200f));
        btn.onClick.AddListener(() => ctrl.LoadLevel(sceneName));
    }

    static void BuildLevel1Internal()
    {
        NewLevelScene();

        var root = new GameObject("Level");
        Slab("Ground", root.transform, new Vector2(0f, -0.3f), 12f, 0.6f, Ground);

        float[] nx = { 7.5f, 10.0f, 12.5f, 15.0f, 17.5f };
        float[] ny = { 2.2f,  3.5f,  4.8f,  6.1f,  7.4f };
        for (int i = 0; i < 5; i++)
            Pin($"Notch{i + 1}", root.transform, new Vector2(nx[i], ny[i]), 0.28f, Notch);

        Slab("EndPlatform", root.transform, new Vector2(23f, 8.1f), 9f, 0.6f, Platform);

        Goal(root.transform, new Vector2(24f, 9.0f));
        BuildLevelClearUI();

        SaveScene(Level1Scene);
    }

    static void BuildLevel2Internal()
    {
        NewLevelScene();

        var root = new GameObject("Level");
        Slab("Ground", root.transform, new Vector2(-1f, -0.3f), 12f, 0.6f, Ground);

        // Stair-step slabs ascending (small landings, harder than ground)
        Slab("Step1", root.transform, new Vector2( 7.5f, 1.3f), 1.2f, 0.35f, Platform);
        Slab("Step2", root.transform, new Vector2(10.0f, 2.6f), 1.2f, 0.35f, Platform);
        Slab("Step3", root.transform, new Vector2(12.5f, 3.9f), 1.2f, 0.35f, Platform);

        // Notch finish — three notches before end platform
        Pin("Notch1", root.transform, new Vector2(15.0f, 5.2f), 0.28f, Notch);
        Pin("Notch2", root.transform, new Vector2(17.5f, 6.5f), 0.28f, Notch);
        Pin("Notch3", root.transform, new Vector2(20.0f, 7.8f), 0.28f, Notch);

        Slab("EndPlatform", root.transform, new Vector2(25.5f, 8.6f), 9f, 0.6f, Platform);

        Goal(root.transform, new Vector2(26.5f, 9.5f));
        BuildLevelClearUI();

        SaveScene(Level2Scene);
    }

    static void BuildLevel3Internal()
    {
        NewLevelScene();

        var root = new GameObject("Level");
        Slab("Ground", root.transform, new Vector2(-1.5f, -0.3f), 11f, 0.6f, Ground);

        // Notch climb, then rest slab, then notch climb again
        Pin("Notch1", root.transform, new Vector2( 6.0f, 1.3f), 0.28f, Notch);
        Pin("Notch2", root.transform, new Vector2( 8.5f, 2.6f), 0.28f, Notch);
        Pin("Notch3", root.transform, new Vector2(11.0f, 3.9f), 0.28f, Notch);

        Slab("RestLedge", root.transform, new Vector2(13.5f, 5.2f), 1.6f, 0.35f, Platform);

        Pin("Notch4", root.transform, new Vector2(16.0f, 6.5f), 0.28f, Notch);
        Pin("Notch5", root.transform, new Vector2(18.5f, 7.8f), 0.28f, Notch);

        Slab("EndPlatform", root.transform, new Vector2(24f, 8.6f), 9f, 0.6f, Platform);

        Goal(root.transform, new Vector2(25f, 9.5f));
        BuildLevelClearUI();

        SaveScene(Level3Scene);
    }

    // ── Shared level pieces ──────────────────────────────────────────────────

    static void NewLevelScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildCamera();
        BuildPlayer();
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

    static void Goal(Transform parent, Vector2 center)
    {
        var go = new GameObject("Goal");
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(center.x, center.y, 0f);

        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        var star = go.AddComponent<StarMesh>();
        star.outerRadius = 0.55f;
        star.innerRadius = 0.07f;
        star.starColor   = GoalGold;
        // StarMesh requires PolygonCollider2D — make it a trigger
        var poly = go.GetComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        star.Rebuild();

        // Trigger collider used by LevelGoal (separate so star shape stays as visual)
        var circle = go.AddComponent<CircleCollider2D>();
        circle.isTrigger = true;
        circle.radius    = 0.55f;

        go.AddComponent<LevelGoal>().nextScene = "MainMenu";
    }

    static void BuildLevelClearUI()
    {
        var canvasGo = new GameObject("LevelClearCanvas");
        var canvas   = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var text = UiText(canvasGo.transform, "ClearLabel", "LEVEL CLEAR", 160, GoalGold, font);
        SetAnchored(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(1600f, 300f));

        var ui = canvasGo.AddComponent<LevelClearUI>();
        ui.label = text;
        text.gameObject.SetActive(false);

        // EventSystem so any future UI input works (not strictly needed for label)
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem").AddComponent<EventSystem>()
                .gameObject.AddComponent<StandaloneInputModule>();
        }
    }

    // ── UI helpers ───────────────────────────────────────────────────────────

    static Text UiText(Transform parent, string name, string content, int size, Color color, Font font)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text      = content;
        t.font      = font;
        t.fontSize  = size;
        t.color     = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow   = VerticalWrapMode.Overflow;
        return t;
    }

    static Button AddBasicButton(Transform parent, string name, string label, Font font)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = UiBtn;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.normalColor      = UiBtn;
        colors.highlightedColor = UiBtnHi;
        colors.pressedColor     = UiPanel;
        colors.selectedColor    = UiBtnHi;
        btn.colors = colors;
        btn.targetGraphic = img;

        var labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(go.transform, false);
        var lt = labelGo.AddComponent<Text>();
        lt.text      = label;
        lt.font      = font;
        lt.fontSize  = 56;
        lt.color     = Color.white;
        lt.alignment = TextAnchor.MiddleCenter;
        var lrt = labelGo.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero;
        lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;

        return btn;
    }

    static void SetAnchored(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 pos, Vector2 size)
    {
        rt.anchorMin        = aMin;
        rt.anchorMax        = aMax;
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
    }

    // ── Geometry helpers ─────────────────────────────────────────────────────

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
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position   = new Vector3(center.x, center.y, 0f);
        go.transform.localScale = new Vector3(radius * 2f, radius * 2f, 0.2f);
        Object.DestroyImmediate(go.GetComponent<SphereCollider>());
        var col    = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;
        go.GetComponent<Renderer>().sharedMaterial = UnlitMat(color);
    }

    static Material UnlitMat(Color color)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", color);
        return mat;
    }

    // ── Scene save / build settings ──────────────────────────────────────────

    static void SaveScene(string path)
    {
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Scenes");
        AssetDatabase.Refresh();
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path);
        Debug.Log($"[StarPlatformerBuilder] Saved {path}");
    }

    static void RegisterScenes()
    {
        var paths = new[] { MenuScene, Level1Scene, Level2Scene, Level3Scene };
        var list  = new List<EditorBuildSettingsScene>();
        foreach (var p in paths)
            if (System.IO.File.Exists(p))
                list.Add(new EditorBuildSettingsScene(p, true));
        EditorBuildSettings.scenes = list.ToArray();
    }
}
