using UnityEngine;

/// Peak-style character built from Unity primitives.
/// Key traits: oversized round head, one dominant jacket color,
/// single-piece arms with a natural downward hang and slight splay.
public class PeakCharacterAssembler : MonoBehaviour
{
    [Header("Colors")]
    public Color jacketColor = new Color(0.18f, 0.42f, 0.78f); // main character color
    public Color pantsColor  = new Color(0.12f, 0.12f, 0.14f); // dark, mostly hidden
    public Color skinColor   = new Color(0.95f, 0.76f, 0.61f); // face only
    public Color bootColor   = new Color(0.10f, 0.09f, 0.08f); // near-black boots

    void Start()
    {
        if (transform.childCount == 0) Assemble();
    }

    public void Assemble()
    {
        // ── Body / Jacket ─────────────────────────────────────────────────
        // Long rounded capsule — jacket covers chest down to upper thigh
        Part("Jacket", PrimitiveType.Capsule, jacketColor,
             pos: new Vector3(0, 0.96f, 0),
             scale: new Vector3(0.54f, 0.60f, 0.50f));

        // ── Head ──────────────────────────────────────────────────────────
        // Deliberately oversized — Peak characters have a big cartoony head
        Part("Head", PrimitiveType.Sphere, skinColor,
             pos: new Vector3(0, 1.62f, 0),
             scale: new Vector3(0.48f, 0.48f, 0.48f));

        // Helmet — same color as jacket, slightly wider and flattened
        Part("Helmet", PrimitiveType.Sphere, jacketColor,
             pos: new Vector3(0, 1.74f, 0),
             scale: new Vector3(0.54f, 0.32f, 0.54f));

        // ── Arms ──────────────────────────────────────────────────────────
        // Single-piece capsules, hanging down with a slight outward splay (~14°)
        // Rotation around Z: positive = top tilts left (left arm splays outward at bottom)
        Part("Arm_L", PrimitiveType.Capsule, jacketColor,
             pos: new Vector3(-0.38f, 0.96f, 0),
             scale: new Vector3(0.14f, 0.38f, 0.14f),
             rot: new Vector3(0, 0, 14f));

        Part("Arm_R", PrimitiveType.Capsule, jacketColor,
             pos: new Vector3( 0.38f, 0.96f, 0),
             scale: new Vector3(0.14f, 0.38f, 0.14f),
             rot: new Vector3(0, 0, -14f));

        // Hands — small rounded sphere at the end of each arm
        Part("Hand_L", PrimitiveType.Sphere, skinColor,
             pos: new Vector3(-0.44f, 0.64f, 0),
             scale: new Vector3(0.12f, 0.12f, 0.12f));

        Part("Hand_R", PrimitiveType.Sphere, skinColor,
             pos: new Vector3( 0.44f, 0.64f, 0),
             scale: new Vector3(0.12f, 0.12f, 0.12f));

        // ── Legs ──────────────────────────────────────────────────────────
        // Short — the jacket covers most of the leg, only bottom third visible
        Part("Leg_L", PrimitiveType.Capsule, pantsColor,
             pos: new Vector3(-0.13f, 0.38f, 0),
             scale: new Vector3(0.17f, 0.30f, 0.17f));

        Part("Leg_R", PrimitiveType.Capsule, pantsColor,
             pos: new Vector3( 0.13f, 0.38f, 0),
             scale: new Vector3(0.17f, 0.30f, 0.17f));

        // ── Boots ─────────────────────────────────────────────────────────
        Part("Boot_L", PrimitiveType.Cube, bootColor,
             pos: new Vector3(-0.13f, 0.07f, 0.04f),
             scale: new Vector3(0.19f, 0.13f, 0.27f));

        Part("Boot_R", PrimitiveType.Cube, bootColor,
             pos: new Vector3( 0.13f, 0.07f, 0.04f),
             scale: new Vector3(0.19f, 0.13f, 0.27f));

        // ── Backpack ──────────────────────────────────────────────────────
        // Simple dark rectangle on the back, slightly darker than jacket
        Color packColor = jacketColor * 0.70f;
        packColor.a = 1f;
        Part("Backpack", PrimitiveType.Cube, packColor,
             pos: new Vector3(0, 1.00f, -0.31f),
             scale: new Vector3(0.28f, 0.38f, 0.14f));
    }

    void Part(string partName, PrimitiveType type, Color color,
              Vector3 pos, Vector3 scale, Vector3 rot = default)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = partName;
        go.transform.SetParent(transform);
        go.transform.localPosition = pos;
        go.transform.localScale    = scale;
        go.transform.localRotation = Quaternion.Euler(rot);

        var col = go.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
        mat.color = color;
        go.GetComponent<Renderer>().sharedMaterial = mat;
    }
}
