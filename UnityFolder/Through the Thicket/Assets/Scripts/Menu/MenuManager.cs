using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument menuUI;
    public void Awake()
    {
        VisualElement root = menuUI.rootVisualElement;
        Button startButton = root.Q<Button>("startButton");
        startButton.clicked += () => StartClicked();
    }
    void StartClicked()
    {
        VisualElement root = menuUI.rootVisualElement;
        Toggle startNewToggle = root.Q<Toggle>();
        bool startNew = startNewToggle.value;
        if (startNew)
        {
            IntegerField seed = root.Q<IntegerField>();
            int seedValue = seed.value;
            if (seedValue == 0)
            {
                seedValue = Random.Range(0, 1);
            }
            StartNewWorld(seedValue);
            return;
        }
        OpenSave();
    }
    void StartNewWorld(int seed)
    {
        WorldClearer.ResetWorld();
        GameParams.worldSeed = seed;
        GameParams.openingNewWorld = true;
        SceneManager.LoadScene("Game");
    }
    void OpenSave()
    {
        GameParams.worldSeed = FileHelper.GetSaveSeed();
        GameParams.openingNewWorld = false;
        SceneManager.LoadScene("Game");
    }
}
