using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public enum GameStates
{
    Base,
    Inventory,
    Travelling,
    Combat
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private PlayerController player;
    private Stack<GameStates> gameState = new Stack<GameStates>(); // Inline initialization
    public void Start()
    {
        inventory.Initialize(this);
        player.Initialize(this);
        EnterState(GameStates.Base);
        EnterState(GameStates.Travelling);
        OpenInventory();
    }
    public void CloseInventory()
    {
        ExitState(GameStates.Inventory);
    }
    public void OpenInventory()
    {
        EnterState(GameStates.Inventory);
    }
    private void EnterState(GameStates state)
    {
        DisableTopState();
        gameState.Push(state);
        EnableTopState();

    }
    private void ExitState(GameStates state)
    {
        if (gameState.Count <= 0)
        {
            return;
        }
        if (gameState.Peek() != state)
        {
            Debug.LogWarning("Attempted to exit a state that is not the current state");
            return;
        }
        DisableTopState();
        gameState.Pop();
        EnableTopState();

    }
    private void EnableTopState()
    {
        if (gameState.Count <= 0)
        {
            return;
        }
        GameStates topState = gameState.Peek();
        switch (topState)
        {
            case GameStates.Base:
                Debug.Log("Opening game");
                break;
            case GameStates.Travelling:
                Debug.Log("Travelling state opened");
                player.SetIsTakingInput(true);
                break;
            case GameStates.Combat:
                Debug.Log("Combat state opened");
                break;
            case GameStates.Inventory:
                Debug.Log("Inventory state opened");
                inventory.Show();
                break;
        }
    }
    private void DisableTopState()
    {
        if(gameState.Count <= 0)
        {
            return;
        }
        GameStates topState = gameState.Peek();
        switch (topState)
        {
            case GameStates.Base:
                Debug.Log("Closing game");
                break;
            case GameStates.Travelling:
                Debug.Log("Travelling state closed");
                player.SetIsTakingInput(false);
                break;
            case GameStates.Combat:
                Debug.Log("Combat state closed");
                break;
            case GameStates.Inventory:
                Debug.Log("Inventory state closed");
                inventory.Hide();
                break;
        }
    }
}