using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatState : IUIState
{
    [SerializeField] private UIDocument combatUI;
    private Enemy currentEnemy;
    private PlayerFighter player;
    private GameManager gameManager;
    private bool isPlayersGo;
    public override void Initialize(GameManager manager)
    {
        VisualElement root = combatUI.rootVisualElement;
        root.Clear();
        Button closeButton = new Button();
        root.Add(closeButton);
        closeButton.clicked += () => manager.CloseState("Combat");
        Close();
        gameManager = manager;

        VisualElement combatContainer = new VisualElement();
        combatContainer.AddToClassList("combat-container");
        root.Add(combatContainer);
        isPlayersGo = true;
    }
    public override void Open()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.Flex;
        base.Open();
        //For testing TODO remove
        SetFighters(new Squirrel(), new PlayerFighter());
        isPlayersGo = true;
        PopulateUI();
    }
    public override void Close()
    {
        combatUI.rootVisualElement.style.display = DisplayStyle.None;
        base.Close();
    }
    public override void UpdateWhenOpen()
    {
        if (!isPlayersGo)
        {
            Debug.Log("Enemy attacking player");
            player.Damage(2);
            TurnSwap();
            PopulateUI();
        }
    }
    public void SetFighters(Enemy e, PlayerFighter p)
    {
        currentEnemy = e;
        player = p;
    }
    private void AttackButtonClicked(int attackID)
    {
        if (!isPlayersGo)
        {
            return;
        }
        Debug.Log("Player attacking enemy");
        int damage = LoadPlayerWeapons()[attackID].GetAttack().GetDamage();
        currentEnemy.Damage(damage);
        Debug.Log("Damage to enemy was: " + damage);
        if (currentEnemy.IsDead())
        {
            gameManager.CloseState("Combat");
            return;
        }
        TurnSwap();
        PopulateUI();
    }
    private void TurnSwap()
    {
        isPlayersGo = !isPlayersGo;
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
        VisualElement playerSection = ConstructFighterSection(player);
        combatContainer.Add(playerSection);

        //Add enemy UI section
        VisualElement enemySection = ConstructFighterSection(currentEnemy);
        combatContainer.Add(enemySection);
    }
    private VisualElement ConstructFighterSection(Fighter fighter)
    {
        VisualElement fighterProfile = ConstructFighterProfile(fighter);
        VisualElement fighterInfoBox = ConstructFighterInfoBox(fighter);
        VisualElement fighterSection = new VisualElement();
        fighterSection.AddToClassList("fighter-section");
        fighterSection.Add(fighterProfile);
        fighterSection.Add(fighterInfoBox);

        return fighterSection;
    }
    private VisualElement ConstructFighterProfile(Fighter fighter)
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

        return fighterProfile;
    }
    private VisualElement ConstructFighterInfoBox(Fighter fighter)
    {
        VisualElement fighterInfoBox = new VisualElement();
        fighterInfoBox.AddToClassList("fighter-info");

        if (fighter is PlayerFighter)
        {
            VisualElement attackBar = new VisualElement();
            attackBar.AddToClassList("fighter-attack-bar");
            fighterInfoBox.Add(attackBar);
            List<Weapon> weapons = LoadPlayerWeapons();
            for(int i = 0; i < 4 && i < weapons.Count; i++)
            {
                Button attackButton = new Button();
                attackButton.clicked += () => AttackButtonClicked(i - 1); // punch is in first slot so we take away one to make up for it
                attackButton.text = weapons[i].ToString();
                attackBar.Add(attackButton);
            }

            VisualElement fighterSpeech = new Label("Im gonna kill you!");
            fighterSpeech.AddToClassList("fighter-speech");
            fighterInfoBox.Add(fighterSpeech);
        }
        else
        {
            Enemy enemy = fighter as Enemy;
            VisualElement enemyDescription = new VisualElement();
            enemyDescription = new Label(enemy.GetDescription());
            enemyDescription.AddToClassList("fighter-description");
            fighterInfoBox.Add(enemyDescription);

            VisualElement enemySpeech = new Label(enemy.GetVoiceLine());
            enemySpeech.AddToClassList("fighter-speech");
            fighterInfoBox.Add(enemySpeech);
        }


        return fighterInfoBox;
    }
    private List<Weapon> LoadPlayerWeapons()
    {
        List<Weapon> weapons = new List<Weapon> { new PunchWeapon() };
        string path = Path.Combine(Application.persistentDataPath, "World", "Inventory", "Player.json");
        FileHelper.DirectoryCheck();
        PersistentInventories saveData = JsonUtility.FromJson<PersistentInventories>(File.ReadAllText(path));
        Inventory weaponsInventory = saveData.inventories[2].GetInventory(false);
        //TODO extract data
        InventorySlot[,] weaponSlots = weaponsInventory.GetSlots();
        foreach (InventorySlot slot in weaponSlots)
        {
            if(slot.item == null)
            {
                continue;
            }
            IItem item = ItemHelper.GetItemFromType(slot.item.GetItemType());
            if(item is Weapon)
            {
                weapons.Add(item as Weapon);
            }
        }

        return weapons;
    }
}
