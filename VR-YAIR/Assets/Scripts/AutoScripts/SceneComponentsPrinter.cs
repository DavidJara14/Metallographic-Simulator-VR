using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneComponentsPrinter : MonoBehaviour
{
    [ContextMenu("Print Scene Components")]
    void PrintSceneComponents()
    {
        HashSet<Type> uniqueComponents = new HashSet<Type>();

        // Recorre todos los GameObjects de la escena activa
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            // Incluye hijos
            Component[] components = root.GetComponentsInChildren<Component>(true);

            foreach (Component comp in components)
            {
                if (comp != null)
                    uniqueComponents.Add(comp.GetType());
            }
        }

        // Ordena alfabéticamente por nombre
        var ordered = uniqueComponents
            .Select(t => t.Name)
            .OrderBy(n => n)
            .ToList();

        // Imprime separados por comas
        Debug.Log(string.Join(", ", ordered));
    }
}
