using UnityEngine;

/// Assembles a rough Peak-style character from Unity primitives.
/// In FPS mode this model is hidden from the player's camera — it exists for
/// third-person previews, shadows, and future co-op visibility.
public class PeakCharacterAssembler : MonoBehaviour
{
    [Header("Jacket / Gear")]
    public Color jacketColor   = new Color(0.18f, 0.42f, 0.78f);
    public Color pantsColor    = new Color(0.14f, 0.14f, 0.17f);
    public Color skinColor     = new Color(0.95f, 0.76f, 0.61f);
    public Color helmetColor   = new Color(0.9f,  0.48f, 0.08f);
    public Color backpackColor = new Color(0.55f, 0.32f, 0.08f);
    public Color bootColor     = new Color(0.12f, 0.1f,  0.08f);

    void Start()
    {
        if (transform.childCount == 0) Assemble();
    }

    public void Assemble()
    {
        // Torso — puffy capsule, slightly wide
        Part("Torso",       PrimitiveType.Capsule, jacketColor,   new Vector3(0,     1.10f,  0),    new Vector3(0.56f, 0.50f, 0.52f));
        // Hips
        Part("Hips",        PrimitiveType.Capsule, pantsColor,    new Vector3(0,     0.72f,  0),    new Vector3(0.50f, 0.28f, 0.46f));
        // Head
        Part("Head",        PrimitiveType.Sphere,  skinColor,     new Vector3(0,     1.72f,  0),    new Vector3(0.34f, 0.34f, 0.34f));
        // Helmet (slightly flattened sphere on top)
        Part("Helmet",      PrimitiveType.Sphere,  helmetColor,   new Vector3(0,     1.83f,  0),    new Vector3(0.40f, 0.26f, 0.40f));

        // Arms — held slightly away from body (puffy jacket silhouette)
        Part("ArmUp_L",     PrimitiveType.Capsule, jacketColor,   new Vector3(-0.42f, 1.24f, 0),   new Vector3(0.20f, 0.27f, 0.20f), new Vector3(0,0, 90));
        Part("ArmUp_R",     PrimitiveType.Capsule, jacketColor,   new Vector3( 0.42f, 1.24f, 0),   new Vector3(0.20f, 0.27f, 0.20f), new Vector3(0,0,-90));
        Part("ArmLo_L",     PrimitiveType.Capsule, jacketColor,   new Vector3(-0.55f, 0.98f, 0),   new Vector3(0.17f, 0.22f, 0.17f), new Vector3(0,0, 90));
        Part("ArmLo_R",     PrimitiveType.Capsule, jacketColor,   new Vector3( 0.55f, 0.98f, 0),   new Vector3(0.17f, 0.22f, 0.17f), new Vector3(0,0,-90));

        // Legs
        Part("Leg_L",       PrimitiveType.Capsule, pantsColor,    new Vector3(-0.16f, 0.38f, 0),   new Vector3(0.22f, 0.38f, 0.22f));
        Part("Leg_R",       PrimitiveType.Capsule, pantsColor,    new Vector3( 0.16f, 0.38f, 0),   new Vector3(0.22f, 0.38f, 0.22f));

        // Boots — slightly forward offset for toe
        Part("Boot_L",      PrimitiveType.Cube,    bootColor,     new Vector3(-0.16f, 0.06f, 0.04f), new Vector3(0.22f, 0.12f, 0.30f));
        Part("Boot_R",      PrimitiveType.Cube,    bootColor,     new Vector3( 0.16f, 0.06f, 0.04f), new Vector3(0.22f, 0.12f, 0.30f));

        // Backpack
        Part("Backpack",    PrimitiveType.Cube,    backpackColor, new Vector3(0, 1.10f, -0.33f),  new Vector3(0.30f, 0.44f, 0.17f));
        // Backpack roll on top
        Part("PackRoll",    PrimitiveType.Capsule, backpackColor * 0.85f, new Vector3(0, 1.48f, -0.30f), new Vector3(0.22f, 0.18f, 0.22f), new Vector3(90,0,0));
    }

    void Part(string partName, PrimitiveType type, Color color,
              Vector3 localPos, Vector3 localScale, Vector3 eulerRot = default)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = partName;
        go.transform.SetParent(transform);
        go.transform.localPosition = localPos;
        go.transform.localScale    = localScale;
        go.transform.localRotation = Quaternion.Euler(eulerRot);

        // Make colliders non-blocking so they don't fight the CharacterController
        var col = go.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        var mat = new Material(Shader.Find("Standard") ?? Shader.Find("Diffuse"));
        mat.color = color;
        go.GetComponent<Renderer>().sharedMaterial = mat;
    }
}
