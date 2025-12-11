using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class LinuxLikeTreePieceVisualElement : VisualElement
{
    private Label _label;
    private ScrollView _iconScrollView;

    public LinuxLikeTreePieceVisualElement()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIToolkit/Extra/RobotElement.uxml");
        visualTree.CloneTree(this);

        _label = this.Q<Label>("LinuxTreeGONameLabel");
        _label.AddToClassList("label-no-spacing");
        //toggle.label = selection.gameObject.name;
        _iconScrollView = this.Q<ScrollView>("RobotCompScrollView");
    }

    public void SetData(TreeElementPiece pieceData)
    {
        _label.text = pieceData.LineNameStr;
        _iconScrollView.Clear();
        foreach(var data in pieceData.ComponentData)
        {
            if (data == null)
                continue;
            var iconVisualElement = new VisualElement();
            iconVisualElement.AddToClassList("icon");
            iconVisualElement.tooltip = data.ComponentType;
            Background bg = new()
            {
                texture = data.ComponentIcon
            };
            StyleBackground sbg = new()
            {
                value = bg
            };
            iconVisualElement.style.backgroundImage = sbg;
            iconVisualElement.style.width = 14;
            iconVisualElement.style.height = 14;

            _iconScrollView.Add(iconVisualElement);
            if(data.ComponentCount > 1)
            {
                var LabelxN = new Label($"x{data.ComponentCount}");
                LabelxN.AddToClassList("label-no-spacing");
                _iconScrollView.Add(LabelxN);
            }
        }
    }

}
