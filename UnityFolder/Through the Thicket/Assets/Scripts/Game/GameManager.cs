using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private PlayerController exploringState;
    [SerializeField] private CombatState combatState;
    private BaseState baseState;
    private Stack<IGameState> gameState;
    public void Start()
    {
        gameState = new Stack<IGameState>();
        baseState = new BaseState();
        baseState.Initialize(this);
        inventory.Initialize(this);
        exploringState.Initialize(this);
        combatState.Initialize(this);
        EnterState(baseState);
        EnterState(exploringState);
    }
    private void FixedUpdate()
    {
        if(gameState == null)
        {
            return;
        }
        if (gameState.Count == 0)
        {
            return;
        }
        gameState.Peek().UpdateWhenOpen();
    }
    public void OpenState(string stateName)
    {
        switch (stateName)
        {
            case "Base":
                Debug.Log("Starting Game");
                EnterState(baseState);
                break;
            case "Exploring":
                EnterState(exploringState);
                break;
            case "Inventory":
                EnterState(inventory);
                break;
            case "Combat":
                EnterState(combatState);
                break;
            default:
                throw new System.Exception("Invalid State Open Requested");
        }
    }
    public void CloseState(IGameState state)
    {
        ExitState(state);
    }
    private void EnterState(IGameState state)
    {
        DisableTopState();
        gameState.Push(state);
        EnableTopState();

    }
    private void ExitState(IGameState state)
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
        IGameState topState = gameState.Peek();
        topState.Open();
    }
    private void DisableTopState()
    {
        if(gameState.Count <= 0)
        {
            return;
        }
        IGameState topState = gameState.Peek();
        topState.Close();
    }
}