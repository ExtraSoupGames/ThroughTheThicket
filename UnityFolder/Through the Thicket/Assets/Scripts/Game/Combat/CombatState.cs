using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatState : IUIState
{
    [SerializeField] private UIDocument combatUI;
    public override void Initialize(GameManager manager)
    {
        VisualElement root = combatUI.rootVisualElement;
        Button myButton = root.Q<Button>();
        myButton.clicked += () => manager.CloseState(this);
        Close();
    }
    public override void Open()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.Flex;
        base.Open();
    }
    public override void Close()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.None;
        base.Close();
    }
    public override void UpdateWhenOpen()
    {

    }
}
