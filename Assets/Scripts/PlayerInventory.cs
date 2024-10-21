using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


[Serializable]
public class StoredItem
{
    [HideInInspector]public string ID = Guid.NewGuid().ToString();
    public ItemDefinition Details;
    public ItemVisual RootVisual;
}

public sealed class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    
    private bool m_IsInventoryReady;
    public List<StoredItem> StoredItems = new List<StoredItem>();
    
    private VisualElement m_Root;
    private VisualElement m_InventoryGrid;
    public Dimensions InventoryDimensions;

    private static Label m_ItemDetailHeader;
    private static Label m_ItemDetailBody;
    private static Label m_ItemDetailPrice;
    
    private VisualElement m_Telegraph;
    private Button m_ButtonDrop;
    private Button m_ButtonEquip;

    private static string currentItemID;

    private UIDocument _uiDocument;

    [SerializeField] public UnityEvent _event;

    private ItemVisual _itemVisual; 
    public static Dimensions SlotDimension { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _uiDocument = GetComponentInChildren<UIDocument>();
            Configure();
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        
        LoadInventory();
        
    }

    private void OnDestroy()
    {
        if(m_ButtonDrop!= null)
            m_ButtonDrop.clickable.clicked -= OnDropClicked;
        if(m_ButtonEquip!= null)
            m_ButtonEquip.clickable.clicked -= OnEquipClicked;
    }

    private async void Configure()
    {
        m_Root = _uiDocument.rootVisualElement;
        m_InventoryGrid = m_Root.Q<VisualElement>("Grid");
        foreach (var child in m_InventoryGrid.Children())
        {
            if (child.name != "SlotIcon")
            {
                m_InventoryGrid.Remove(child);
            }
        }
        VisualElement itemDetails = m_Root.Q<VisualElement>("ItemDetails");

        m_ItemDetailHeader = itemDetails.Q<Label>("ItemName");
        m_ItemDetailBody = itemDetails.Q<Label>("Description");
        m_ItemDetailPrice = itemDetails.Q<Label>("SellPrice");
        m_ButtonDrop = itemDetails.Q<Button>("btn_Drop");
        m_ButtonDrop.clickable.clicked += OnDropClicked;
        m_ButtonEquip = itemDetails.Q<Button>("btn_Equip");
        m_ButtonEquip.clickable.clicked += OnEquipClicked;
        ConfigureInventoryTelegraph();

        await UniTask.WaitForEndOfFrame();

        ConfigureSlotDimensions();
        m_IsInventoryReady = true;
    }
    
    private void ConfigureInventoryTelegraph()
    {
        m_Telegraph = new VisualElement
        {
            name = "Telegraph",
        };

        //set style
        m_Telegraph.AddToClassList("slot-icon-highlighted");

        //Add to UI
        AddItemToInventoryGrid(m_Telegraph);
    } 
    
    private void ConfigureSlotDimensions()
    {
        VisualElement firstSlot = m_InventoryGrid.Children().First();
        SlotDimension = new Dimensions
        {
            Width = Mathf.RoundToInt(firstSlot.worldBound.width),
            Height = Mathf.RoundToInt(firstSlot.worldBound.height)
        };
    }
    
    private void AddItemToInventoryGrid(VisualElement item)
    {
        m_InventoryGrid.Add(item);
    }

    private void RemoveItemFromInventoryGrid(VisualElement item)
    {
        m_InventoryGrid.Remove(item);
    }

    private async void LoadInventory()
    {
        //make sure inventory is in ready state
        await UniTask.WaitUntil(() => m_IsInventoryReady);
        //load

        foreach (StoredItem loadedItem in StoredItems)
        {
            
            
            loadedItem.ID = Guid.NewGuid().ToString();
            _itemVisual = new ItemVisual(loadedItem.Details, loadedItem.ID);

            AddItemToInventoryGrid(_itemVisual);
         
            

            bool inventoryHasSpace = await GetPositionForItem(_itemVisual);
            if (!inventoryHasSpace)
            {
                Debug.Log("No space - Cannot pick up the item");
                RemoveItemFromInventoryGrid(_itemVisual);
                continue;
            }
            ConfigureInventoryItem(loadedItem, _itemVisual);
        }
        Debug.Log(m_InventoryGrid.childCount);
    }
    
    
    private static void ConfigureInventoryItem(StoredItem item, ItemVisual visual)
    {
        item.RootVisual = visual;
        visual.style.visibility = Visibility.Visible;
    }
    
    public static void UpdateItemDetails(ItemDefinition item, string ID)
    {
        
        currentItemID = ID;
        m_ItemDetailHeader.text = item.ItemName;
        m_ItemDetailBody.text = item.Description;
        m_ItemDetailPrice.text = item.SellPrice.ToString();
    }
    
    private static void SetItemPosition(VisualElement element, Vector2 vector)
    {
        element.style.left = vector.x;
        element.style.top = vector.y;
    }
    
    
    private async Task<bool> GetPositionForItem(VisualElement newItem)
    {
        for (int y = 0; y < InventoryDimensions.Height; y++)
        {
            for (int x = 0; x < InventoryDimensions.Width; x++)
            {
                //try position
                SetItemPosition(newItem, new Vector2(SlotDimension.Width * x, 
                    SlotDimension.Height * y));

                await UniTask.WaitForEndOfFrame();

                StoredItem overlappingItem = StoredItems.FirstOrDefault(s => 
                    s.RootVisual != null && 
                    s.RootVisual.layout.Overlaps(newItem.layout));

                //Nothing is here! Place the item.
                if (overlappingItem == null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    


    
    
    
    public (bool canPlace, Vector2 position) ShowPlacementTarget(ItemVisual draggedItem)
    {
        //Check to see if it's hanging over the edge - if so, do not place.
        if (!m_InventoryGrid.layout.Contains(new Vector2(draggedItem.localBound.xMax, draggedItem.localBound.yMax)))
        {
            m_Telegraph.style.visibility = Visibility.Hidden;
            return (canPlace: false, position: Vector2.zero);
        }

        VisualElement targetSlot = m_InventoryGrid.Children().Where(x => x.layout.Overlaps(draggedItem.layout) && x != draggedItem).OrderBy(x => Vector2.Distance(x.worldBound.position, draggedItem.worldBound.position)).First();

        m_Telegraph.style.width = draggedItem.style.width;
        m_Telegraph.style.height = draggedItem.style.height;

        SetItemPosition(m_Telegraph, new Vector2(targetSlot.layout.position.x, targetSlot.layout.position.y));

        m_Telegraph.style.visibility = Visibility.Visible;

        var overlappingItems = StoredItems.Where(x => x.RootVisual != null && x.RootVisual.layout.Overlaps(m_Telegraph.layout)).ToArray();

        if (overlappingItems.Length > 1)
        {
            m_Telegraph.style.visibility = Visibility.Hidden;
            return (canPlace: false, position: Vector2.zero);
        }

        return (canPlace: true, targetSlot.worldBound.position);

    }
    
    private void OnDropClicked()
    {
        foreach (var Item in StoredItems)
        {
            if (currentItemID == Item.ID)
            {
                RemoveItemFromInventoryGrid(Item.RootVisual);
            }
        }
    }

    private void OnEquipClicked()
    {
        Debug.Log($"equip is not implemented right now");
    }

    public void UIEnabled()
    {
        /*_uiDocument.enabled = true;
        Configure();
        Debug.Log("enable");
        
        LoadInventory();*/
    }
    public void UIDisabled()
    {
        /*_uiDocument.enabled = false;
        Debug.Log("disabled");*/
    }
    
}
