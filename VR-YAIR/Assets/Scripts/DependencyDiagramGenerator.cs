using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DependencyDiagramGenerator : EditorWindow
{
    private static string outputPath = "Assets/Dependencies.mmd";

    [MenuItem("Tools/Generate Dependency Diagram (Avanzado)")]
    public static void GenerateDiagram()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        // 1. Detectar todas las clases definidas
        var classDefinitions = new HashSet<string>();
        var fileContents = new Dictionary<string, string>();

        foreach (string file in files)
        {
            string content = File.ReadAllText(file);

            // Quitar comentarios y strings para evitar falsos positivos
            //content = StripCommentsAndStrings(content);

            fileContents[file] = content;

            // Detectar definiciones de clase
            MatchCollection matches = Regex.Matches(content, @"\bclass\s+(\w+)");
            foreach (Match match in matches)
            {
                classDefinitions.Add(match.Groups[1].Value);
            }
        }

        // 2. Detectar dependencias reales
        var dependencies = new HashSet<(string from, string to)>();

        foreach (var kvp in fileContents)
        {
            string content = kvp.Value;
            string fileName = Path.GetFileNameWithoutExtension(kvp.Key);

            foreach (string className in classDefinitions)
            {
                if (fileName == className) continue; // No se depende de sí mismo

                // 2a. Herencia
                if (Regex.IsMatch(content, @"\bclass\s+" + Regex.Escape(fileName) + @"\s*:\s*" + Regex.Escape(className) + @"\b"))
                {
                    dependencies.Add((fileName, className));
                    continue;
                }

                if (Regex.IsMatch(content, @"\b" + className + @"\b"))
                {
                    dependencies.Add((fileName, className));
                }

                // 2b. Uso como tipo de variable o parámetro
                if (Regex.IsMatch(content, @"\b" + Regex.Escape(className) + @"\s+\w+\b"))
                {
                    dependencies.Add((fileName, className));
                    continue;
                }

                // 2c. Llamadas a métodos estáticos o de instancia
                if (Regex.IsMatch(content, @"\b" + Regex.Escape(className) + @"\s*\.\s*\w+\b"))
                {
                    dependencies.Add((fileName, className));
                    continue;
                }
            }
        }

        // 3. Generar archivo Mermaid
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
        Debug.Log($"Diagrama avanzado generado en: {outputPath}");
    }


    [MenuItem("Tools/Generate Full Dependency Diagram")]
    public static void GenerateFullDiagram()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        // 1. Detectar todas las clases definidas
        var classDefinitions = new HashSet<string>();
        var fileContents = new Dictionary<string, string>();

        foreach (string file in files)
        {
            string content = File.ReadAllText(file);
            fileContents[file] = content;

            MatchCollection matches = Regex.Matches(content, @"\bclass\s+(\w+)");
            foreach (Match match in matches)
            {
                classDefinitions.Add(match.Groups[1].Value);
            }
        }

        // 2. Detectar dependencias (cuando un archivo usa otra clase)
        var dependencies = new List<(string from, string to)>();

        foreach (var kvp in fileContents)
        {
            string content = kvp.Value;
            string fileName = Path.GetFileNameWithoutExtension(kvp.Key);

            foreach (string className in classDefinitions)
            {
                if (fileName == className) continue; // No se depende de sí mismo

                if (Regex.IsMatch(content, @"\b" + className + @"\b"))
                {
                    dependencies.Add((fileName, className));
                }
            }
        }

        // 3. Generar archivo Mermaid
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
        Debug.Log($"Diagrama generado en: {outputPath}");
    }



/// <summary>
/// Quita comentarios y strings de un código C# para evitar falsos positivos.
/// </summary>
private static string StripCommentsAndStrings(string code)
    {
        // Quitar comentarios multilínea /* ... */
        code = Regex.Replace(code, @"/\*.*?\*/", "", RegexOptions.Singleline);

        // Quitar comentarios de línea // ...
        code = Regex.Replace(code, @"//.*", "");

        // Quitar strings "..."
        code = Regex.Replace(code, @"""([^""\\]|\\.)*""", "");

        return code;
    }
}
