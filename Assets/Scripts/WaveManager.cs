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
    public TMP_Text waveText; // ---- UI Text to display the current wave ----
    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWave++;
            UpdateWaveText();
            StartCoroutine(SpawnWave(currentWave));
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
        isSpawning = true;
        int enemiesToSpawn = enemiesPerWave * waveNumber;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f); // ---- Delay between enemy spawns ----
        }

        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }
}
