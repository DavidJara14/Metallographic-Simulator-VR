#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneComponentsWithIcons : EditorWindow
{
    private List<(string name, Texture icon)> components = new();

    [MenuItem("Tools/Print Scene Components With Icons")]
    public static void ShowWindow()
    {
        var window = GetWindow<SceneComponentsWithIcons>("Scene Components");
        window.ScanScene();
        window.Show();
    }

    private void ScanScene()
    {
        HashSet<Type> uniqueTypes = new HashSet<Type>();
        components.Clear();

        Scene scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            var comps = root.GetComponentsInChildren<Component>(true);

            foreach (var c in comps)
            {
                if (c == null) continue;
                uniqueTypes.Add(c.GetType());
            }
        }

        foreach (var type in uniqueTypes.OrderBy(t => t.Name))
        {
            GUIContent content = EditorGUIUtility.ObjectContent(null, type);
            components.Add((type.Name, content.image));
        }
    }

    private Vector2 scroll;

    private void OnGUI()
    {
        if (GUILayout.Button("Rescan Scene"))
            ScanScene();

        scroll = GUILayout.BeginScrollView(scroll);

        foreach (var (name, icon) in components)
        {
            GUILayout.BeginHorizontal();

            if (icon != null)
                GUILayout.Label(icon, GUILayout.Width(60), GUILayout.Height(60));
            else
                GUILayout.Space(22);

            GUILayout.Label(name);

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}
#endif
