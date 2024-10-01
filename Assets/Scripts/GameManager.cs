using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public UIManager uIManager;
    public LevelManager levelManager;
    public static GameManager instance;
    public bool IsPaused;
    public bool IsCredits;
    public bool IsControls;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        uIManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if (uIManager == null)
        {
            Debug.LogError("UIManager is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyBinds();
        UISelection();
    }
    void KeyBinds()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && levelManager.currentLevel != "MainMenu" && IsPaused == false)
        {
            Pause();
            Debug.Log("Pause");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && levelManager.currentLevel != "MainMenu" && IsPaused == true)
        {
            Resume();
            Debug.Log("Game");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsCredits == true)
        {
            IsCredits = false;
        }
    }
    void Pause()
    {
        IsPaused = true;
        uIManager.gameState = UIManager.GameState.Pause;
    }
    public void Resume()
    {
        IsPaused = false;
        uIManager.gameState = UIManager.GameState.Game;
    }
    public void OnCreditsButtonClicked()
    {
        IsCredits = true;
        ShowCredits();
    }
    public void Creditsback()
    {
        IsCredits = false;
        IsControls = false;
    }
    public void OnControlsButtonClicked()
    {
        IsControls = true;
        uIManager.gameState = UIManager.GameState.Controls;
    }
    void UISelection()
    {
        if (levelManager.currentLevel == "MainMenu" && IsCredits == false && IsControls == false)
        {
            uIManager.gameState = UIManager.GameState.MainMenu;
            IsPaused = false;
        }
        else if (levelManager.currentLevel == "Noah's Testing area")
        {
            if (IsPaused == true)
            {
                Pause();
            }
            else if (IsPaused == false)
            {
                Resume();
            }
        }
    }
    void ShowCredits()
    {
        if (IsCredits == true)
        {
            levelManager.currentLevel = "Credits";
            uIManager.gameState = UIManager.GameState.credits;
        }
    }
}
