using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // ---- Enum for the different game states ----

    
    public enum GameState
    {
        MainMenu,
        Game,
        Credits,
        Pause,
        Controls,
    }

    public GameState gameState;
    public List<GameObject> uiElements;

    void Start()
    {
        uiElements = new List<GameObject>();
        foreach (Transform child in transform)
        {
            uiElements.Add(child.gameObject);
        }
    }

    void Update()
    {
        UpdateUIState();
    }

    void UpdateUIState()
    {
        // ---- Deactivate all UI elements ----
        foreach (var element in uiElements)
        {
            element.SetActive(false);
        }

        // ---- Activate specific UI elements based on the game state ----
        switch (gameState)
        {
            case GameState.MainMenu:
                ActivateUIElement(0);
                break;
            case GameState.Game:
                // ---- No UI elements to activate for the game state ----
                break;
            case GameState.Credits:
                ActivateUIElement(1);
                break;
            case GameState.Pause:
                ActivateUIElement(2);
                break;
            case GameState.Controls:
                ActivateUIElement(3);
                break;
        }
    }

    void ActivateUIElement(int index)
    {
        if (index >= 0 && index < uiElements.Count)
        {
            uiElements[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"UI element at index {index} does not exist.");
        }
    }
}
