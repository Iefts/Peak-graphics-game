using UnityEditor;
using UnityEngine;

/// Peak Game ▶ Fix Pink Materials
/// Finds every material using a non-URP shader and upgrades it to URP Lit,
/// correctly mapping Built-in property names to their URP equivalents:
///   _MainTex → _BaseMap
///   _Color   → _BaseColor
public static class ShaderFixer
{
    [MenuItem("Peak Game/Fix Pink Materials")]
    public static void FixPinkMaterials()
    {
        var urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null)
        {
            Debug.LogError("[ShaderFixer] URP Lit shader not found — is URP installed and the pipeline asset assigned?");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        int fixedCount = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;
            if (mat.isVariant) continue;

            bool needsUpgrade = mat.shader == null
                             || mat.shader.name == "Hidden/InternalErrorShader"
                             || (!mat.shader.name.StartsWith("Universal Render Pipeline")
                              && !mat.shader.name.StartsWith("Shader Graphs/"));

            if (!needsUpgrade) continue;

            // Read properties BEFORE switching shader (property names differ per shader)
            Texture tex = mat.HasProperty("_MainTex")   ? mat.GetTexture("_MainTex")
                        : mat.HasProperty("_BaseMap")    ? mat.GetTexture("_BaseMap")
                        : mat.mainTexture;

            Color col = mat.HasProperty("_Color")      ? mat.GetColor("_Color")
                      : mat.HasProperty("_BaseColor")   ? mat.GetColor("_BaseColor")
                      : Color.white;

            // Switch to URP Lit
            mat.shader = urpLit;

            // Write using URP Lit's property names
            if (tex != null) mat.SetTexture("_BaseMap", tex);
            mat.SetColor("_BaseColor", col);

            EditorUtility.SetDirty(mat);
            fixedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[ShaderFixer] Fixed {fixedCount} materials.");
    }
}
