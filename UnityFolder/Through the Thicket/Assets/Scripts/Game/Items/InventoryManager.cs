using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour, IUIState
{
    private VisualElement inventoryGrid;
    private UIDocument inventoryUI;
    private StackItem heldItem;
    private VisualElement heldItemVisual;
    private List<Inventory> inventories;
    public void Initialize(GameManager manager)
    {
        inventoryUI = GetComponent<UIDocument>();
        VisualElement root= inventoryUI.rootVisualElement;

        inventoryGrid = root.Q<VisualElement>("ItemGrid");
        Button myButton = root.Q<Button>("CloseButton");
        myButton.clicked += () => manager.CloseState(this);

        bool[,] shape = new bool[5, 5]
        {
            {true, true, true, true, true },
            {true, true, true, false, true},
            {true, false, false, true, true },
            {true, false, true, true, true },
            {true, false, false, false, true }
        };
        //testing setup for stack inventory testing
        Inventory inventory = new TestInventory(shape, 0, 0);
        //creates the held item, and places it into the inventory
        heldItem = new StackItem(new Stone());
        InventorySlot tempSlot = inventory.GetSlot(0, 1);
        inventory.ClickAt(ref heldItem, ref tempSlot);
        heldItem = new StackItem(new Stone());
        tempSlot = inventory.GetSlot(0, 2);
        inventory.ClickAt(ref heldItem, ref tempSlot);

        shape = new bool[3, 3]
{
            {true, true, true },
            {true, true, true },
            {true, false, false  }
};
        Inventory craftingArea = new CraftingArea(shape, 50, 50);
        inventories = new List<Inventory>();
        inventories.Add(inventory);
        inventories.Add(craftingArea);
        PopulateAllGrids();

        //creates a UIElement to display the held item
        heldItem = null;
        heldItemVisual = new VisualElement();
        heldItemVisual.AddToClassList("item-image");
        heldItemVisual.AddToClassList("held-item");
        //disable interaction to allow clicking on things in the inventory even when an item is held
        heldItemVisual.pickingMode = PickingMode.Ignore;
        root.Add(heldItemVisual);
        heldItemVisual.parent.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        UpdateHeldItem();

        Close();
    }
    public void PopulateAllGrids()
    {
        inventoryGrid.Clear();
        foreach (Inventory i in inventories)
        {
            i.PopulateGrid(inventoryGrid, this);
        }
    }
    public StackItem GetHeldItem()
    {
        return heldItem;
    }
    public void SetHeldItem(StackItem heldItem)
    {
        this.heldItem = heldItem;
    }
    public VisualElement GetGrid()
    {
        return inventoryGrid;
    }

    public void Close()
    {
        inventoryUI.rootVisualElement.style.display = DisplayStyle.None;
    }
    public void Open()
    {
        inventoryUI.rootVisualElement.style.display = DisplayStyle.Flex;
        Button myButton = inventoryUI.rootVisualElement.Q<Button>("CloseButton");
        myButton.Focus(); // Ensure the button is focused so it can immediately register clicks
        myButton.SetEnabled(true);
    }
    public void UpdateWhenOpen()
    {
        
    }
    public void UpdateHeldItem()
    {
        heldItemVisual.Clear();
        if (heldItem == null)
        {
            return;
        }
        heldItem.PopulateSlot(heldItemVisual);
    }
    private void OnPointerMove(PointerMoveEvent evt)
    {

        // Get mouse position relative to the UI element (not screen space)
        Vector2 localMousePos = evt.localPosition;

        // Get the element size (for centering)
        float width = heldItemVisual.resolvedStyle.width;
        float height = heldItemVisual.resolvedStyle.height;

        // Apply the position to the element (centering it on the cursor)
        heldItemVisual.style.left = localMousePos.x - (width / 2);
        heldItemVisual.style.top = localMousePos.y + (height / 2) - heldItemVisual.parent.resolvedStyle.height;
    }

}