#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class TreePrinterMenu
{
    [MenuItem("Tools/Tree Printer/Print Selected GameObject Tree")]
    public static void PrintSelectedTree()
    {
        GameObject go = Selection.activeGameObject;

        if (go == null)
        {
            Debug.LogError("No GameObject selected.");
            return;
        }

        TreePrinter.PrintTree(go);
    }
}
#endif
