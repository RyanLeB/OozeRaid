using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;
    public LevelManager levelManager;
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsCredits { get; private set; }
    public bool IsControls { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        uIManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();

        if (uIManager == null)
        {
            Debug.LogError("UIManager is null");
        }
    }

    void Update()
    {
        HandleKeyBinds();
        UpdateUIState();
    }

    void HandleKeyBinds()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (levelManager.currentLevel != "MainMenu")
            {
                if (IsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            else if (IsCredits)
            {
                IsCredits = false;
            }
        }
    }

    void Pause()
    {
        IsPaused = true;
        uIManager.gameState = UIManager.GameState.Pause;
        Time.timeScale = 0f; // ---- Pause the game ----
    }

    public void Resume()
    {
        IsPaused = false;
        uIManager.gameState = UIManager.GameState.Game;
        Time.timeScale = 1f; // ---- Resume the game ----
    }

    public void OnCreditsButtonClicked()
    {
        IsCredits = true;
        ShowCredits();
    }

    public void OnControlsButtonClicked()
    {
        IsControls = true;
        uIManager.gameState = UIManager.GameState.Controls;
    }

    public void CreditsBack()
    {
        IsCredits = false;
        IsControls = false;
    }

    void UpdateUIState()
    {
        if (levelManager.currentLevel == "MainMenu" && !IsCredits && !IsControls)
        {
            uIManager.gameState = UIManager.GameState.MainMenu;
            IsPaused = false;
        }
        else if (levelManager.currentLevel == "Testing")
        {
            if (IsPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    void ShowCredits()
    {
        if (IsCredits)
        {
            levelManager.LoadLevel("Credits");
            uIManager.gameState = UIManager.GameState.Credits;
        }
    }
}
