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
    private string SetName;
    private string SetDescription;
    private int SetSellPrice;
    private Sprite SetSetIcon;

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

    public void CreateGUI()
  {
      // Get a list of all sprites in the project.
      var allItemsGuids = AssetDatabase.FindAssets("t:ItemDefinition");
      var allItems = new List<ScriptableObject>();
      
      foreach (var guid in allItemsGuids)
      {
          allItems.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)));
      }
      
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
      var leftPane = new ListView();
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
      
      button.clicked += ButtonOnclicked;
      
      
      

      // React to the user's selection.
      leftPane.selectionChanged += OnSpriteSelectionChange;
      

      // Restore the selection index from before the hot reload.
      leftPane.selectedIndex = m_SelectedIndex;

      // Store the selection index when the selection changes.
      leftPane.selectionChanged += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
  }

    private void ButtonOnclicked()
    {
        Debug.Log("klick");
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
              lastItem = sc;
              // Add a new Image control and display the sprite.
              var name = new TextField();
              name.RegisterValueChangedCallback(evt =>
              {
                  sc.FriendlyName = name.value;
                  EditorUtility.SetDirty(sc);
              });
              name.value = sc.FriendlyName;
              SetName = name.value;
              
              var description = new TextField();
              description.RegisterValueChangedCallback(evt =>
              {
                  sc.Description = description.value;
                  EditorUtility.SetDirty(sc);
              });
              description.value = sc.Description;
              SetDescription = description.value;
              
              var sellprice = new TextField();
              sellprice.RegisterValueChangedCallback(evt =>
              {
                  sc.SellPrice = int.Parse(sellprice.value);
                  EditorUtility.SetDirty(sc);
              });
              sellprice.value = sc.SellPrice.ToString();
              SetSellPrice = int.Parse(sellprice.value);
              
              var icon = new ObjectField();
              icon.RegisterValueChangedCallback(evt =>
              {
                  sc.Icon = icon.value as Sprite;
                  EditorUtility.SetDirty(sc);
              });
              icon.objectType = typeof(Sprite);
              icon.value = sc.Icon;

              // Add the Image control to the right-hand pane.
              m_RightPane.Add(name);
              m_RightPane.Add(description);
              m_RightPane.Add(sellprice);
              m_RightPane.Add(icon);


              sc.FriendlyName = name.value;
              sc.Description = description.value;
              sc.SellPrice = int.Parse(sellprice.value);
              sc.Icon = icon.value as Sprite;
              
          }
          /*
          var selectedSprite = enumerator.Current as Texture2D;
          if (selectedSprite != null)
          {
              // Add a new Image control and display the sprite.
              var spriteImage = new Image();
              spriteImage.scaleMode = ScaleMode.ScaleToFit;
              spriteImage.image = selectedSprite;

              // Add the Image control to the right-hand pane.
              m_RightPane.Add(spriteImage);
          }*/
      }
  }
}
