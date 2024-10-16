using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;

public class MyCustomEditor : EditorWindow
{
    [SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_RightPane;
    private ItemDefinition lastItem;
    private string newAssetName;
    private string[] ItemsGuids;
    private string path;
    private VisualElement leftPane;
    private EnumField SortDrop;
    private ListView leftList;
    private string selectedName;
    private string newname;
    private string lastname;
    private List<ItemDefinition> allItems;
    private Sorting sortingtype = Sorting.Alphabet;

    [MenuItem("Window/ItemManager")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("Item Manager");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
  }

    private void OnEnable()
    {
        lastname = null;
    }

    

    private List<ScriptableObject> FindScriptable()
    {
        var allItemsGuids = AssetDatabase.FindAssets("t:ItemDefinition");
        var allItems = new List<ScriptableObject>();
      
        foreach (var guid in allItemsGuids)
        {
            allItems.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        return allItems;
    }

    public void CreateGUI()
    {
      // Get a list of all sprites in the project.

      // Create a two-pane view with the left pane being fixed.
      var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
      

      // Add the panel to the visual tree by adding it as a child to the root element.
      rootVisualElement.Add(splitView);

      // A TwoPaneSplitView always needs two child elements.
      leftPane = new VisualElement();
      
      leftList = new ListView();
      SortDrop = new EnumField(Sorting.Alphabet);
      leftPane.Add(SortDrop);
      leftPane.Add(leftList);
      splitView.Add(leftPane);
      m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
      splitView.Add(m_RightPane);

      // Initialize the list view with all sprites' names.
      MakeUI();

      Button button = new Button();
      button.name = "create";
      button.text = "Create New Item";
      leftList.hierarchy.Add(button);
      
      
      button.clicked += ButtonOnClicked;

      
      //TODO: Make it so that it doesnt break on hotload
      SortDrop.style.alignSelf = Align.FlexStart;
      SortDrop.RegisterCallback<ChangeEvent<Enum>>(evt =>
      {
          sortingtype = (Sorting)Enum.Parse(typeof(Sorting), evt.newValue.ToString());
          MakeUI();
      });
      

      // React to the user's selection.
      leftList.selectionChanged += OnItemSelectionChange;
      

      
      
      // Restore the selection index from before the hot reload.
      leftList.selectedIndex = m_SelectedIndex;

      // Store the selection index when the selection changes.
      leftList.selectionChanged += (items) => { m_SelectedIndex = leftList.selectedIndex; };
  }

    private void ButtonOnClicked()
    {
        m_RightPane.Clear();

        var Save = new Button();
        Save.text = "Save";
        
        
        var name = new TextField();
        name.RegisterValueChangedCallback(evt =>
        {
            newname = name.value;
        });
        
        
        Save.clicked += CreateItem;
        m_RightPane.Add(name);
        m_RightPane.Add(Save);


        
        
    }
    
    

    private void OnItemSelectionChange(IEnumerable<object> selectedItems)
  {
      
      // Clear all previous content from the pane.
      m_RightPane.Clear();
      
      
      
      var enumerator = selectedItems.GetEnumerator();
      if (enumerator.MoveNext())
      {
          
          var sc = enumerator.Current as ItemDefinition;
          if (sc != null)
          {
              AssetDatabase.RenameAsset(path, newAssetName);
              ItemsGuids = AssetDatabase.FindAssets(sc.name);
              path = AssetDatabase.GUIDToAssetPath(ItemsGuids[0]);
              selectedName = sc.name;


              var Aname = new TextField("Name of asset");
              Aname.RegisterValueChangedCallback(evt =>
              {
                  newAssetName = Aname.value;
              });
              Aname.value = sc.name;
              Aname.style.alignSelf = Align.Stretch;
              Aname.style.marginTop = 10;
              
              var name = new TextField("Name of item")
              {
              };
              name.RegisterValueChangedCallback(evt =>
              {
                  sc.FriendlyName = name.value;
                  EditorUtility.SetDirty(sc);
              });
              name.value = sc.FriendlyName;
              name.style.alignSelf = Align.Stretch;
              name.style.marginTop = 10;

              var rarity = new EnumField("Rarity of item",sc.Rarity);
              
              rarity.RegisterCallback<ChangeEvent<Enum>>(evt =>
              {
                  sc.Rarity = (Rarity)Enum.Parse(typeof(Rarity), evt.newValue.ToString());
                  EditorUtility.SetDirty(sc);
              });
              
              rarity.style.alignSelf = Align.Stretch;


              var description = new TextField("Description of item");
              description.RegisterValueChangedCallback(evt =>
              {
                  sc.Description = description.value;
                  EditorUtility.SetDirty(sc);
              });
              description.value = sc.Description;
              description.multiline = true;
              description.style.alignSelf = Align.Stretch;
              
              var sellprice = new IntegerField("Sell price for item");
              sellprice.RegisterValueChangedCallback(evt =>
              {
                  sc.SellPrice = sellprice.value;
                  EditorUtility.SetDirty(sc);
              });
              sellprice.value = sc.SellPrice;
              sellprice.style.alignSelf = Align.Stretch;

              
              var icon = new ObjectField("Icon for item");
              icon.RegisterValueChangedCallback(evt =>
              {
                  sc.Icon = icon.value as Sprite;
                  EditorUtility.SetDirty(sc);
              });
              icon.objectType = typeof(Sprite);
              icon.value = sc.Icon;
              icon.style.alignSelf = Align.Stretch;
              
              var dimensions = new Vector2IntField("Dimensions for item");
              dimensions.RegisterValueChangedCallback(evt =>
              {
                  if (dimensions.value.x < 1)
                  {
                      dimensions.value = new Vector2Int(1,dimensions.value.y);
                  }
                  else if (dimensions.value.y < 1)
                  {
                      dimensions.value = new Vector2Int(dimensions.value.x,1);
                  }
                  else
                  {
                      sc.SlotDimension.x = dimensions.value.x;
                      sc.SlotDimension.y = dimensions.value.y;
                      EditorUtility.SetDirty(sc);
                  }
                  
              });
              dimensions.value = new Vector2Int(sc.SlotDimension.x, sc.SlotDimension.y);
              dimensions.style.alignSelf = Align.Stretch;
              
              
              var Save = new Button();
              Save.text = "Save";
              Save.style.marginTop = 30;
              Save.style.alignSelf =  Align.Center;
              Save.style.maxWidth = 400;
              Save.style.minWidth = 300;
              Save.style.maxHeight = 20;
              Save.style.minHeight = 20;
              Save.style.flexGrow = 1;
              
              var Delete = new Button();
              Delete.text = "Delete Item";
              Delete.style.marginTop = 10;
              Delete.style.maxWidth = 400;
              Delete.style.minWidth = 300;
              Delete.style.alignSelf =  Align.Center;
              Delete.style.maxHeight = 20;
              Delete.style.minHeight = 20;
              Delete.style.flexGrow = 1;
              
              
              m_RightPane.Add(Aname);
              m_RightPane.Add(name);
              m_RightPane.Add(rarity);
              m_RightPane.Add(description);
              m_RightPane.Add(sellprice);
              m_RightPane.Add(dimensions);
              m_RightPane.Add(icon);
              m_RightPane.Add(Save);
              m_RightPane.Add(Delete);

              Save.clicked += SaveItem;
              Delete.clicked += DeleteItem;
              leftList.RefreshItems();
              
          }
      }
  }

    //TODO: Fix this button
    private void SaveItem()
    {
        Debug.Log("Save");
        MakeUI();
    }
    private void DeleteItem()
    {
        if (selectedName!= null)
        {
            //Debug.Log($"Tar bort {selectedName}");
            AssetDatabase.DeleteAsset($"Assets/Scripts/SO/{selectedName}.asset");
            //Debug.Log($"Assets/Scripts/SO/{selectedName}.asset");
        }
        MakeUI();
     
        
        //SaveItem();
        leftList.SetSelection(-1);
    }

    private void CreateItem()
    {
        //TODO: Check the name of every SO
        //TODO: Fix visualization 
        if (newname != null && newname != lastname)
        {
            ItemDefinition item = ScriptableObject.CreateInstance<ItemDefinition>();
            AssetDatabase.CreateAsset(item, $"Assets/Scripts/SO/{newname}.asset");
            MakeUI();
            lastname = newname;

            var children = leftList.hierarchy.Children();
            int i = 0;
            foreach (var I in children)
            {
                foreach (var child in I.Children())
                {
                    i++;
                    if (child.name == newname)
                    {
                        leftList.SetSelection(i);
                        leftList.SetSelection(i-1);
                        return;
                    }
                }
            }
        }
    }

    private void MakeUI()
    {
        allItems = new List<ItemDefinition>();
        var allScriptable = FindScriptable();
        foreach (var scriptable in allScriptable)
        {
            if (scriptable is ItemDefinition itemDefinition)
            {
                allItems.Add(itemDefinition);
            }
        }
        leftList.Clear();
        Sort(allItems);
        leftList.makeItem = () => new Label();
        leftList.bindItem = (item, index) =>
        {
            (item as Label).text = allItems[index].name;
            (item as Label).name = allItems[index].name;
        };
        leftList.itemsSource = allItems;
    }

    private List<ItemDefinition> Sort(List<ItemDefinition> list)
    {
        if (sortingtype == Sorting.AlphabetReverse)
        {
            list.Reverse();
            return list;
        }
        if (sortingtype is Sorting.Rarity or Sorting.RarityReverse)
        {
            for (int i = 0; i < list.Count - i; i++)
            {
                for (int j = 0; j < list.Count - i - 1; j++)
                {
                    if (list[j].Rarity > list[j + 1].Rarity)
                    {

                        (list[j + 1], list[j]) =
                            (list[j], list[j + 1]);
                    }
                }
            }

            if (sortingtype == Sorting.Rarity) return list;
            list.Reverse();
            return list;
        }
        
        if (sortingtype is Sorting.Cost or Sorting.CostReverse)
        {
            for (int i = 0; i < list.Count - i; i++)
            {
                for (int j = 0; j < list.Count - i - 1; j++)
                {
                    if (list[j].SellPrice > list[j + 1].SellPrice)
                    {

                        (list[j + 1], list[j]) =
                            (list[j], list[j + 1]);
                    }
                }
            }
            if (sortingtype == Sorting.CostReverse) return list;
            list.Reverse();
            return list;
        }

        if (sortingtype is Sorting.Area or Sorting.AreaReverse)
        {
            for (int i = 0; i < list.Count - i; i++)
            {
                for (int j = 0; j < list.Count - i - 1; j++)
                {
                    if (list[j].SlotDimension.x*list[j].SlotDimension.y > list[j + 1].SlotDimension.x*list[j + 1].SlotDimension.y)
                    {

                        (list[j + 1], list[j]) =
                            (list[j], list[j + 1]);
                    }
                }
            }

            if (sortingtype == Sorting.Area) return list;
            list.Reverse();
            return list;
        }
        return list;
    }
}


public enum Sorting
{
    [InspectorName("Alphabet↓")]Alphabet,
    [InspectorName("Alphabet↑")]AlphabetReverse, 
    [InspectorName("Rarity↓")]Rarity,
    [InspectorName("Rarity↑")]RarityReverse,
    [InspectorName("Cost↓")]Cost,
    [InspectorName("Cost↑")]CostReverse,
    [InspectorName("Area↓")]Area,
    [InspectorName("Area↑")]AreaReverse
}
