using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class TileInteraction
{
    public abstract void Execute(GameManager gameManager);
}
public class TileDestruction : TileInteraction
{
    private GameObject tile;
    private Layers layer;
    public TileDestruction(GameObject tile, Layers layer)
    {
        this.tile = tile;
        this.layer = layer;
    }
    public override void Execute(GameManager gameManager)
    {
        switch (layer)
        {
            case Layers.Object:
                gameManager.GivePlayerItem(ItemHelper.GetItemFromType((tile.GetComponent<TileDataHolder>().thisTileData.ObjectType as ICollectable).GetItemType()));
                gameManager.SetTile(tile.GetComponent<TileDataHolder>().thisTileData.X, tile.GetComponent<TileDataHolder>().thisTileData.Y, Layers.Object, new EmptyObject());
                break;
            case Layers.Foliage:
                gameManager.GivePlayerItem(ItemHelper.GetItemFromType((tile.GetComponent<TileDataHolder>().thisTileData.FoliageType as ICollectable).GetItemType()));
                gameManager.SetTile(tile.GetComponent<TileDataHolder>().thisTileData.X, tile.GetComponent<TileDataHolder>().thisTileData.Y, Layers.Foliage, new EmptyFoliage());
                break;
            case Layers.Base:
                Debug.Log("Dont mine the floor please :)");
                break;
        }
    }
}
public class TilePlacement : TileInteraction
{
    private GameObject tile;
    private Layers layer;
    private IPlacable placer;
    public TilePlacement(GameObject tileToPlaceAt, Layers layerToPlaceOn, IPlacable placeThis)
    {
        tile = tileToPlaceAt;
        layer = layerToPlaceOn;
        placer = placeThis;
    }
    public override void Execute(GameManager gameManager)
    {
        gameManager.SetTile(tile.GetComponent<TileDataHolder>().thisTileData.X, tile.GetComponent<TileDataHolder>().thisTileData.Y, layer, placer);
        gameManager.TakeFromPlacablesInventory(placer);
    }
}
public class TileInteractionExit : TileInteraction
{
    public TileInteractionExit()
    {

    }
    public override void Execute(GameManager gameManager)
    {
        //does nothing
    }
}
public class EnterDungeonOption : TileInteraction
{
    int id;
    static int nextDungeonID = 0;
    int X;
    int Y;
    public EnterDungeonOption(int dungeonID, ref DungeonEntrance entrance, int entranceX, int entranceY)
    {
        id = dungeonID;
        if(id == 0)
        {
            nextDungeonID++;   
            id = nextDungeonID;
            entrance.dungeonID = id;
            X = entranceX;
            Y = entranceY;
        }
    }
    public static void SetCurrentDungeonID(int id)
    {
        nextDungeonID = id;
    }
    public override void Execute(GameManager gameManager)
    {
        gameManager.SetTile(X, Y, Layers.Object, new EmptyObject()); // TODO change to some sort of a collapsed cave or smth
        gameManager.SetDungeonID(id);
        gameManager.OpenState("Dungeon");
        gameManager.SaveGlobalVariables(id);
    }
}
public class ExitDungeonOption : TileInteraction
{
    public ExitDungeonOption()
    {
    }
    public override void Execute(GameManager gameManager)
    {
        gameManager.CloseState("Dungeon");
    }
}
public class MyceliumBoostOption : TileInteraction
{
    int X;
    int Y;

    public MyceliumBoostOption(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override void Execute(GameManager gameManager)
    {
        gameManager.SetTile(X, Y, Layers.Foliage, new EmptyFoliage()); // removes the mycelium
        gameManager.GivePlayerMyceliumBoost();
    }
}
public class TileInteractionOption
{
    string displayText;
    TileInteraction interaction;
    public TileInteractionOption(string displayText, TileInteraction interaction)
    { 
        this.displayText = displayText; 
        this.interaction = interaction;
    }
    public string GetDisplay()
    {
        return displayText;
    }
    public TileInteraction GetInteraction()
    {
        return interaction;
    }

}
public class TileInteractionMenu
{
    List<TileInteractionOption> options;
    public TileInteractionMenu()
    {
        options = new List<TileInteractionOption>();
    }
    public void AddOptions(List<TileInteractionOption> newOptions)
    {
        options.AddRange(newOptions);
    }
    public void AddOption(TileInteractionOption newOption)
    {
        options.Add(newOption);
    }
    public List<TileInteractionOption> GetOptions()
    {
        return options;
    }
}
