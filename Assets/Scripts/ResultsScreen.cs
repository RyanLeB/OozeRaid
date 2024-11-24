using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public TMP_Text waveText;
    public TMP_Text timeText;
    public TMP_Text currencyText;
    public CanvasGroup canvasGroup;



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

        Time.timeScale = 0f;

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


