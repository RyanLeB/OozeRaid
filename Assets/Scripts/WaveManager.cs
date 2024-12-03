using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    private List<FloatingEnemy> _activeFloatingEnemies = new List<FloatingEnemy>();
    public GameObject floatingEnemyPrefab;
    public GameObject dragonEnemyPrefab;
    public GameObject explosionPrefab;
    
    
    public Transform[] spawnPoints;
    public Transform[] floatingEnemySpawnPoints;
    public Transform dragonSpawnPoint;
    private bool isDragonFightStarted = false;
    
    public float timeBetweenWaves = 2f;
    public int enemiesPerWave = 5;
    private int _spawnIndex = 0;
    private bool gameCompleted = false;
    
    
    private List<int> _availableFloatingEnemySpawnPoints = new List<int>(); // ---- List of available spawn points for floating enemies ----

    public TMP_Text waveText;
    public TMP_Text waveCompleteText;
    public SpriteRenderer backgroundImage;
    public Slider waveProgressSlider;
    public Image waveProgressFill;

    public int currentWave = 0;
    public int activeEnemies = 0;

    void Start()
    {
        waveCompleteText.gameObject.SetActive(false);
        waveProgressSlider.maxValue = 15; 
        waveProgressSlider.value = 1; 
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
                GameManager.Instance.audioManager.PlayMusic("SecondPhase");


                for (int i = 0; i < floatingEnemySpawnPoints.Length; i++)
                {
                    _availableFloatingEnemySpawnPoints.Add(i);
                }
                
                StartCoroutine(SmoothTransitionToRed());
            }
            if (currentWave == 16)
            {
                StartCoroutine(SmoothTransitionToPurple());
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

        if (waveProgressSlider != null)
        {
            StartCoroutine(AnimateSlider(waveProgressSlider.value, currentWave));
        }
        
        if (currentWave == 16)
        {
            waveText.text = "Wave: " + " ???";
            waveText.color = Color.red;
        }
    }

    


    private IEnumerator SpawnWave(int waveNumber)
    {
        int enemiesToSpawn = enemiesPerWave * waveNumber;
        activeEnemies = enemiesToSpawn;

        float spawnInterval = Mathf.Max(0.1f, 0.5f - (waveNumber * 0.02f)); 

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            if (waveNumber >= 8 && waveNumber <= 15)
            {
                SpawnFloatingEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFloatingEnemy()
    {
        if (isDragonFightStarted || _availableFloatingEnemySpawnPoints.Count == 0)
        {
            return; // ---- Do not spawn floating enemies if the dragon fight has started or there are no available spawn points ----
        }

        int spawnIndex = _availableFloatingEnemySpawnPoints[Random.Range(0, _availableFloatingEnemySpawnPoints.Count)];
        _availableFloatingEnemySpawnPoints.Remove(spawnIndex);

        Transform spawnPoint = floatingEnemySpawnPoints[spawnIndex];
        GameObject floatingEnemy = Instantiate(floatingEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
        FloatingEnemy floatingEnemyScript = floatingEnemy.GetComponent<FloatingEnemy>();
        floatingEnemyScript.OnEnemyDeath += () => HandleFloatingEnemyDeath(spawnIndex);
        _activeFloatingEnemies.Add(floatingEnemyScript);
        
        // ---- Scaling animation ----
        floatingEnemy.transform.localScale = Vector3.zero;
        floatingEnemy.transform.DOScale(new Vector3(3, 3, 1), 1f).SetEase(Ease.OutBounce);

        // ---- Particle effects ----
        GameObject spawnEffect = Instantiate(explosionPrefab, spawnPoint.position, Quaternion.identity);
        ParticleSystem particleSystem = spawnEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            GameManager.Instance.audioManager.PlaySFX("wizardSpawn");
        }
        Destroy(spawnEffect, particleSystem.main.duration);
    }

    private void SpawnDragonEnemy()
    {
        if (dragonSpawnPoint == null)
        {
            Debug.LogError("Dragon spawn point is not assigned.");
            return;
        }

        foreach (var floatingEnemy in _activeFloatingEnemies)
        {
            if (floatingEnemy != null)
            {
                Destroy(floatingEnemy.gameObject);
            }
        }
        _activeFloatingEnemies.Clear();
        
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
        isDragonFightStarted = true;

        // ---- Particle effects ----
        GameObject spawnEffect = Instantiate(explosionPrefab, dragonSpawnPoint.position, Quaternion.identity);
        ParticleSystem particleSystem = spawnEffect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            GameManager.Instance.audioManager.PlaySFX("dragonSpawn");
        }
        Destroy(spawnEffect, particleSystem.main.duration);
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
        StartCoroutine(TriggerCutsceneAndShowResults());
    }
    
    private IEnumerator TriggerCutsceneAndShowResults()
    {
        // ---- Trigger particle effects ----
        StartCoroutine(TriggerMultipleExplosions());

        // ---- Wait for the cutscene duration ----
        yield return new WaitForSeconds(5f);

        // ---- Show results screen ----
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

    private void TriggerExplosiveEffects(List<Vector3> positions)
    {
        foreach (var position in positions)
        {
            GameObject explosionEffect = Instantiate(explosionPrefab, position, Quaternion.identity);
            ParticleSystem particleSystem = explosionEffect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
                GameManager.Instance.audioManager.PlaySFX("enemyDeath");
            }

            Destroy(explosionEffect, particleSystem.main.duration);
        }
    }
    
    private IEnumerator TriggerMultipleExplosions()
    {
        List<Vector3> firstExplosionPositions = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(-1, -1, 0)
        };

        List<Vector3> secondExplosionPositions = new List<Vector3>
        {
            new Vector3(2, 2, 0),
            new Vector3(-2, -2, 0),
            new Vector3(3, 3, 0)
        };

        List<Vector3> thirdExplosionPositions = new List<Vector3>
        {
            new Vector3(1, 1, 0),
            new Vector3(-3, -3, 0),
            new Vector3(6, 3, 0)
        };
        
        
        TriggerExplosiveEffects(firstExplosionPositions);
        yield return new WaitForSeconds(0.5f); 

        
        TriggerExplosiveEffects(secondExplosionPositions);
        yield return new WaitForSeconds(0.5f); 

        
        TriggerExplosiveEffects(firstExplosionPositions);
        yield return new WaitForSeconds(0.5f); 
        
        TriggerExplosiveEffects(secondExplosionPositions);
        yield return new WaitForSeconds(0.5f); 
        
        TriggerExplosiveEffects(thirdExplosionPositions);
        yield return new WaitForSeconds(0.5f);
        
        TriggerExplosiveEffects(secondExplosionPositions);
        yield return new WaitForSeconds(0.5f); 
        
        
        TriggerExplosiveEffects(firstExplosionPositions);
        yield return new WaitForSeconds(0.5f); 
        
        
        TriggerExplosiveEffects(thirdExplosionPositions);
        yield return new WaitForSeconds(0.1f);
        TriggerExplosiveEffects(firstExplosionPositions);
        yield return new WaitForSeconds(0.1f);
        TriggerExplosiveEffects(secondExplosionPositions);
        yield return new WaitForSeconds(0.5f);

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

            GameManager.Instance.audioManager.PlaySFX("waveComplete");
            
            // ---- Reset the text scale and alpha ----
            waveCompleteText.transform.localScale = Vector3.zero;
            waveCompleteText.DOFade(0, 0);

            // ---- Animate the text scale and alpha with a wave effect ----
            Sequence sequence = DOTween.Sequence();
            sequence.Append(waveCompleteText.transform.DOScale(Vector3.one * 3, 0.5f).SetEase(Ease.OutBounce));
            sequence.Join(waveCompleteText.DOFade(1, 0.5f));
            sequence.AppendInterval(1.5f);
            sequence.Append(waveCompleteText.DOFade(0, 0.5f));
            sequence.OnComplete(() => waveCompleteText.gameObject.SetActive(false));

            // ---- Add wave effect ----
            waveCompleteText.transform.DOShakePosition(1f, new Vector3(0, 10, 0), 10, 90, false, true);

            yield return sequence.WaitForCompletion();
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
    
    
    private IEnumerator SmoothTransitionToRed()
    {
        float transitionDuration = 1f;
        float elapsedTime = 0f;
        Color initialColor = backgroundImage.color;
        Color targetColor = Color.red;

        while (elapsedTime < transitionDuration)
        {
            backgroundImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundImage.color = targetColor;
    }
    private IEnumerator SmoothTransitionToPurple()
    {
        float transitionDuration = 1f;
        float elapsedTime = 0f;
        Color initialColor = backgroundImage.color;
        Color targetColor = Color.magenta;

        while (elapsedTime < transitionDuration)
        {
            backgroundImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundImage.color = targetColor;
    }
    
    
    private IEnumerator AnimateSlider(float startValue, float endValue)
    {
        float duration = 1f;
        float elapsedTime = 0f;

        
        Color fillColor = waveProgressFill.color;
        fillColor.a = 1f;
        waveProgressFill.color = fillColor;

        while (elapsedTime < duration)
        {
            waveProgressSlider.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        waveProgressSlider.value = endValue;

        
        fillColor.a = 0.5f;
        waveProgressFill.color = fillColor;
    }
    
    
}