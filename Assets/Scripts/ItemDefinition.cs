using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName ="New Item", menuName ="Data/Item")]
public class ItemDefinition : ScriptableObject
{
    public string ID = Guid.NewGuid().ToString();
    public string FriendlyName;
    public string Description;
    public int SellPrice;
    public Sprite Icon;
    public Dimensions SlotDimension;
}

[Serializable]
public struct Dimensions
{
    
    [Min(1)]int? height;
    [Min(1)]int? width;
    public int Height { get { return height ?? 1; } set { height = value; } }
    public int Width{ get { return width ?? 1; } set { width = value; } }
}

