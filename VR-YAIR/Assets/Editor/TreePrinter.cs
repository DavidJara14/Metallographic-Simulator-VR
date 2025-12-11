#if UNITY_EDITOR

using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

[ExecuteAlways]
public static class TreePrinter
{
    [Header("Root object to print")]
    public static GameObject root;

    [Header("TMP Output (same GameObject)")]
    public static TMP_Text outputText;

    [Header("Auto generated TMP Sprite Asset")]
    public static TMP_SpriteAsset runtimeSpriteAsset;

    private static Dictionary<Texture2D, uint> textureToGlyphIndex = new Dictionary<Texture2D, uint>();

    [ContextMenu("Print Tree (Dynamic TMP Sprites)")]
    public static void PrintTree(GameObject GO)
    {
        if (GO == null)
        {
            Debug.LogError("GO is not assigned");
            return;
        }

        root = GO;

        if (outputText == null)
            return;
            //outputText = GetComponent<TMP_Text>();

        if (outputText == null)
        {
            Debug.LogError("TMP_Text not found on GameObject.");
            return;
        }

        EnsureSpriteAsset();

        string tree = "Tree:\n" + BuildTree(root.transform, "", true);

        outputText.spriteAsset = runtimeSpriteAsset;
        outputText.text = tree;
    }

    // ---------------- SPRITE ASSET ----------------

    private static void EnsureSpriteAsset()
    {
        if (runtimeSpriteAsset != null)
            return;

        runtimeSpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        runtimeSpriteAsset.name = "Auto_Generated_Component_Icons";

        runtimeSpriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"));

        textureToGlyphIndex.Clear();
    }

    private static uint RegisterIcon(Texture2D tex)
    {
        if (textureToGlyphIndex.TryGetValue(tex, out uint index))
            return index;

        uint newIndex = (uint)runtimeSpriteAsset.spriteGlyphTable.Count;

        var charDef = new TMP_SpriteCharacter
        {
            name = $"icon_{newIndex}",
            unicode = 0xE000 + newIndex,
            glyphIndex = newIndex,
            scale = 1
        };

        var xd = new TMP_SpriteGlyph(newIndex, new GlyphMetrics(
                tex.width,
                tex.height,
                0,
                0,
                tex.width
            ), new GlyphRect(0, 0, tex.width, tex.height),
            1, 0);

        runtimeSpriteAsset.spriteGlyphTable.Add(xd);
        runtimeSpriteAsset.spriteCharacterTable.Add(charDef);

        textureToGlyphIndex.Add(tex, newIndex);

        return newIndex;
    }

    // ---------------- TREE ----------------

    private static string BuildTree(Transform current, string prefix, bool isLast)
    {
        string branch = prefix + (isLast ? "└── " : "├── ");
        string icons = GetComponentIcons(current.gameObject);

        string result = branch + current.name + " " + icons + "\n";

        string newPrefix = prefix + (isLast ? "          " : "│      ");

        for (int i = 0; i < current.childCount; i++)
        {
            bool childIsLast = (i == current.childCount - 1);
            result += BuildTree(current.GetChild(i), newPrefix, childIsLast);
        }

        return result;
    }

    private static string GetComponentIcons(GameObject go)
    {
        Component[] comps = go.GetComponents<Component>();

        Dictionary<uint, int> iconCounts = new Dictionary<uint, int>();

        foreach (var c in comps)
        {
            if (c == null) continue;

            var gui = EditorGUIUtility.ObjectContent(c, c.GetType());
            var tex = gui.image as Texture2D;
            if (tex == null) continue;

            uint glyphIndex = RegisterIcon(tex);

            if (!iconCounts.ContainsKey(glyphIndex))
                iconCounts[glyphIndex] = 0;

            iconCounts[glyphIndex]++;
        }

        string result = "";

        foreach (var kv in iconCounts)
        {
            if (kv.Value == 1)
                result += $"<sprite=#{kv.Key}> ";
            else
                result += $"<sprite=#{kv.Key}>x{kv.Value} ";
        }

        return result.TrimEnd();
    }

    // ---------------- CLEANUP (optional) ----------------

    [ContextMenu("Clear Generated Sprite Asset")]
    private static void ClearSpriteAsset()
    {
        runtimeSpriteAsset = null;
        textureToGlyphIndex.Clear();
    }
}

#endif
