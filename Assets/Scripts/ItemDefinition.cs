using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName ="New Item", menuName ="Data/Item")]
public class ItemDefinition : ScriptableObject
{
    [HideInInspector]public string ID = Guid.NewGuid().ToString();
    [HideInInspector]public string AssetName;
    public Rarity Rarity;
    public string ItemName;
    public string Description;
    public int SellPrice;
    public Vector2Int SlotDimension = new Vector2Int(1,1);
    public Sprite Icon;
    
}

[Serializable]
public struct Dimensions
{
    /*[Min(1)]int? height;
    [Min(1)]int? width;
    public int Height { get { return height ?? 1; } set { height = value; } }
    public int Width{ get { return width ?? 1; } set { width = value; } }*/
    
    public int Height;
    public int Width;
}
public enum Rarity
{
    Common,
    Uncommon, 
    Rare,
    VeryRare,
    Mythic
}

