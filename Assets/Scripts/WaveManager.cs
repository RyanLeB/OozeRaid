using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // ---- The enemy prefab to spawn ----
    private List<FloatingEnemy> activeFloatingEnemies = new List<FloatingEnemy>();
    public GameObject floatingEnemyPrefab; // ---- The floating enemy prefab to spawn ----
    public Transform[] spawnPoints; // ---- Array of spawn points for normal enemies ----
    public Transform[] floatingEnemySpawnPoints; // ---- Array of spawn points for floating enemies ----
    public float timeBetweenWaves = 5f; // ---- Time between waves ----
    public int enemiesPerWave = 5; // ---- Number of enemies per wave ----
    private int _spawnIndex = 0; // ---- Index to alternate between spawn points ----

    public TMP_Text waveText; // ---- UI Text to display the current wave ----
    public TMP_Text waveCompleteText; // ---- UI Text to display wave complete message ----
    public SpriteRenderer backgroundImage; // ---- Reference to the background image ----

    public AudioSource firsthalfWaves; // ---- Audio source for first half of waves (0-10)----
    public AudioSource secondhalfWaves; // ---- Audio source for second half of waves (11-20)----

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
                firsthalfWaves.Stop(); // ---- Stop the first half of waves audio ----
                secondhalfWaves.Play(); // ---- Play the second half of waves audio ----
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
    }

    private void ChangeBackgroundColorToRed()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.red;
        }
    }

    private IEnumerator SpawnWave(int waveNumber)
    {
        int enemiesToSpawn = enemiesPerWave * waveNumber;
        activeEnemies = enemiesToSpawn;
        Debug.Log($"Spawning wave {waveNumber} with {enemiesToSpawn} enemies.");

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
            Debug.LogError("No floating enemy spawn points assigned.");
            return;
        }

        // ---- Check if there are fewer than 2 active floating enemies ----
        if (activeFloatingEnemies.Count >= 2)
        {
            Debug.Log("There are already 2 active floating enemies. Skipping spawn.");
            return;
        }

        // ---- Spawn the floating enemy at a random floating enemy spawn point ----
        Transform spawnPoint = floatingEnemySpawnPoints[Random.Range(0, floatingEnemySpawnPoints.Length)];
        GameObject floatingEnemy = Instantiate(floatingEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
        FloatingEnemy floatingEnemyScript = floatingEnemy.GetComponent<FloatingEnemy>();
        floatingEnemyScript.OnEnemyDeath += HandleEnemyDeath;
        activeFloatingEnemies.Add(floatingEnemyScript); // ---- Add to the list of active floating enemies ----
        activeEnemies++; // ---- Increment activeEnemies count ----
        Debug.Log($"Floating enemy {floatingEnemy.GetInstanceID()} spawned and event subscribed.");
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }

        // ---- Alternate between spawn points ----
        Transform spawnPoint = spawnPoints[_spawnIndex];
        _spawnIndex = (_spawnIndex + 1) % spawnPoints.Length;

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.OnEnemyDeath += HandleEnemyDeath;
        Debug.Log($"Enemy {enemy.GetInstanceID()} spawned and event subscribed.");
    }

    private void HandleEnemyDeath()
    {
        activeEnemies--;
        Debug.Log($"Enemy died. Active enemies remaining: {activeEnemies}");

        // ---- Remove from the list of active floating enemies if it is a floating enemy ----
        activeFloatingEnemies.RemoveAll(fe => fe == null);
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