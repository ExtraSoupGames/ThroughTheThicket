using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    private VisualElement inventoryGrid;
    private UIDocument inventoryUI;
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
            {true, false, true, false, true},
            {true, false, false, true, true },
            {true, false, true, true, true },
            {true, false, false, false, true }
        };
        Inventory inventory = new TestInventory(shape, 0, 0);
        Item heldItem = new StackItem(Items.Stone);
        inventory.ClickAt(ref heldItem, inventory.GetSlot(0, 1));
        PopulateInventory(inventory);
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
                //Create a new slot (or placeholder if no slot is present)
                //TODO store inventory.GetSlots() and maybe do switch case on emptiness and validity
                VisualElement newSlot = new VisualElement();
                if (inventory.GetSlots()[i, j].IsValid())
                {
                    newSlot.AddToClassList("item-slot");
                    if (!inventory.GetSlots()[i, j].IsEmpty())
                    {
                        VisualElement slotItem = new VisualElement();
                        slotItem.AddToClassList("item-image");
                        Debug.Log(inventory.GetSlots()[i, j].item.GetSprite().texture);
                        slotItem.style.backgroundImage = inventory.GetSlots()[i, j].item.GetSprite().texture;
                        newSlot.Add(slotItem);
                    }
                }
                else
                {
                    newSlot.AddToClassList("item-slot-placeholder");
                }
                newRow.Add(newSlot);
            }
        }
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
}