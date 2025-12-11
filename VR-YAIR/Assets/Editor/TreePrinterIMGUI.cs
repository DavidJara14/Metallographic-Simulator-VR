#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TreePrinterIMGUI : EditorWindow
{
    private GameObject root;
    private Vector2 scroll;

    [MenuItem("Window/Tree Printer (IMGUI)")]
    public static void Open()
    {
        GetWindow<TreePrinterIMGUI>("Tree Printer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tree Printer (IMGUI)", EditorStyles.boldLabel);
        GUILayout.Space(5);

        root = (GameObject)EditorGUILayout.ObjectField(
            "Root GameObject",
            root,
            typeof(GameObject),
            true
        );

        GUILayout.Space(5);

        if (root == null)
        {
            EditorGUILayout.HelpBox("Assign a root GameObject.", MessageType.Info);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawTransformTree(root.transform, "", true);

        EditorGUILayout.EndScrollView();
    }

    private void DrawTransformTree(Transform t, string prefix, bool isLast)
    {
        Rect lineRect = EditorGUILayout.GetControlRect(false, 18);

        string branch = prefix + (isLast ? "└─ " : "├─ ");
        string nameText = branch + t.name;

        // Dibujar texto nombre
        EditorGUI.LabelField(
            new Rect(lineRect.x, lineRect.y, lineRect.width, lineRect.height),
            nameText
        );

        float iconOffset = GUI.skin.label.CalcSize(new GUIContent(nameText)).x + 5;

        // Dibujar iconos de componentes
        var counts = GetComponentIconCounts(t.gameObject);

        float x = lineRect.x + iconOffset;

        foreach (var kv in counts)
        {
            Texture icon = kv.Key;
            int count = kv.Value;

            if (icon == null)
                continue;

            Rect iconRect = new Rect(x, lineRect.y + 1, 16, 16);
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

            x += 18;

            if (count > 1)
            {
                GUI.Label(
                    new Rect(x, lineRect.y, 24, lineRect.height),
                    "x" + count
                );
                x += 20;
            }
        }

        // Prefijo para hijos
        string childPrefix = prefix + (isLast ? "        " : "│     ");

        for (int i = 0; i < t.childCount; i++)
        {
            DrawTransformTree(
                t.GetChild(i),
                childPrefix,
                i == t.childCount - 1
            );
        }
    }

    private Dictionary<Texture, int> GetComponentIconCounts(GameObject go)
    {
        var result = new Dictionary<Texture, int>();
        var isScriptIcon = new Dictionary<Texture, bool>();

        var components = go.GetComponents<Component>();

        foreach (var c in components)
        {
            if (c == null) continue;

            var content = EditorGUIUtility.ObjectContent(c, c.GetType());
            var icon = content.image;
            if (icon == null) continue;

            if (!result.ContainsKey(icon))
            {
                result[icon] = 0;
                isScriptIcon[icon] = false;
            }

            result[icon]++;

            // Marcar si es script C#
            if (c is MonoBehaviour)
                isScriptIcon[icon] = true;
        }

        // Post-procesar scripts
        var keys = new List<Texture>(result.Keys);

        foreach (var tex in keys)
        {
            if (isScriptIcon.TryGetValue(tex, out bool isScript) && isScript)
            {
                result[tex] = Mathf.Max(1, result[tex] / 2);
            }
        }

        return result;
    }


}
#endif
