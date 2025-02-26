using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatState : IUIState
{
    [SerializeField] private UIDocument combatUI;
    private Enemy currentEnemy;
    public override void Initialize(GameManager manager)
    {
        VisualElement root = combatUI.rootVisualElement;
        root.Clear();
        //Button closeButton = new Button();
        //root.Add(closeButton);
        //closeButton.clicked += () => manager.CloseState("Combat");
        Close();

        VisualElement combatContainer = new VisualElement();
        combatContainer.AddToClassList("combat-container");
        root.Add(combatContainer);
    }
    public override void Open()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.Flex;
        base.Open();
        //For testing TODO remove
        currentEnemy = new Squirrel();
        PopulateUI();
    }
    public override void Close()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.None;
        base.Close();
    }
    public override void UpdateWhenOpen()
    {

    }
    public void SetEnemy(Enemy e)
    {
        currentEnemy = e;
    }
    public void PopulateUI()
    {
        if (currentEnemy == null)
        {
            return;
        }
        VisualElement root = combatUI.rootVisualElement;
        VisualElement combatContainer = root.Q<VisualElement>(className: "combat-container");
        combatContainer.Clear();

        //Add player UI section
        VisualElement playerSection = ConstructFighterSection(new PlayerFighter());
        combatContainer.Add(playerSection);

        //Add enemy UI section
        VisualElement enemySection = ConstructFighterSection(currentEnemy);
        combatContainer.Add(enemySection);
    }
    private VisualElement ConstructFighterSection(Fighter fighter)
    {
        Label fighterName = new Label(fighter.GetName());
        fighterName.AddToClassList("fighter-name");

        VisualElement fighterNameBar = new VisualElement();
        fighterNameBar.AddToClassList("fighter-name-bar");
        fighterNameBar.Add(fighterName);

        VisualElement fighterPicture = new VisualElement();
        fighterPicture.AddToClassList("fighter-picture");
        fighterPicture.style.backgroundImage = fighter.GetTexture();

        VisualElement fighterProfile = new VisualElement();
        fighterProfile.AddToClassList("fighter-profile");
        fighterProfile.Add(fighterPicture);
        fighterProfile.Add(fighterNameBar);

        VisualElement fighterSection = new VisualElement();
        fighterSection.AddToClassList("fighter-section");
        fighterSection.Add(fighterProfile);

        return fighterSection;
    }
}
