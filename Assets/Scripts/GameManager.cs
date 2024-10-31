using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;
    public LevelManager levelManager;
    public static GameManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsUpgrades { get; private set; }
    public bool IsControls { get; private set; }

    
    
    public ResultsScreen resultsScreen;
    public GameObject player;

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
            else if (IsUpgrades)
            {
                IsUpgrades = false;
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
    
    
    public void BackToMainMenu()
    {
        IsControls = false;
        uIManager.gameState = UIManager.GameState.MainMenu;
    }
    
    public void OnUpgradesButtonClicked()
    {
        IsUpgrades = true;
        ShowUpgrades();
    }

    public void OnControlsButtonClicked()
    {
        IsControls = true;
        uIManager.gameState = UIManager.GameState.Controls;
    }

    public void CreditsBack()
    {
        IsUpgrades = false;
        IsControls = false;
    }

    void UpdateUIState()
    {
        if (levelManager.currentLevel == "MainMenu" && !IsUpgrades && !IsControls)
        {
            ResetGame(); // ---- Reset the game state ----
            player.SetActive(false);
            uIManager.gameState = UIManager.GameState.MainMenu;
            IsPaused = false;
        }
        else if (levelManager.currentLevel == "Testing")
        {
             
            player.SetActive(true);
            EnablePlayerScripts();
            if (IsPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
        
        else if (levelManager.currentLevel == "Upgrades")
        {
            IsUpgrades = true;
            ResetGame(); // ---- Reset the game state ----
            player.SetActive(false);
            uIManager.gameState = UIManager.GameState.MainMenu;
            IsPaused = false;
        
        
        }
    }
    
    void EnablePlayerScripts()
    {
        MonoBehaviour[] scripts = player.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }
    
    
    public void ResetGame()
    {
        if (resultsScreen != null)
        {
            resultsScreen.gameObject.SetActive(false);
            resultsScreen.canvasGroup.alpha = 0;
        }
        Time.timeScale = 1f; // ---- Resume the game ----
        IsPaused = false;
        
        // ---- Reset player health ----
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }
    }
    
    
    
    public void PlayGame()
    {
        
        ResetGame();
        levelManager.LoadLevel("Testing");
        player.SetActive(true);
        EnablePlayerScripts();
        uIManager.gameState = UIManager.GameState.Game;
        
    }
    
    public void ShowUpgrades()
    {
        
        levelManager.LoadLevel("Upgrades");
        resultsScreen.gameObject.SetActive(false);
        IsUpgrades = true;
        ResetGame(); // ---- Reset the game state ----
        player.SetActive(false);
        uIManager.gameState = UIManager.GameState.Upgrades;
        IsPaused = false;
    }
}
