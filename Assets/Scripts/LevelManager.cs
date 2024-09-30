using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.IO;

public class LevelManager : MonoBehaviour
{


    //NOTE: All scenes must be added to  the build settings in order for this script to work


    //Script call
    public static LevelManager LevelMan;
    //Varaiables
    public string currentLevel;
    public string NextLevel;
    public string PreviousLevel;
    public string sceneName;
    public int SceneCount;
    public List<string> levelList;

    // Start is called before the first frame update
    void Start()
    {
        //Add all levels to list
        levelList = new List<string>();
        SceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < SceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            levelList.Add(sceneName);
        }

    }
    // Update is called once per frame
    void Update()
    {
        //Display current level name in inspector
        currentLevel = SceneManager.GetActiveScene().name;
    }
    public void LoadNextLevel()
    {
        //Load the next level in the list
        NextLevel = levelList[levelList.IndexOf(currentLevel) + 1];
        SceneManager.LoadScene(NextLevel);
    }
    public void LoadPreviousLevel()
    {
        //Load the previous level in the list
        PreviousLevel = levelList[levelList.IndexOf(currentLevel) - 1];
        SceneManager.LoadScene(PreviousLevel);
    }
    public void LoadLevel(string levelName)
    {
        //Load a specific level
        levelName = currentLevel;
        SceneManager.LoadScene(levelName);
    }
    public void Quit()
    {
        //Quit the game
        Application.Quit();
    }
}
