using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InventoryManager : IUIState
{
    private VisualElement inventoryGrid;
    private VisualElement inventoryContainer;
    private StackItem heldItem;
    private VisualElement heldItemVisual;
    private Inventory mainInventory;
    private Inventory craftingInventory;
    private List<Inventory> subInventories;
    private int selectedInventoryTab;
    [SerializeField] private UIDocument inventoryUI;
    public override void Initialize(GameManager manager)
    {
        VisualElement root = inventoryUI.rootVisualElement;

        inventoryContainer = root.Q<VisualElement>("InventoryContainer");
        inventoryGrid = root.Q<VisualElement>("ItemGrid");
        Button myButton = root.Q<Button>("CloseButton");
        myButton.clicked += () => manager.CloseState("Inventory");

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
        selectedInventoryTab = 0;

        shape = new bool[3, 3]
{
            {true, true, true },
            {true, true, true },
            {true, false, false  }
};
        craftingInventory = new CraftingArea(shape, 0, 0);
        subInventories = new List<Inventory>();
        mainInventory = inventory;

        shape = new bool[4, 4]
        {
            {true, true, true, true},
            {true, false, false, true},
            {true, false, false, true},
            {true, false, false, true }
        };
        Inventory tabbedInventory = new TestInventory(shape, 0, 0);
        subInventories.Add(tabbedInventory);

        shape = new bool[4, 4]
{
            {true, true, true, true},
            {true, false, false, true},
            {true, false, false, true},
            {true, true, true, true }
};
        Inventory tabbedInventory2 = new TestInventory(shape, 0, 0);
        subInventories.Add(tabbedInventory2);
        RefreshInventory();

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
    public void RefreshInventory()
    {
        inventoryGrid.Clear();
        mainInventory.PopulateGrid(inventoryContainer, this);
        inventoryContainer.Remove(inventoryContainer.Q<VisualElement>(className: "crafting-tab"));
        craftingInventory.PopulateGrid(inventoryContainer, this);
        while (inventoryContainer.Q<VisualElement>(className: "inventory-tab") != null)
        {
            inventoryContainer.Remove(inventoryContainer.Q<VisualElement>(className: "inventory-tab"));
        }
        int tabOffset = 5;
        int tabIndex = 0;
        foreach (Inventory i in subInventories)
        {
            i.PopulateGrid(inventoryContainer, this, tabOffset, tabIndex, tabIndex == selectedInventoryTab);
            tabOffset += 10;
            tabIndex++;
        }
        Debug.Log("Selected tab is: " + selectedInventoryTab);
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

    public override void Close()
    {
        inventoryUI.rootVisualElement.style.display = DisplayStyle.None;
        base.Close();
    }
    public override void Open()
    {
        inventoryUI.rootVisualElement.style.display = DisplayStyle.Flex;
        Button myButton = inventoryUI.rootVisualElement.Q<Button>("CloseButton");
        myButton.Focus(); // Ensure the button is focused so it can immediately register clicks
        myButton.SetEnabled(true);
        base.Open();
    }
    public override void UpdateWhenOpen()
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
    public void SelectTab(int tabIndex)
    {
        selectedInventoryTab = tabIndex;
        RefreshInventory();
    }

}