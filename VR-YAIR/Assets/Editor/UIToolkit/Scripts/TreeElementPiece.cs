using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeElementPiece
{
    public string LineNameStr;
    public List<TreeElementComponentData> ComponentData;

    public TreeElementPiece(string lineString)
    {
        LineNameStr = lineString;
    }

    public void AddComponentIcons(GameObject reference)
    {
        var GOComponents = reference.GetComponents(typeof(Component));
        ComponentData = new List<TreeElementComponentData>();
        var i = 0;
        foreach(var comp in GOComponents)
        {
            int index = -1;
            var componentType = comp.GetType();
            int j = 0;
            foreach (var compAlmacenado in ComponentData)
            {
                if(compAlmacenado.ComponentIcon == ((Texture2D)EditorGUIUtility.ObjectContent(comp, componentType).image))
                {
                    index = j;
                    break;
                }
                j++;
            }
            if(index == -1)
            {
                ComponentData.Add(new TreeElementComponentData()
                {
                    ComponentCount = 1,
                    ComponentIcon = (Texture2D)EditorGUIUtility.ObjectContent(comp, componentType).image,
                    ComponentType = componentType.ToString(),
                    rawComponent = comp
                });
            }
            else
            {
                ComponentData[index].ComponentCount++;
            }
            i++;
        }
        foreach (var comp in ComponentData)
        {
            if (comp.rawComponent is MonoBehaviour && !(comp.rawComponent is VRC.SDKBase.Network.VRCNetworkBehaviour || comp.rawComponent is VRC.SDK3.Network.VRCNetworkBehaviour))
                comp.ComponentCount /= 2 ;
        }
        Debug.Log("All icons added");
    }

    public class TreeElementComponentData
    {
        public string ComponentType;
        public Component rawComponent;
        public int ComponentCount;
        public Texture2D ComponentIcon;
    }

}
