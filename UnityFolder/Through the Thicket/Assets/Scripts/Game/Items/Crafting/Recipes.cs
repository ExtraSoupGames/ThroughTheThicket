using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public CraftingArea(bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY, new HashSet<Items> {Items.Stone, Items.MegaStoneTest })
    {
        recipes = new Recipes();
        outputSlots = new CraftingOutputSlot[3] { new CraftingOutputSlot(this, shape.GetLength(0) + 1, 0), new CraftingOutputSlot(this, shape.GetLength(0) + 1, 1), new CraftingOutputSlot(this, shape.GetLength(0) + 1, 2) };
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
            Debug.Log("Certainly looks like we might be about to craft");
            //double check that the hoveredslot is a crafting output slot, and that heldItem is empty
            if (hoveredSlot is CraftingOutputSlot && heldItem == null)
            {
                Debug.Log("Certainly rilly rilly looks like we might be about to craft");

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
            Debug.Log("Valid recipe found");
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
        Debug.Log("Crafting item...");
        List<Items> itemIngredients = outputSlot.recipe.ingredients;
        foreach(Items ingredient in itemIngredients)
        {
            Debug.Log("foreach ingredient loop ");
            bool hadItem = false;
            foreach(InventorySlot slot in slots)
            {
                Debug.Log("foreach slot foreach ingreadient loop ");
                if (slot.IsEmpty())
                {
                    continue;
                }
                if (slot.item.GetItemType() == ingredient)
                {
                    Debug.Log("Removing 1 ingredient of type: " + ingredient);
                    slot.Remove();
                    hadItem = true;
                    break;
                }
            }
            if (!hadItem)
            {
                //TODO cancel craft properly (return partially used ingredients
                return;
            }
        }
        heldItem = new StackItem(outputSlot.recipe.output);
    }
    public override void PopulateGrid(VisualElement inventoryGrid, InventoryManager invenManager)
    {
        UpdateOutputSlots();
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
        recipes.Add(new Recipe(new StackItem(new Stone(), 3), new List<Items> { Items.Stone , Items.Stone}));
        recipes.Add(new Recipe(new StackItem(new MegaStoneTesting(), 1), new List<Items> { Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone, Items.Stone }));
    }
    public List<Recipe> EvaluateCraftingArea(InventorySlot[,] craftingAreaSlots)
    {
        Debug.Log("Evaluating recipes");
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
        foreach(Recipe r in recipes)
        {
            bool validRecipe = true;
            foreach(Items ingredient in r.ingredients)
            {
                if (craftingIngredients.Contains(ingredient))
                {
                    Debug.Log("Crafting recipe had 1 ingredient valid");
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
