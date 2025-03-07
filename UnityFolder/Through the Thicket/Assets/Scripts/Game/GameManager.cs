using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField] private TileInteractionState tileState;
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
        tileState.Initialize(this);
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
    public void CloseState(string stateName)
    {
        switch (stateName)
        {
            case "Base":
                ExitState(baseState);
                break;
            case "Exploring":
                ExitState(surfaceState);
                break;
            case "Dungeon":
                ExitState(dungeonState);
                break;
            case "Inventory":
                ExitState(inventory);
                break;
            case "Combat":
                ExitState(combatState);
                break;
            case "Tile":
                ExitState(tileState);
                break;
            default:
                throw new System.Exception("Invalid State Open Requested");
        }
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
    public void InputReceived(string inputString)
    {
        Inputs input;
        switch (inputString)
        {
            case "UIToggle":
                input = Inputs.UIToggle;
                break;
            case "UIClose":
                input = Inputs.UIClose;
                break;
            case "Debug1":
                input = Inputs.Debug1;
                break;
            case "Debug2":
                input = Inputs.Debug2;
                break;
            case "LClick":
                input = Inputs.LClick;
                break;
            case "RClick":
                input = Inputs.RClick;
                break;
            default:
                input = 0;
                Debug.Log("Error finding specified Input");
                break;
        }
        if (uiState.TryPeek(out _))
        {
            uiState.Peek().TakeInput(input);
            return;
        }
        worldState.Peek().TakeInput(input);
    }

    public void EnterTileInteractionMode(TileInteractionMenu tileInteractionOptions, GameObject selectedObject)
    {
        tileInteractionOptions.AddOptions(inventory.GetInteractionOptions(selectedObject));
        EnterState(tileState);
        tileState.PopulateTileInteractionMenu(tileInteractionOptions, selectedObject);
    }
    public void ExitTileInteractionMode(TileInteraction interaction)
    {
        interaction.Execute(this);
        ExitState(tileState);
    }
    public void GivePlayerItem(IItem item)
    {
        if(!inventory.StuffIntoMainInventory(new StackItem(item)))
        {
            Debug.Log("Item didn't fit in inventory! UH OH");
        }
    }

    public void SetTile(int x, int y, Layers layer, ITileSegment segment)
    {
        worldState.Peek().SetTile(x, y, layer, segment);
    }

    public void TakeFromPlacablesInventory(IPlacable placer)
    {
        inventory.TakeFromPlacablesInventory(placer);
    }
}