using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LinuxLikeTreePrinterUIToolkit : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private ObjectField _TargetGO;
    private ScrollView _TreeScrollView;

    [MenuItem("Window/LinuxLikeTreePrinterUIToolkit")]
    public static void ShowExample()
    {
        LinuxLikeTreePrinterUIToolkit wnd = GetWindow<LinuxLikeTreePrinterUIToolkit>();
        wnd.titleContent = new GUIContent("LinuxLikeTreePrinterUIToolkit");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        var selection = Selection.activeGameObject;
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        _TargetGO = labelFromUXML.Q<ObjectField>("GameObjectField");
        _TargetGO.RegisterValueChangedCallback(evt => 
        {            
            if (evt.newValue == null || !((GameObject)evt.newValue).scene.IsValid()) //return if its not a gameobject in scene
            {
                return;
            }
            GenerateLinuxTree((GameObject)evt.newValue);
        }); 
        
        _TreeScrollView = labelFromUXML.Q<ScrollView>("ElementsScrollView");
    }

    private void GenerateLinuxTree(GameObject newGameObject)
    {
        string FullLog = "";
        List<LinuxLikeTreePieceVisualElement> ElementList = new();
        RecursiveTreeElement(ref FullLog, ref ElementList, newGameObject, new List<bool> { }, 0, true);
        _TreeScrollView.Clear();
        foreach (LinuxLikeTreePieceVisualElement Element in ElementList)
        {
            _TreeScrollView.Add(Element);
        }
        Debug.Log(FullLog);
        
    }

    private void RecursiveTreeElement(ref string FullLog, ref List<LinuxLikeTreePieceVisualElement> LinuxElementList, GameObject GOToDescribe, List<bool> containsPrevLine, int DepthLevel = 0, bool isLastChild = false)
    {
        string PrevToUnderline = "";
        for (int j = 0; j < DepthLevel; j++)
        {
            PrevToUnderline += (containsPrevLine[j] ? "│   " : "    ");
        }
        string Underline = "├─ ";
        if (containsPrevLine.Count <= DepthLevel)
            containsPrevLine.Add(false);
        if (isLastChild)
        {
            Underline = "└─ ";
            containsPrevLine[DepthLevel] = false;
        }
        else
        {
            if (containsPrevLine.Count <= DepthLevel)
                containsPrevLine.Add(true);
            else
                containsPrevLine[DepthLevel] = true;
        }
        Underline = PrevToUnderline + Underline + GOToDescribe.name + "\n";
        Debug.Log(Underline);
        FullLog += Underline;
        var LinuxLikeElementPiece = new LinuxLikeTreePieceVisualElement();
        var xd = new TreeElementPiece(Underline);
        xd.AddComponentIcons(GOToDescribe);
        LinuxLikeElementPiece.SetData(xd);
        LinuxLikeElementPiece.style.height = 18.4f;
        LinuxElementList.Add(LinuxLikeElementPiece);
        int iteration = 0;
        foreach (Transform child in GOToDescribe.transform)
        {
            RecursiveTreeElement(ref FullLog, ref LinuxElementList, child.gameObject, containsPrevLine, DepthLevel + 1, iteration == GOToDescribe.transform.childCount - 1);
            iteration++;
        }
    }
    
}
