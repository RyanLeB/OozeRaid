using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // ---- The enemy prefab to spawn ----
    private List<FloatingEnemy> _activeFloatingEnemies = new List<FloatingEnemy>();
    public GameObject floatingEnemyPrefab; // ---- The floating enemy prefab to spawn ----
    public GameObject dragonEnemyPrefab; // ---- The dragon enemy prefab to spawn ----
    
    public Transform[] spawnPoints; // ---- Array of spawn points for normal enemies ----
    public Transform[] floatingEnemySpawnPoints; // ---- Array of spawn points for floating enemies ----
    public Transform dragonSpawnPoint; // ---- Spawn point for the dragon enemy ----
    
    public float timeBetweenWaves = 2f; // ---- Time between waves ----
    public int enemiesPerWave = 5; // ---- Number of enemies per wave ----
    private int _spawnIndex = 0; // ---- Index to alternate between spawn points ----

    public TMP_Text waveText; // ---- UI Text to display the current wave ----
    public TMP_Text waveCompleteText; // ---- UI Text to display wave complete message ----
    public SpriteRenderer backgroundImage; // ---- Reference to the background image ----
    
    
    
    
    public AudioSource firstHalfWaves; // ---- Audio source for first half of waves (0-10)----
    public AudioSource secondHalfWaves; // ---- Audio source for second half of waves (11-20)----
    public AudioSource finalHalfWaves; // ---- Audio source for final fight (16)----
    
    public int currentWave = 0;
    public int activeEnemies = 0;

    void Start()
    {
        waveCompleteText.gameObject.SetActive(false); // ---- Hide wave complete text initially ----
        StartCoroutine(StartNextWave());
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
                firstHalfWaves.Stop(); // ---- Stop the first half of waves audio ----
                secondHalfWaves.Play(); // ---- Play the second half of waves audio ----
                
            }
            if (currentWave == 16)
            {
                ChangeBackgroundColorToPurple();
                secondHalfWaves.Stop(); // ---- Stop the second half of waves audio ----
                finalHalfWaves.Play(); // ---- Play the final fight audio ----
                SpawnDragonEnemy();
                yield break; // ---- Stop spawning other waves ----
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
        //Debug.Log($"Spawning wave {waveNumber} with {enemiesToSpawn} enemies.");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            if (waveNumber >= 8 && waveNumber <= 15)
            {
                SpawnFloatingEnemy();
            }
            yield return new WaitForSeconds(0.5f); // ---- Delay between enemy spawns ----
        }
    }

    private void SpawnFloatingEnemy()
    {
        if (floatingEnemySpawnPoints.Length == 0)
        {
            //Debug.LogError("No floating enemy spawn points assigned.");
            return;
        }

        // ---- Check if there are fewer than 2 active floating enemies ----
        if (_activeFloatingEnemies.Count >= 2)
        {
            //Debug.Log("There are already 2 active floating enemies. Skipping spawn.");
            return;
        }

        // ---- Spawn the floating enemy at a random floating enemy spawn point ----
        Transform spawnPoint = floatingEnemySpawnPoints[Random.Range(0, floatingEnemySpawnPoints.Length)];
        GameObject floatingEnemy = Instantiate(floatingEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
        FloatingEnemy floatingEnemyScript = floatingEnemy.GetComponent<FloatingEnemy>();
        floatingEnemyScript.OnEnemyDeath += HandleEnemyDeath;
        _activeFloatingEnemies.Add(floatingEnemyScript); // ---- Add to the list of active floating enemies ----
        activeEnemies++; // ---- Increment activeEnemies count ----
        //Debug.Log($"Floating enemy {floatingEnemy.GetInstanceID()} spawned and event subscribed.");
    }

    
    private void SpawnDragonEnemy()
    {
        if (dragonSpawnPoint == null)
        {
            Debug.LogError("Dragon spawn point is not assigned.");
            return;
        }

        // ---- Spawn the dragon enemy at the dragon spawn point ----
        GameObject dragonEnemy = Instantiate(dragonEnemyPrefab, dragonSpawnPoint.position, dragonSpawnPoint.rotation);
        DragonEnemy dragonEnemyScript = dragonEnemy.GetComponent<DragonEnemy>();
        dragonEnemyScript.OnEnemyDeath += HandleDragonEnemyDeath;
        activeEnemies++; // ---- Increment activeEnemies count ----
        //Debug.Log($"Dragon enemy {dragonEnemy.GetInstanceID()} spawned and event subscribed.");
    }
    
    
    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            //Debug.LogError("No spawn points assigned.");
            return;
        }

        // ---- Alternate between spawn points ----
        Transform spawnPoint = spawnPoints[_spawnIndex];
        _spawnIndex = (_spawnIndex + 1) % spawnPoints.Length;

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.OnEnemyDeath += HandleEnemyDeath;
        //Debug.Log($"Enemy {enemy.GetInstanceID()} spawned and event subscribed.");
    }

    private void HandleEnemyDeath()
    {
        activeEnemies--;
        //Debug.Log($"Enemy died. Active enemies remaining: {activeEnemies}");

        // ---- Remove from the list of active floating enemies if it is a floating enemy ----
        _activeFloatingEnemies.RemoveAll(fe => fe == null);
    }

    
    private void HandleDragonEnemyDeath()
    {
        activeEnemies--;
        //Debug.Log($"Dragon enemy died. Active enemies remaining: {activeEnemies}");

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
            yield return new WaitForSeconds(2f); // ---- Display wave complete message for 2 seconds ----
            waveCompleteText.gameObject.SetActive(false);
        }
    }
}