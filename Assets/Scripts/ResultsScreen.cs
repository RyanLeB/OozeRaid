using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    // ---- Text objects to display wave, time and currency ----
    
    public TMP_Text waveText;
    public TMP_Text timeText;
    public TMP_Text currencyText;
    public CanvasGroup canvasGroup;

    // ---- Explosion prefab reference ----
    public GameObject explosionPrefab;


    void Start()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void ShowResults(int wave, float time, int currency)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowResultsCoroutine(wave, time, currency));
    }
    
    private IEnumerator ShowResultsCoroutine(int wave, float time, int currency)
    {
        float targetTimeScale = 0.1f;
        float duration = 1f; 
        float elapsedTime = 0f;
        float initialTimeScale = Time.timeScale;

        while (elapsedTime < duration)
        {
            Time.timeScale = Mathf.Lerp(initialTimeScale, targetTimeScale, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = targetTimeScale;

        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1f;

        GameManager.Instance.DeactivateAllEnemies();

        // ---- Disable the player sprite ----
        SpriteRenderer playerSpriteRenderer = GameManager.Instance.player.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = false;
        }

        // ---- Disable the player gun ----
        PlayerGun playerGun = GameManager.Instance.player.GetComponentInChildren<PlayerGun>();
        if (playerGun != null)
        {
            playerGun.GetComponent<SpriteRenderer>().enabled = false;
        }

        // ---- Trigger explosion effect if the player is dead ----
        PlayerHealth playerHealth = GameManager.Instance.player.GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.isDead)
        {
            GameObject explosionEffect = Instantiate(explosionPrefab, GameManager.Instance.player.transform.position, Quaternion.identity);
            ParticleSystem particleSystem = explosionEffect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
                GameManager.Instance.audioManager.PlaySFX("playerExplode");
                GameManager.Instance.audioManager.PlaySFX("playerDeath");
            }
            Destroy(explosionEffect, particleSystem.main.duration);
        }
        
        
        
        
        gameObject.SetActive(true);
        if (GameManager.Instance.isDragonDead)
        {
            waveText.text = "You've beaten the game!";
        }
        else
        {
            waveText.text = "Wave: " + wave;
        }
        timeText.text = "Time: " + time.ToString("F2") + "s";
        currencyText.text = "Blobs: " + currency;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1f).SetEase(Ease.InOutQuad).SetUpdate(true);
    }
}


