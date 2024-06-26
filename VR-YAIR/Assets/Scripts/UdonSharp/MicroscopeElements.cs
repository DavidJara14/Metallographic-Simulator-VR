using System;
using System.Xml.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.Udon;

public class MicroscopeElements : UdonSharpBehaviour
{
    [SerializeField] public ElementType[] elementos;
    public Sprite placeHolder;
}

