using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class TileInteractionState : IUIState
{
    [SerializeField] private UIDocument tileSelectorUI;
    private GameManager gameManager;
    int menuSize = 200;
    float menuRadius = 80f;
    public override void Initialize(GameManager manager)
    {
        gameManager = manager;
        Close();
    }

    public override void UpdateWhenOpen()
    {
    }
    public void PopulateTileInteractionMenu(TileInteractionMenu menu, GameObject tile)
    {
        VisualElement root = tileSelectorUI.rootVisualElement;
        root.Clear();
        VisualElement radialMenu = new VisualElement();
        radialMenu.AddToClassList("radial-menu");
        VisualElement centreDot = new VisualElement();
        centreDot.AddToClassList("centre-dot");
        radialMenu.Add(centreDot);
        List<TileInteractionOption> options = menu.GetOptions();
        int totalOptions = options.Count;
        for(int i = 0; i < totalOptions; i++)
        {
            TileInteractionOption option = options[i];
            Button optionButton = new Button();
            optionButton.AddToClassList("radial-button");
            optionButton.clicked += () => ExitMenu(option);
            optionButton.text = option.GetDisplay();

            float angle = (i * 360f) / totalOptions;
            Vector2 buttonPos = GetCircularPosition(angle, menuRadius);

            // Adjust for button size (assuming 40x40 buttons, adjust as needed)
            float buttonSize = 40f;
            optionButton.style.left = buttonPos.x - buttonSize / 2;
            optionButton.style.top = buttonPos.y - buttonSize / 2;

            radialMenu.Add(optionButton);
        }
        root.Add(radialMenu);
        MoveMenuToGameObject(tile, radialMenu);
        
    }
    Vector2 GetCircularPosition(float angle, float radius)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Cos(radian) * radius + menuSize / 2 - 20,
            Mathf.Sin(radian) * radius + menuSize / 2 - 20
        );
    }

    public void MoveMenu(Vector2 screenPosition, VisualElement menu)
    {
        menu.style.left = screenPosition.x - menuSize / 2;
        menu.style.top = screenPosition.y - menuSize / 2;
    }

    public void MoveMenuToGameObject(GameObject target, VisualElement menu)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        screenPos.y = Screen.height - screenPos.y;
        Vector2 uiPos = tileSelectorUI.rootVisualElement.WorldToLocal(screenPos);
        MoveMenu(screenPos, menu);
    }
    public void ExitMenu(TileInteractionOption option = null)
    {
        gameManager.ExitTileInteractionMode(option.GetInteraction());
    }
    public override void Open()
    {
        tileSelectorUI.rootVisualElement.style.display = DisplayStyle.Flex;
        base.Open();
    }
    public override void Close()
    {
        tileSelectorUI.rootVisualElement.style.display = DisplayStyle.None;
        base.Close();
    }

    public override void TakeInput(Inputs input)
    {
        if(input == Inputs.UIToggle || input == Inputs.UIClose)
        {
            gameManager.ExitTileInteractionMode(new TileInteractionExit());
        }
    }
}
