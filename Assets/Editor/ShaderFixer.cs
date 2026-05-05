using UnityEditor;
using UnityEngine;

/// Peak Game ▶ Fix Pink Materials
/// Finds every material in the project whose shader is missing or unsupported
/// (shows as pink in the scene) and replaces it with URP Lit.
/// Preserves the material's existing main texture and color.
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

            // Pink in URP = shader exists but isn't a URP shader.
            // Replace anything that isn't already a URP or Shader Graph shader.
            bool needsUpgrade = mat.shader == null
                             || mat.shader.name == "Hidden/InternalErrorShader"
                             || (!mat.shader.name.StartsWith("Universal Render Pipeline")
                              && !mat.shader.name.StartsWith("Shader Graphs/"));

            if (needsUpgrade)
            {
                Texture mainTex = mat.mainTexture;
                Color   col     = mat.color;

                mat.shader      = urpLit;
                mat.mainTexture = mainTex;
                mat.color       = col;

                EditorUtility.SetDirty(mat);
                fixedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[ShaderFixer] Fixed {fixedCount} pink materials.");
    }
}
