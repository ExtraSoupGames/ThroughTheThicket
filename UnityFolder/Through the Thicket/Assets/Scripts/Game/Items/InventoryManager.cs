using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    private VisualElement inventoryGrid;
    private UIDocument inventoryUI;
    private Item heldItem;
    private VisualElement heldItemVisual;
    public void Initialize(GameManager manager)
    {
        inventoryUI = GetComponent<UIDocument>();
        VisualElement root= inventoryUI.rootVisualElement;

        inventoryGrid = root.Q<VisualElement>("ItemGrid");
        Button myButton = root.Q<Button>("CloseButton");
        myButton.clicked += () => manager.CloseInventory();

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
        heldItem = new StackItem(Items.Stone);
        InventorySlot tempSlot = inventory.GetSlot(0, 1);
        inventory.ClickAt(ref heldItem, ref tempSlot);
 
        PopulateInventory(inventory);
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
        Hide();
    }

    void PopulateInventory(Inventory inventory)
    {
        inventoryGrid.Clear();
        for (int i = 0; i < inventory.GetSlots().GetLength(0); i++)
        {
            //Create a new row
            VisualElement newRow = new VisualElement();
            newRow.AddToClassList("item-row");
            inventoryGrid.Add(newRow);
            //Populate it with slots
            for (int j = 0; j < inventory.GetSlots().GetLength(1); j++)
            {
                InventorySlot currentSlot = inventory.GetSlots()[i, j];
                //Create a new slot (or placeholder if no slot is present)
                if (currentSlot.IsValid())
                {
                    Button newSlot = new Button();
                    newSlot.clicked += (() => ClickAtSlot(inventory, currentSlot, newSlot));
                    newSlot.AddToClassList("item-slot");

                    if (!currentSlot.IsEmpty())
                    {
                        currentSlot.item.PopulateSlot(newSlot);
                    }
                    newRow.Add(newSlot);
                }
                else
                {
                    VisualElement newSlot = new VisualElement();
                    newSlot.AddToClassList("item-slot-placeholder");
                    newRow.Add(newSlot);
                }
            }
        }
    }
    private void ClickAtSlot(Inventory inven, InventorySlot slot, VisualElement slotVisual)
    {
        inven.ClickAt(ref heldItem, ref slot);
        //update the whole inventory visual
        PopulateInventory(inven);
        UpdateHeldItem();
    }
    public void Hide()
    {
        inventoryUI.rootVisualElement.style.display = DisplayStyle.None;
    }
    public void Show()
    {
        inventoryUI.rootVisualElement.style.display = DisplayStyle.Flex;
        Button myButton = inventoryUI.rootVisualElement.Q<Button>("CloseButton");
        myButton.Focus(); // Ensure the button is focused so it can immediately register clicks
        myButton.SetEnabled(true);
    }
    private void UpdateHeldItem()
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