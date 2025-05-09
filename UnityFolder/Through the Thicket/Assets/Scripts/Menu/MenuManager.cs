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
        IntegerField seed = root.Q<IntegerField>();
        int seedValue = seed.value;
        if(seedValue == 0)
        {
            seedValue = Random.Range(0, 1);
        }
        StartNewWorld(seedValue);
    }
    void StartNewWorld(int seed)
    {
        WorldClearer.ResetWorld();
        GameParams.worldSeed = seed;
        GameParams.openingNewWorld = true;
        SceneManager.LoadScene("Game");
    }
    void OpenSave(int save)
    {
        GameParams.worldSave = save;
        GameParams.openingNewWorld = false;
        SceneManager.LoadScene("Game");
    }
}
