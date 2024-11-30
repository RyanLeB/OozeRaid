using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    private List<FloatingEnemy> _activeFloatingEnemies = new List<FloatingEnemy>();
    public GameObject floatingEnemyPrefab;
    public GameObject dragonEnemyPrefab;

    public Transform[] spawnPoints;
    public Transform[] floatingEnemySpawnPoints;
    public Transform dragonSpawnPoint;

    public float timeBetweenWaves = 2f;
    public int enemiesPerWave = 5;
    private int _spawnIndex = 0;
    private bool gameCompleted = false;
    
    
    private List<int> _availableFloatingEnemySpawnPoints = new List<int>(); // ---- List of available spawn points for floating enemies ----

    public TMP_Text waveText;
    public TMP_Text waveCompleteText;
    public SpriteRenderer backgroundImage;


    public int currentWave = 0;
    public int activeEnemies = 0;

    void Start()
    {
        waveCompleteText.gameObject.SetActive(false);
        StartCoroutine(StartNextWave());

        StartCoroutine(CheckForAvailableSpawnPoints());
        
    }
        

    // ---- Coroutine to start the next wave ----
    private IEnumerator StartNextWave()
    {
        while (true)
        {
            currentWave++;
            UpdateWaveText();
            if (currentWave == 8 && currentWave <= 15)
            {
                ChangeBackgroundColorToRed();
                GameManager.Instance.audioManager.PlayMusic("SecondPhase");
                
                
                for (int i = 0; i < floatingEnemySpawnPoints.Length; i++)
                {
                    _availableFloatingEnemySpawnPoints.Add(i);
                }
            }
            if (currentWave == 16)
            {
                ChangeBackgroundColorToPurple();
                GameManager.Instance.audioManager.PlayMusic("ThirdPhase");
                
                SpawnDragonEnemy();
                yield break;
            }

            yield return StartCoroutine(SpawnWave(currentWave));
            yield return StartCoroutine(CheckWaveComplete());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave + " / " + "15";
        }

        if (currentWave == 16)
        {
            waveText.text = "Wave: " + " ???";
            waveText.color = Color.red;
        }
    }

    private void ChangeBackgroundColorToRed()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.red;
        }
    }

    private void ChangeBackgroundColorToPurple()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.magenta;
        }
    }

    private IEnumerator SpawnWave(int waveNumber)
    {
        int enemiesToSpawn = enemiesPerWave * waveNumber;
        activeEnemies = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            if (waveNumber >= 8 && waveNumber <= 15)
            {
                SpawnFloatingEnemy();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnFloatingEnemy()
    {
        if (_availableFloatingEnemySpawnPoints.Count == 0)
        {
            return;
        }

        int spawnIndex = _availableFloatingEnemySpawnPoints[Random.Range(0, _availableFloatingEnemySpawnPoints.Count)];
        _availableFloatingEnemySpawnPoints.Remove(spawnIndex);

        Transform spawnPoint = floatingEnemySpawnPoints[spawnIndex];
        GameObject floatingEnemy = Instantiate(floatingEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
        FloatingEnemy floatingEnemyScript = floatingEnemy.GetComponent<FloatingEnemy>();
        floatingEnemyScript.OnEnemyDeath += () => HandleFloatingEnemyDeath(spawnIndex);
        _activeFloatingEnemies.Add(floatingEnemyScript);
        activeEnemies++;
    }

    private void SpawnDragonEnemy()
    {
        if (dragonSpawnPoint == null)
        {
            Debug.LogError("Dragon spawn point is not assigned.");
            return;
        }

        // Clear any existing bullets at the dragon's spawn point
        Collider2D[] colliders = Physics2D.OverlapCircleAll(dragonSpawnPoint.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Destroy(collider.gameObject);
            }
        }

        GameObject dragonEnemy = Instantiate(dragonEnemyPrefab, dragonSpawnPoint.position, dragonSpawnPoint.rotation);
        DragonEnemy dragonEnemyScript = dragonEnemy.GetComponent<DragonEnemy>();
        dragonEnemyScript.OnEnemyDeath += HandleDragonEnemyDeath;
        activeEnemies++;
    }

    
    
    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            return;
        }

        Transform spawnPoint = spawnPoints[_spawnIndex];
        _spawnIndex = (_spawnIndex + 1) % spawnPoints.Length;

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.OnEnemyDeath += HandleRegularEnemyDeath;
    }

    private void HandleRegularEnemyDeath()
    {
        activeEnemies--;
    }

    private void HandleFloatingEnemyDeath(int spawnIndex)
    {
        activeEnemies--;
        _activeFloatingEnemies.RemoveAll(fe => fe == null);
        StartCoroutine(MakeSpawnPointAvailable(spawnIndex, 2f)); // ---- Make the spawn point available after 2 seconds ----
    }

    private void HandleDragonEnemyDeath()
    {
        activeEnemies--;
        GameManager.Instance.isDragonDead = true;
        
        GameManager.Instance.DestroyAllBullets();
        ResultsScreen results = GameManager.Instance.resultsScreen;
        PlayerCurrency playerCurrency = GameManager.Instance.player.GetComponent<PlayerCurrency>();
        if (results != null)
        {
            results.ShowResults(currentWave, Time.timeSinceLevelLoad, playerCurrency.GetCurrency());
        }
        gameCompleted = true;

        GameManager.Instance.player.GetComponent<PlayerHealth>().isDead = true;
        GameManager.Instance.player.GetComponent<PlayerHealth>().ResetSpriteColor();
        GameManager.Instance.player.GetComponent<PlayerMovement>().enabled = false;
        GameManager.Instance.player.GetComponentInChildren<PlayerGun>().enabled = false;
        
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(dragonSpawnPoint.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Destroy(collider.gameObject);
            }
        }
    }

    private IEnumerator CheckWaveComplete()
    {
        while (activeEnemies > 0)
        {
            yield return null;
        }

        if (waveCompleteText != null)
        {
            waveCompleteText.gameObject.SetActive(true);
            waveCompleteText.text = "Wave " + currentWave + " Complete!";
            yield return new WaitForSeconds(2f);
            waveCompleteText.gameObject.SetActive(false);
        }
    }

    
    private IEnumerator MakeSpawnPointAvailable(int spawnIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        _availableFloatingEnemySpawnPoints.Add(spawnIndex); 
    }
    
    private IEnumerator CheckForAvailableSpawnPoints()
    {
        while (true)
        {
            if (gameCompleted) // ---- Stop spawning if the game is completed ----
            {
                yield break;
            }

            if (_availableFloatingEnemySpawnPoints.Count > 0 && _activeFloatingEnemies.Count < 2)
            {
                SpawnFloatingEnemy();
            }
            yield return new WaitForSeconds(0.5f); // ---- Checks for spawn every 0.5 seconds ----
        }
    }
}