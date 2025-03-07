using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class CraftingOutputSlot : InventorySlot
{
    public Recipe recipe;
    public CraftingOutputSlot(Inventory inventory, int X, int Y) : base(inventory, true, X, Y)
    {
    }
    public void SetRecipe(Recipe r)
    {
        item = r.output;
        recipe = r;
    }
    public void ClearRecipe()
    {
        item = null;
        recipe = null;
    }

}
public class CraftingArea : StackInventory
{
    private List<Recipe> possibleRecipes;
    private CraftingOutputSlot[] outputSlots;
    Recipes recipes;
    public CraftingArea(bool[,] shape) : base(shape, ItemHelper.AllItemSet(), "Crafting")
    {
        recipes = new Recipes();
        outputSlots = new CraftingOutputSlot[3] { new CraftingOutputSlot(this, shape.GetLength(0) + 1, 0), new CraftingOutputSlot(this, shape.GetLength(0) + 1, 1), new CraftingOutputSlot(this, shape.GetLength(0) + 1, 2) };
    }
    public CraftingArea(List<PersistentSlot> slots, int width, int height) : base(slots, width, height, "Crafting", ItemHelper.AllItemSet())
    {
        recipes = new Recipes();
        outputSlots = new CraftingOutputSlot[3] { new CraftingOutputSlot(this, width + 1, 0), new CraftingOutputSlot(this, width + 1, 1), new CraftingOutputSlot(this, width + 1, 2) };
    }
    private void RefreshRecipes()
    {
        possibleRecipes = recipes.EvaluateCraftingArea(slots);
    }
    public override void ClickAt(ref StackItem heldItem, ref InventorySlot hoveredSlot)
    {
        if(!(hoveredSlot is CraftingOutputSlot))
        {
            base.ClickAt(ref heldItem, ref hoveredSlot);
        }
        RefreshRecipes();
        if (outputSlots.Contains<InventorySlot>(hoveredSlot))
        {
            //double check that the hoveredslot is a crafting output slot, and that heldItem is empty
            if (hoveredSlot is CraftingOutputSlot && heldItem == null)
            {

                //and if it is, craft that recipe
                CraftItem((CraftingOutputSlot)hoveredSlot, ref heldItem);
                return;
            }
        }
        UpdateOutputSlots();
    }
    private void UpdateOutputSlots()
    {
        RefreshRecipes();
        int outputIndex = 0;
        foreach (CraftingOutputSlot outSlot in outputSlots)
        {
            outSlot.ClearRecipe();
        }
        foreach (Recipe r in possibleRecipes)
        {
            outputSlots[outputIndex].item = r.output;
            outputSlots[outputIndex].SetRecipe(r);
            outputIndex++;
            if (outputIndex > outputSlots.Length)
            {
                break;
            }
        }
    }
    private void CraftItem(CraftingOutputSlot outputSlot, ref StackItem heldItem)
    {
        if (outputSlot.recipe == null)
        {
            return;
        }
        List<Items> itemIngredients = outputSlot.recipe.ingredients;
        foreach(Items ingredient in itemIngredients)
        {
            bool hadItem = false;
            foreach(InventorySlot slot in slots)
            {
                if (slot.IsEmpty() || !slot.IsValid())
                {
                    continue;
                }
                if (slot.item.GetItemType() == ingredient)
                {
                    slot.Remove();
                    hadItem = true;
                    break;
                }
            }
            if (!hadItem)
            {
                //TODO cancel craft properly (return partially used ingredients)
                return;
            }
        }
        heldItem = StackItem.CopyStackItem(outputSlot.recipe.output);
    }
    protected override VisualElement ConstructAsTab(out VisualElement inventoryGrid, int tabOffset, InventoryManager invenManager, int tabIndex, bool isSelectedTab)
    {
        VisualElement newTab = new VisualElement();
        newTab.pickingMode = PickingMode.Ignore;
        newTab.AddToClassList("crafting-tab");

        VisualElement inventoryHolder = new VisualElement();
        inventoryHolder.AddToClassList("tab-inventory-holder");
        newTab.Add(inventoryHolder);


        Label inventoryNameLabel = new Label(inventoryName);
        inventoryNameLabel.AddToClassList("inventory-title");

        VisualElement inventoryHeader = new VisualElement();
        inventoryHeader.AddToClassList("header");
        inventoryHeader.Add(inventoryNameLabel);

        inventoryHolder.Add(inventoryHeader);

        inventoryGrid = new VisualElement();
        inventoryGrid.AddToClassList("inventory-grid");
        inventoryHolder.Add(inventoryGrid);

        return newTab;
    }
    public override void PopulateGrid(VisualElement inventoryContainer, InventoryManager invenManager, int tabOffset = 0, int tabIndex = 0, bool isSelectedTab = false)
    {
        UpdateOutputSlots();
        VisualElement inventoryGrid;
        inventoryContainer.Add(ConstructAsTab(out inventoryGrid, tabOffset, invenManager, tabIndex, isSelectedTab));


        for (int i = 0; i < GetSlots().GetLength(0); i++)
        {
            //Create a new row
            VisualElement newRow = new VisualElement();
            newRow.AddToClassList("item-row");
            inventoryGrid.Add(newRow);
            //Populate it with slots
            for (int j = 0; j < GetSlots().GetLength(1); j++)
            {
                InventorySlot currentSlot = GetSlots()[i, j];
                //Create a new slot (or placeholder if no slot is present)
                if (currentSlot.IsValid())
                {
                    Button newSlot = new Button();
                    newSlot.clicked += (() => ClickAtSlot(invenManager, currentSlot, newSlot));
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
        //create a row for the output slots
        VisualElement outputRow = new VisualElement();
        outputRow.AddToClassList("item-row");
        inventoryGrid.Add(outputRow);
        for (int i = 0; i < outputSlots.Length; i++)
        {
            CraftingOutputSlot currentSlot = outputSlots[i];
            //Create a new slot (or placeholder if no slot is present)
            if (currentSlot.IsValid())
            {
                Button newSlot = new Button();
                newSlot.clicked += (() => ClickAtSlot(invenManager, currentSlot, newSlot));
                newSlot.AddToClassList("item-slot");

                if (!currentSlot.IsEmpty())
                {
                    currentSlot.item.PopulateSlot(newSlot);
                }
                outputRow.Add(newSlot);
            }
            else
            {
                VisualElement newSlot = new VisualElement();
                newSlot.AddToClassList("item-slot-placeholder");
                outputRow.Add(newSlot);
            }
        }
    }


}
public class Recipe
{
    public List<Items> ingredients;
    public StackItem output;
    public Recipe(StackItem POutput, List<Items> PIngredients)
    {
        ingredients = PIngredients;
        output = POutput;
    }
}

public class Recipes
{
    public List<Recipe> recipes;
    public Recipes()
    {
        recipes = new List<Recipe>();
        //Add recipes here
        recipes.Add(new Recipe(new StackItem(new Rock(), 1), new List<Items> { Items.Pebble , Items.Pebble, Items.Pebble, Items.Pebble, Items.Pebble }));
        recipes.Add(new Recipe(new StackItem(new Pebble(), 5), new List<Items> { Items.Rock }));
        recipes.Add(new Recipe(new StackItem(new Club(), 1), new List<Items> { Items.Pebble, Items.Twigs, Items.Twigs }));
        recipes.Add(new Recipe(new StackItem(new Spear(), 1), new List<Items> { Items.Rock, Items.Twigs, Items.Twigs }));
        recipes.Add(new Recipe(new StackItem(new FireStarter(), 1), new List<Items> { Items.Rock, Items.Flint}));
        recipes.Add(new Recipe(new StackItem(new CampFire(), 1), new List<Items> { Items.FireStarter, Items.Twigs, Items.Twigs }));
    }
    public List<Recipe> EvaluateCraftingArea(InventorySlot[,] craftingAreaSlots)
    {
        List<Recipe> possibleRecipes = new List<Recipe>();
        List<Items> craftingIngredients = new List<Items>();
        foreach (InventorySlot slot in craftingAreaSlots)
        {
            if(slot.item == null)
            {
                continue;
            }
            for (int i =0;i < slot.item.GetCount(); i++)
            {
                craftingIngredients.Add(slot.item.GetItemType());
            }
        }
        List<Items> allIngredients = new List<Items>(craftingIngredients);
        foreach(Recipe r in recipes)
        {
            craftingIngredients = new List<Items>( allIngredients);
            bool validRecipe = true;
            foreach(Items ingredient in r.ingredients)
            {
                if (craftingIngredients.Contains(ingredient))
                {
                    craftingIngredients.Remove(ingredient);
                }
                else
                {
                    validRecipe = false;
                    break;
                }
            }
            if (validRecipe)
            {
                possibleRecipes.Add(r);
            }
        }
        return possibleRecipes;
    }
}
