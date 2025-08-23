using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class RoslynDependencyDiagramGenerator : EditorWindow
{
    private static string outputPath = "Assets/RoslynDependencies.mmd";

    [MenuItem("Tools/Generate Roslyn Dependency Diagram")]
    public static void GenerateDiagram()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        // Guardamos dependencias con tipo (variable o método)
        var dependencies = new HashSet<(string from, string to, string kind)>();

        foreach (var file in files)
        {
            string code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();

            var classDecls = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDecl in classDecls)
            {
                string from = classDecl.Identifier.Text;

                //// 1. Herencia
                //if (classDecl.BaseList != null)
                //{
                //    foreach (var baseType in classDecl.BaseList.Types)
                //        dependencies.Add((from, baseType.Type.ToString(), "variable"));
                //}

                // 2. Campos
                foreach (var field in classDecl.DescendantNodes().OfType<FieldDeclarationSyntax>())
                {
                    string typeName = field.Declaration.Type.ToString();
                    dependencies.Add((from, typeName, "variable"));
                }

                // 3. Propiedades
                foreach (var prop in classDecl.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                {
                    string typeName = prop.Type.ToString();
                    dependencies.Add((from, typeName, "variable"));
                }

                //// 4. Métodos
                //foreach (var method in classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>())
                //{
                //    // Tipo de retorno
                //    string returnType = method.ReturnType.ToString();
                //    if (returnType != "void")
                //        dependencies.Add((from, returnType, "variable"));

                //    // Parámetros
                //    foreach (var param in method.ParameterList.Parameters)
                //        dependencies.Add((from, param.Type.ToString(), "variable"));

                //    // Instanciaciones (new ClaseX())
                //    foreach (var obj in method.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
                //        dependencies.Add((from, obj.Type.ToString(), "method"));

                //    // Llamadas a métodos
                //    foreach (var inv in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
                //    {
                //        if (inv.Expression is MemberAccessExpressionSyntax memberAccess)
                //        {
                //            string className = memberAccess.Expression.ToString();
                //            // Heurística: solo considerar nombres que parezcan tipos
                //            if (!string.IsNullOrWhiteSpace(className) && char.IsUpper(className[0]))
                //                dependencies.Add((from, className, "method"));
                //        }
                //    }
                //}
            }
        }

        // 5. Generar archivo Mermaid
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("```mermaid");
            writer.WriteLine("graph TD");

            foreach (var dep in dependencies)
            {
                if (dep.from == dep.to || string.IsNullOrWhiteSpace(dep.to))
                    continue;

                string arrow = dep.kind switch
                {
                    "variable" => $"--> |variable|",
                    "method" => $"-.-|llama|",
                    _ => "-->"
                };

                writer.WriteLine($"    {dep.from} {arrow} {dep.to}");
            }

            writer.WriteLine("```");
        }

        AssetDatabase.Refresh();
        Debug.Log($"Diagrama generado con Roslyn en: {outputPath}");
    }
}
