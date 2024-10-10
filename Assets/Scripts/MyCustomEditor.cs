using System;
using System.Collections.Generic;
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
    private ListView leftPane;
    private string newname;
    private string lastname;

    [MenuItem("Window/UI Toolkit/MyCustomEditor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("My Custom Editor");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
  }


    /*public void OnGUI()
    {
        if (GUILayout.Button("Add"))
        {
            Debug.Log("Was Pressed");
        }
    }*/

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
      
      var allItems = new List<ScriptableObject>();
      allItems = FindScriptable();
      /*
      var allObjectGuids = AssetDatabase.FindAssets("t:Texture2D");
      var allObjects = new List<Texture2D>();
      foreach (var guid in allObjectGuids)
      {
        allObjects.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid)));
      }*/

      // Create a two-pane view with the left pane being fixed.
      var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
      

      // Add the panel to the visual tree by adding it as a child to the root element.
      rootVisualElement.Add(splitView);

      // A TwoPaneSplitView always needs two child elements.
      leftPane = new ListView();
      splitView.Add(leftPane);
      m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
      splitView.Add(m_RightPane);

      // Initialize the list view with all sprites' names.
      leftPane.makeItem = () => new Label();
      leftPane.bindItem = (item, index) => { (item as Label).text = allItems[index].name; };
      leftPane.itemsSource = allItems;

      Button button = new Button();
      button.name = "create";
      button.text = "Create New Item";
      leftPane.hierarchy.Add(button);
      
      button.clicked += ButtonOnClicked;
      
      
      

      // React to the user's selection.
      leftPane.selectionChanged += OnSpriteSelectionChange;
      

      // Restore the selection index from before the hot reload.
      leftPane.selectedIndex = m_SelectedIndex;

      // Store the selection index when the selection changes.
      leftPane.selectionChanged += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
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
    
    

    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
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
              
              
              
              var Aname = new TextField();
              Aname.RegisterValueChangedCallback(evt =>
              {
                  newAssetName = Aname.value;
              });
              Aname.value = sc.name;
              
              
              var name = new TextField();
              name.RegisterValueChangedCallback(evt =>
              {
                  sc.FriendlyName = name.value;
                  EditorUtility.SetDirty(sc);
              });
              name.value = sc.FriendlyName;
              
              
              var description = new TextField();
              description.RegisterValueChangedCallback(evt =>
              {
                  sc.Description = description.value;
                  EditorUtility.SetDirty(sc);
              });
              description.value = sc.Description;
              
              var sellprice = new TextField();
              sellprice.RegisterValueChangedCallback(evt =>
              {
                  sc.SellPrice = int.Parse(sellprice.value);
                  EditorUtility.SetDirty(sc);
              });
              sellprice.value = sc.SellPrice.ToString();
              
              var icon = new ObjectField();
              icon.RegisterValueChangedCallback(evt =>
              {
                  sc.Icon = icon.value as Sprite;
                  EditorUtility.SetDirty(sc);
              });
              icon.objectType = typeof(Sprite);
              icon.value = sc.Icon;

              // Add the Image control to the right-hand pane.
              m_RightPane.Add(Aname);
              m_RightPane.Add(name);
              m_RightPane.Add(description);
              m_RightPane.Add(sellprice);
              m_RightPane.Add(icon);
              
          }
      }
  }

    private void CreateItem()
    {
        if (newname != null || newname != lastname)
        {
            ItemDefinition item = ScriptableObject.CreateInstance<ItemDefinition>();
            AssetDatabase.CreateAsset(item, $"Assets/Scripts/SO/{newname}.asset");
        
            var allItems = new List<ScriptableObject>();
            allItems = FindScriptable();
        
            leftPane.makeItem = () => new Label();
            leftPane.bindItem = (item, index) => { (item as Label).text = allItems[index].name; };
            leftPane.itemsSource = allItems;
            lastname = newname;
            /*
            leftPane.IndexOf(item)

            int Itemindex;
            foreach (var I in allItems)
            {
                if (I.name == newname)
                {
                    Itemindex=I.
                }
            }
            
            leftPane.SetSelection();*/
        }
        
    }
}
