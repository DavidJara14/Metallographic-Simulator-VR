#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TreePrinterUIToolkit : EditorWindow
{
    private GameObject root;
    private ScrollView scroll;

    [MenuItem("Window/Tree Printer (UI Toolkit - Text Tree)")]
    public static void Open()
    {
        var wnd = GetWindow<TreePrinterUIToolkit>();
        wnd.titleContent = new GUIContent("Tree Printer (Text)");
        wnd.minSize = new Vector2(500, 300);
    }

    private void CreateGUI()
    {
        rootVisualElement.Clear();

        var toolbar = new VisualElement();
        toolbar.style.flexDirection = FlexDirection.Row;
        toolbar.style.paddingLeft = 6;
        toolbar.style.paddingTop = 4;
        toolbar.style.paddingBottom = 4;

        var objField = new ObjectField("Root")
        {
            objectType = typeof(GameObject),
            allowSceneObjects = true
        };
        objField.RegisterValueChangedCallback(e =>
        {
            root = e.newValue as GameObject;
            Rebuild();
        });

        toolbar.Add(objField);
        rootVisualElement.Add(toolbar);

        scroll = new ScrollView();
        scroll.style.flexGrow = 1;
        scroll.style.paddingLeft = 6;
        scroll.style.paddingTop = 4;

        // Fuente monoespaciada
        var font = AssetDatabase.LoadAssetAtPath<Font>(
    "Assets/Editor/Fonts/GoogleSansCode-Regular.ttf"
);
        scroll.style.unityFont = font;

        rootVisualElement.Add(scroll);
    }

    private void Rebuild()
    {
        scroll.Clear();
        if (root == null) return;

        DrawNode(root.transform, "", true, new List<bool>());
    }

    // flags: indica si cada ancestro fue el último en su nivel
    private void DrawNode(Transform t, string prefix, bool isLast, List<bool> flags)
    {
        string line = BuildPrefix(flags, isLast) + t.name;

        var label = new Label(line);
        label.style.whiteSpace = WhiteSpace.Normal;
        label.style.unityFontStyleAndWeight = FontStyle.Normal;

        scroll.Add(label);

        // preparar flags para hijos
        var newFlags = new List<bool>(flags) { isLast };

        for (int i = 0; i < t.childCount; i++)
        {
            bool childIsLast = (i == t.childCount - 1);
            DrawNode(t.GetChild(i), "", childIsLast, newFlags);
        }
    }

    private string BuildPrefix(List<bool> flags, bool isLast)
    {
        string result = "";

        // Ancestors
        for (int i = 0; i < flags.Count; i++)
        {
            if (flags[i])
                result += "       ";   // last ancestor -> blank
            else
                result += "│      ";  // vertical line
        }

        // Current node
        if (flags.Count > 0)
        {
            if (isLast)
                result += "└─ ";
            else
                result += "├─ ";
        }

        return result;
    }
}
#endif
