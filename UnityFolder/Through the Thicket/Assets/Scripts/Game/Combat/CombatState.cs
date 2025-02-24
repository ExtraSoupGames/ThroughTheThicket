using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatState : MonoBehaviour, IUIState
{
    private UIDocument combatUI;
    public void Initialize(GameManager manager)
    {
        combatUI = GetComponent<UIDocument>();
        VisualElement root = combatUI.rootVisualElement;
        Button myButton = root.Q<Button>();
        myButton.clicked += () => manager.CloseState(this);
        Close();
    }
    public void Open()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void Close()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.None;
    }
    public void UpdateWhenOpen()
    {

    }
}
