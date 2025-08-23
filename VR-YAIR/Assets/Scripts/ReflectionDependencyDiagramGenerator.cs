using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReflectionDependencyDiagramGenerator : MonoBehaviour
{
    private static string outputPath = "Assets/ReflectionDependencies.mmd";

    [MenuItem("Tools/Generate Reflection Dependency Diagram")]
    public static void GenerateDiagram()
    {
        string assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "Library/ScriptAssemblies/Assembly-CSharp.dll");

        if (!File.Exists(assemblyPath))
        {
            Debug.LogError("No se encontró Assembly-CSharp.dll. Asegúrate de que el proyecto esté compilado.");
            return;
        }

        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        var types = assembly.GetTypes()
            .Where(t => t.IsClass) // puedes filtrar aquí
            .ToList();

        var dependencies = new HashSet<(string from, string to)>();

        foreach (var type in types)
        {
            string from = type.Name;

            // 1. Herencia
            if (type.BaseType != null && type.BaseType.Assembly == assembly)
            {
                dependencies.Add((from, type.BaseType.Name));
            }

            // 2. Interfaces implementadas
            foreach (var iface in type.GetInterfaces().Where(i => i.Assembly == assembly))
            {
                dependencies.Add((from, iface.Name));
            }

            // 3. Campos y propiedades
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (field.FieldType.Assembly == assembly)
                    dependencies.Add((from, field.FieldType.Name));
            }

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (prop.PropertyType.Assembly == assembly)
                    dependencies.Add((from, prop.PropertyType.Name));
            }

            // 4. Métodos (parámetros y return types)
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                if (method.ReturnType.Assembly == assembly)
                    dependencies.Add((from, method.ReturnType.Name));

                foreach (var param in method.GetParameters())
                {
                    if (param.ParameterType.Assembly == assembly)
                        dependencies.Add((from, param.ParameterType.Name));
                }
            }
        }

        // 5. Generar archivo Mermaid
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("```mermaid");
            writer.WriteLine("graph TD");

            foreach (var dep in dependencies)
            {
                writer.WriteLine($"    {dep.from} --> {dep.to}");
            }

            writer.WriteLine("```");
        }

        AssetDatabase.Refresh();
        Debug.Log($"Diagrama con reflection generado en: {outputPath}");
    }
}
