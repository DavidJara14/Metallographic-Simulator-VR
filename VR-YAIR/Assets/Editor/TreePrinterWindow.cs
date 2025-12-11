#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class TreePrinterWindow : EditorWindow
{
    private TMP_Text targetTMP;
    private GameObject rootObject;
    private Vector2 scroll;
    [Header("Auto generated TMP Sprite Asset")]
    private TMP_SpriteAsset runtimeSpriteAsset;
    private Dictionary<Texture2D, uint> textureToGlyphIndex = new Dictionary<Texture2D, uint>();

    [MenuItem("Window/Tree Printer")]
    public static void ShowWindow()
    {
        GetWindow<TreePrinterWindow>("Tree Printer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tree Printer Tool", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        // Campo para TMP_Text
        targetTMP = (TMP_Text)EditorGUILayout.ObjectField(
            "Target TMP Text",
            targetTMP,
            typeof(TMP_Text),
            true
        );

        // Campo para Root GameObject
        rootObject = (GameObject)EditorGUILayout.ObjectField(
            "Root GameObject",
            rootObject,
            typeof(GameObject),
            true
        );

        EditorGUILayout.Space();

        GUI.enabled = (targetTMP != null && rootObject != null);
        if (GUILayout.Button("Print Tree Into TMP"))
        {
            PrintTree();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview (Console-safe):");

        /*scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(150));
        if (rootObject != null)
        {
            EditorGUILayout.TextArea(
                BuildTree(rootObject.transform, "", true),
                GUILayout.ExpandHeight(true)
            );
        }
        EditorGUILayout.EndScrollView();*/
    }

    private void PrintTree()
    {
        EnsureSpriteAsset();

        string tree = "Tree:\n" + BuildTree(rootObject.transform, "", true);

        Undo.RecordObject(targetTMP, "Print Tree");
        targetTMP.text = tree;
        EditorUtility.SetDirty(targetTMP);
    }

    private string BuildTree(Transform current, string prefix, bool isLast)
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

    private string GetComponentIcons(GameObject go)
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

    private void EnsureSpriteAsset()
    {
        if (runtimeSpriteAsset != null)
            return;

        runtimeSpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        runtimeSpriteAsset.name = "Runtime_Tree_Icons";

        // Crear atlas
        Texture2D atlas = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
        runtimeSpriteAsset.spriteSheet = atlas;

        // Crear material
        runtimeSpriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"));
        runtimeSpriteAsset.material.mainTexture = atlas;

        // IMPORTANTE: forzar inicialización interna
        runtimeSpriteAsset.fallbackSpriteAssets = new List<TMP_SpriteAsset>();
        runtimeSpriteAsset.spriteCharacterTable.Clear();
        runtimeSpriteAsset.spriteGlyphTable.Clear();

        runtimeSpriteAsset.UpdateLookupTables();

        if (targetTMP != null)
            targetTMP.spriteAsset = runtimeSpriteAsset;

        EditorUtility.SetDirty(runtimeSpriteAsset);
    }




    private uint RegisterIcon(Texture2D tex)
    {
        EnsureSpriteAsset();

        if (textureToGlyphIndex.TryGetValue(tex, out uint index))
            return index;

        uint newIndex = (uint)runtimeSpriteAsset.spriteGlyphTable.Count;
        Texture2D atlas = runtimeSpriteAsset.spriteSheet as Texture2D;

        int size = 32;
        int cols = atlas.width / size;
        int x = (int)(newIndex % cols) * size;
        int y = atlas.height - ((int)(newIndex / cols) + 1) * size;

        Texture2D readable = tex.isReadable ? tex : GetReadableTextureCopy(tex);

        int copyW = Mathf.Min(size, readable.width);
        int copyH = Mathf.Min(size, readable.height);

        Color[] src = readable.GetPixels(0, 0, copyW, copyH);

        // Crear buffer del tamaño correcto
        Color[] dst = new Color[size * size];

        // Copiar fila por fila
        for (int row = 0; row < copyH; row++)
        {
            for (int col = 0; col < copyW; col++)
            {
                dst[row * size + col] = src[row * copyW + col];
            }
        }

        atlas.SetPixels(x, y, size, size, dst);
        atlas.Apply();

        atlas.Apply();

        var glyph = new TMP_SpriteGlyph(
            newIndex,
            new GlyphMetrics(size, size, 0, 0, size),
            new GlyphRect(x, y, size, size),
            1,
            0
        );

        var character = new TMP_SpriteCharacter
        {
            name = $"icon_{newIndex}",
            unicode = 0xE000 + newIndex,
            glyphIndex = newIndex,
            scale = 1
        };

        runtimeSpriteAsset.spriteGlyphTable.Add(glyph);
        runtimeSpriteAsset.spriteCharacterTable.Add(character);

        runtimeSpriteAsset.UpdateLookupTables();

        textureToGlyphIndex.Add(tex, newIndex);

        return newIndex;
    }


    private Texture2D GetReadableTextureCopy(Texture tex)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            tex.width,
            tex.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(tex, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(tex.width, tex.height);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }

}
#endif
