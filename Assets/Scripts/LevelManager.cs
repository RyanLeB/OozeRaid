using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // ---- Static reference to the LevelManager script ----
    public static LevelManager Instance { get; private set; }

    // ---- Variables ----
    public string currentLevel => SceneManager.GetActiveScene().name;
    public List<string> LevelList { get; private set; }


    void Start()
    {
        // ---- Add all levels to list ----
        LevelList = new List<string>();
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            LevelList.Add(sceneName);
        }
    }

    // ---- 
    // NOTE*
    // ----
    // These *LoadNextLevel()* *LoadPreviousLevel()* methods currently aren't being used since the game initially had multiple levels,
    // this has changed to one level early in development.
    // ----

    public void LoadNextLevel()
    {
        int currentIndex = LevelList.IndexOf(currentLevel);
        if (currentIndex >= 0 && currentIndex < LevelList.Count - 1)
        {
            string nextLevel = LevelList[currentIndex + 1];
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            Debug.LogWarning("Next level does not exist.");
        }
    }

    public void LoadPreviousLevel()
    {
        int currentIndex = LevelList.IndexOf(currentLevel);
        if (currentIndex > 0)
        {
            string previousLevel = LevelList[currentIndex - 1];
            SceneManager.LoadScene(previousLevel);
        }
        else
        {
            Debug.LogWarning("Previous level does not exist.");
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }


    public void Quit()
    {
        // ---- Quits the game ----
        Application.Quit();
    }
}
