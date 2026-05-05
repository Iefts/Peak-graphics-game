using UnityEditor;
using UnityEngine;

/// Peak Game ▶ Fix Polytope Materials
/// The ShaderFixer previously wrote _BaseColor {0,0,0,0} into Polytope Studio
/// materials. The URP package restored the correct shaders but that black/zero-alpha
/// colour remained. This resets _BaseColor to opaque white on every material in
/// the Polytope Studio folder so the original shader colours show through.
public static class PolytopeMatFixer
{
    [MenuItem("Peak Game/Fix Polytope Materials")]
    public static void Fix()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material",
            new[] { "Assets/Polytope Studio" });

        int fixed = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;
            if (mat.isVariant) continue;

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", Color.white);
                EditorUtility.SetDirty(mat);
                fixed++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[PolytopeMatFixer] Reset _BaseColor to white on {fixed} materials.");
    }
}
