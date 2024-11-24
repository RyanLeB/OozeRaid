using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uIManager;
    public LevelManager levelManager;
    public AudioManager audioManager;

    public static GameManager Instance { get; private set; }
    private bool IsPaused;
    private bool IsUpgrades;
    private bool IsControls;

    public GameObject panel;
    public ResultsScreen resultsScreen;
    public GameObject player;

    public bool isDragonDead = false;
    
    
    // ---- Singleton pattern to ensure only one instance of the GameManager ----
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
        audioManager = FindObjectOfType<AudioManager>();

        if (uIManager == null)
        {
            Debug.LogError("UIManager is null");
        }

        audioManager.PlayMusic("MainMenu");
    }

    void Update()
    {
        // HandleKeyBinds();
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
            audioManager.PlayMusic("MainMenu");
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
            audioManager.PlayMusic("Shop");
            player.SetActive(false);
            uIManager.gameState = UIManager.GameState.MainMenu;
            IsPaused = false;
        }
    }

    // ---- Used to re-enable player scripts when the game is played----
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
            playerHealth.ResetSpriteColor();
        }

        // ---- Reset player gun ability ----
        PlayerGun playerGun = player.GetComponentInChildren<PlayerGun>();
        if (playerGun != null)
        {
            playerGun.ResetAbility();
            playerGun.GetComponent<SpriteRenderer>().enabled = true;
        }
        
        SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = true;
        }
        
    }

    public void PlayGame()
    {
        ResetGame();
        levelManager.LoadLevel("Testing");
        player.SetActive(true);
        EnablePlayerScripts();
        uIManager.gameState = UIManager.GameState.Game;
        audioManager.PlayMusic("FirstPhase");
    }

    public void MainMenu()
    {
        levelManager.LoadLevel("MainMenu");
        player.SetActive(false);
        uIManager.gameState = UIManager.GameState.MainMenu;
        IsPaused = false;
    }

    // ---- Method to activate the panel ----
    public void ActivatePanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    // ---- Method to deactivate the panel ----
    public void DeactivatePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void DestroyAllBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }
    
    public void DeactivateAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
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
        audioManager.PlayMusic("Shop");

        player.GetComponent<PlayerUpgrades>().SaveData();
    }
}