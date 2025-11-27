using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class ClassCallGraphGenerator : EditorWindow
{
    private static string targetClassName = "BotellaLab";   // <--- EDITA AQUÍ LA CLASE A ANALIZAR
    private static string outputPath = "Assets/ClassCallGraph.mmd";

    [MenuItem("Tools/Generate Class Call Graph")]
    public static void GenerateGraph()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        // Datos
        var outgoingCalls = new List<(string fromClass, string methodName, string toClass)>();
        var incomingCalls = new List<(string fromClass, string methodName, string toClass)>();

        // INDEX: NombreClase ? Diccionario Métodos
        var classMethods = new Dictionary<string, HashSet<string>>();

        foreach (var file in files)
        {
            string code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();

            foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string className = classDecl.Identifier.Text;

                if (!classMethods.ContainsKey(className))
                    classMethods[className] = new HashSet<string>();

                // Registrar métodos
                foreach (var method in classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    classMethods[className].Add(method.Identifier.Text);
                }
            }
        }

        // SEGUNDA PASADA: detectar llamadas
        foreach (var file in files)
        {
            string code = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();

            foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string fromClass = classDecl.Identifier.Text;

                foreach (var methodDecl in classDecl.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    string fromMethod = methodDecl.Identifier.Text;

                    // Búsqueda de invocaciones
                    foreach (var call in methodDecl.DescendantNodes().OfType<InvocationExpressionSyntax>())
                    {
                        string calledMethod = call.Expression.ToString();

                        // Detectar "Clase.Metodo" o "obj.Metodo"
                        string classTarget = null;
                        string methodTarget = null;

                        if (call.Expression is MemberAccessExpressionSyntax access)
                        {
                            classTarget = access.Expression.ToString();
                            methodTarget = access.Name.Identifier.Text;
                        }
                        else if (call.Expression is IdentifierNameSyntax id)
                        {
                            // Llamada local: MyMethod()
                            methodTarget = id.Identifier.Text;

                            // ¿Es de target class?
                            if (classMethods.ContainsKey(targetClassName) &&
                                classMethods[targetClassName].Contains(methodTarget))
                            {
                                classTarget = targetClassName;
                            }
                        }

                        if (classTarget == null || methodTarget == null)
                            continue;

                        // OUTGOING: targetClass llama a otros
                        if (fromClass == targetClassName)
                        {
                            outgoingCalls.Add((fromClass, fromMethod + "()", classTarget));
                        }

                        // INCOMING: otros llaman al targetClass
                        if (classTarget == targetClassName)
                        {
                            incomingCalls.Add((fromClass, fromMethod + "()", classTarget));
                        }
                    }
                }
            }
        }

        // GENERAR MERMAID
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("```mermaid");
            writer.WriteLine("graph TD");

            // Outgoing: targetClass ? others
            foreach (var call in outgoingCalls)
            {
                writer.WriteLine($"    {call.fromClass} -->|\"{call.methodName}\"| {call.toClass}");
            }

            // Incoming: others ? targetClass
            foreach (var call in incomingCalls)
            {
                writer.WriteLine($"    {call.fromClass} -->|\"{call.methodName}\"| {call.toClass}");
            }

            writer.WriteLine("```");
        }

        AssetDatabase.Refresh();
        Debug.Log("Graph generated: " + outputPath);
    }
}
