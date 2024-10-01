using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab; // ---- The enemy prefab to spawn ----
    public Transform[] spawnPoints; // ---- Array of spawn points ----
    public float timeBetweenWaves = 5f; // ---- Time between waves ----
    public int enemiesPerWave = 5; // ---- Number of enemies per wave ----
    private int spawnIndex = 0; // ---- Index to alternate between spawn points ----

    public TMP_Text waveText; // ---- UI Text to display the current wave ----
    public TMP_Text waveCompleteText; // ---- UI Text to display wave complete message ----

    private int currentWave = 0;
    private int activeEnemies = 0;
    

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
            yield return StartCoroutine(SpawnWave(currentWave));
            yield return StartCoroutine(CheckWaveComplete());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
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
            yield return new WaitForSeconds(0.5f); // ---- Delay between enemy spawns ----
        }

        
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned.");
            return;
        }

        // ---- Alternate between spawn points ----
        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length;

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.OnEnemyDeath += HandleEnemyDeath;
        Debug.Log($"Enemy {enemy.GetInstanceID()} spawned and event subscribed.");
    }

    private void HandleEnemyDeath()
    {
        activeEnemies--;
        Debug.Log($"Enemy died. Active enemies remaining: {activeEnemies}");
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


