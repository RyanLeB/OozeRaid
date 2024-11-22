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

    private List<int> _availableFloatingEnemySpawnPoints = new List<int>(); // ---- List of available spawn points for floating enemies ----

    public TMP_Text waveText;
    public TMP_Text waveCompleteText;
    public SpriteRenderer backgroundImage;

    public AudioSource firstHalfWaves;
    public AudioSource secondHalfWaves;
    public AudioSource finalHalfWaves;

    public int currentWave = 0;
    public int activeEnemies = 0;

    void Start()
    {
        waveCompleteText.gameObject.SetActive(false);
        StartCoroutine(StartNextWave());

        
        for (int i = 0; i < floatingEnemySpawnPoints.Length; i++)
        {
            _availableFloatingEnemySpawnPoints.Add(i);
        }

        
        StartCoroutine(CheckForAvailableSpawnPoints());
    }

    private IEnumerator StartNextWave()
    {
        while (true)
        {
            currentWave++;
            UpdateWaveText();
            if (currentWave == 8)
            {
                ChangeBackgroundColorToRed();
                firstHalfWaves.Stop();
                secondHalfWaves.Play();
            }
            if (currentWave == 16)
            {
                ChangeBackgroundColorToPurple();
                secondHalfWaves.Stop();
                finalHalfWaves.Play();
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
        floatingEnemyScript.OnEnemyDeath += () => HandleEnemyDeath(spawnIndex);
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

    private void HandleEnemyDeath(int spawnIndex)
    {
        activeEnemies--;
        _activeFloatingEnemies.RemoveAll(fe => fe == null);
        StartCoroutine(MakeSpawnPointAvailable(spawnIndex, 2f)); // ---- Make the spawn point available after 2 seconds ----
    }

    private void HandleDragonEnemyDeath()
    {
        activeEnemies--;

        ResultsScreen results = GameManager.Instance.resultsScreen;
        PlayerCurrency playerCurrency = GameManager.Instance.player.GetComponent<PlayerCurrency>();
        if (results != null)
        {
            results.ShowResults(currentWave, Time.timeSinceLevelLoad, playerCurrency.GetCurrency());
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
            if (_availableFloatingEnemySpawnPoints.Count > 0 && _activeFloatingEnemies.Count < 2)
            {
                SpawnFloatingEnemy();
            }
            yield return new WaitForSeconds(0.5f); // ---- Checks for spawn every 1.5 seconds ----
        }
    }
}