using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SurfaceState surfaceState;
    [SerializeField] private DungeonState dungeonState;
    [SerializeField] private CombatState combatState;
    [SerializeField] private BaseState baseState;
    private Stack<IWorldState> worldState;
    private Stack<IUIState> uiState;
    public void Start()
    {
        worldState = new Stack<IWorldState>();
        uiState = new Stack<IUIState>();
        playerController.Initialize(this);
        baseState.Initialize(this);
        inventory.Initialize(this);
        surfaceState.Initialize(this);
        combatState.Initialize(this);
        dungeonState.Initialize(this);
        EnterState(baseState);
        EnterState(surfaceState);
    }
    private void FixedUpdate()
    {
        UpdateWorldState();
        UpdateUIState();
    }
    private void UpdateWorldState()
    {
        if (worldState == null)
        {
            return;
        }
        if (worldState.Count == 0)
        {
            return;
        }
        worldState.Peek().UpdateWhenOpen();
    }
    private void UpdateUIState()
    {
        if (uiState == null)
        {
            return;
        }
        if (uiState.Count == 0)
        {
            return;
        }
        uiState.Peek().UpdateWhenOpen();
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
                EnterState(surfaceState);
                break;
            case "Dungeon":
                EnterState(dungeonState);
                Debug.Log("DUNGEONS STATE ENTERED");
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
        if(state is IWorldState)
        {
            DisableTopWorldState();
            worldState.Push(state as IWorldState);
            EnableTopWorldState();
            return;
        }
        if(state is IUIState)
        {
            PauseTopWorldState();
            DisableTopUIState();
            uiState.Push(state as IUIState);
            EnableTopUIState();
        }

    }
    private void ExitState(IGameState state)
    {
        if (state is IWorldState)
        {
            if (worldState.Count <= 0)
            {
                return;
            }
            if (worldState.Peek() != state)
            {
                Debug.LogWarning("Attempted to exit a state that is not the current state");
                return;
            }
            DisableTopWorldState();
            worldState.Pop();
            EnableTopWorldState();
        }
        if(state is IUIState)
        {
            if (uiState.Count <= 0)
            {
                return;
            }
            if (uiState.Peek() != state)
            {
                Debug.LogWarning("Attempted to exit a state that is not the current state");
                return;
            }
            DisableTopUIState();
            uiState.Pop();
            EnableTopUIState();
            if(uiState.Count == 0)
            {
                PlayTopWorldState();
            }
        }
    }
    private void EnableTopWorldState()
    {
        if (worldState.Count <= 0)
        {
            return;
        }
        IWorldState topState = worldState.Peek();
        topState.Open();
    }
    private void DisableTopWorldState()
    {
        if(worldState.Count <= 0)
        {
            return;
        }
        IWorldState topState = worldState.Peek();
        topState.Close();
    }
    private void EnableTopUIState()
    {
        if (uiState.Count <= 0)
        {
            return;
        }
        IUIState topState = uiState.Peek();
        topState.Open();
    }
    private void DisableTopUIState()
    {
        if (uiState.Count <= 0)
        {
            return;
        }
        IUIState topState = uiState.Peek();
        topState.Close();
        if(uiState.Count == 0)
        {
            PlayTopWorldState();
        }
    }
    private void PauseTopWorldState()
    {
        worldState.Peek().Pause();
    }
    private void PlayTopWorldState()
    {
        worldState.Peek().Play();
    }
    public void InputReceived(int input)
    {
        worldState.Peek().TakeInput(input);
    }
}