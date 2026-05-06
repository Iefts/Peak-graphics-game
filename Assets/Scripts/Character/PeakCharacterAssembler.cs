using UnityEngine;

/// Assembles a Peak-style character from Unity primitives.
/// Big round head, one dominant jacket colour, single-piece arms with slight splay.
public class PeakCharacterAssembler : MonoBehaviour
{
    [Header("Colours")]
    public Color jacketColor = new Color(0.18f, 0.42f, 0.78f);
    public Color pantsColor  = new Color(0.12f, 0.12f, 0.14f);
    public Color skinColor   = new Color(0.95f, 0.76f, 0.61f);
    public Color bootColor   = new Color(0.10f, 0.09f, 0.08f);

    void Start() { if (transform.childCount == 0) Assemble(); }

    public void Assemble()
    {
        Part("Jacket",   PrimitiveType.Capsule, jacketColor, new Vector3(0,      0.96f,  0),    new Vector3(0.54f, 0.60f, 0.50f));
        Part("Head",     PrimitiveType.Sphere,  skinColor,   new Vector3(0,      1.62f,  0),    new Vector3(0.48f, 0.48f, 0.48f));
        Part("Helmet",   PrimitiveType.Sphere,  jacketColor, new Vector3(0,      1.74f,  0),    new Vector3(0.54f, 0.32f, 0.54f));
        Part("Arm_L",    PrimitiveType.Capsule, jacketColor, new Vector3(-0.38f, 0.96f,  0),    new Vector3(0.14f, 0.38f, 0.14f), new Vector3(0, 0,  14f));
        Part("Arm_R",    PrimitiveType.Capsule, jacketColor, new Vector3( 0.38f, 0.96f,  0),    new Vector3(0.14f, 0.38f, 0.14f), new Vector3(0, 0, -14f));
        Part("Hand_L",   PrimitiveType.Sphere,  skinColor,   new Vector3(-0.44f, 0.64f,  0),    new Vector3(0.12f, 0.12f, 0.12f));
        Part("Hand_R",   PrimitiveType.Sphere,  skinColor,   new Vector3( 0.44f, 0.64f,  0),    new Vector3(0.12f, 0.12f, 0.12f));
        Part("Leg_L",    PrimitiveType.Capsule, pantsColor,  new Vector3(-0.13f, 0.38f,  0),    new Vector3(0.17f, 0.30f, 0.17f));
        Part("Leg_R",    PrimitiveType.Capsule, pantsColor,  new Vector3( 0.13f, 0.38f,  0),    new Vector3(0.17f, 0.30f, 0.17f));
        Part("Boot_L",   PrimitiveType.Cube,    bootColor,   new Vector3(-0.13f, 0.07f,  0.04f), new Vector3(0.19f, 0.13f, 0.27f));
        Part("Boot_R",   PrimitiveType.Cube,    bootColor,   new Vector3( 0.13f, 0.07f,  0.04f), new Vector3(0.19f, 0.13f, 0.27f));

        Color packColor = jacketColor * 0.70f; packColor.a = 1f;
        Part("Backpack", PrimitiveType.Cube,    packColor,   new Vector3(0,      1.00f, -0.31f), new Vector3(0.28f, 0.38f, 0.14f));
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

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", color);
        go.GetComponent<Renderer>().sharedMaterial = mat;
    }
}
