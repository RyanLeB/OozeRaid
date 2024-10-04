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
        waveText.text = "Wave: " + wave;
        timeText.text = "Time: " + time.ToString("F2") + "s";
        currencyText.text = "Blobs Collected: " + currency;

        canvasGroup.DOFade(1, 1f).SetEase(Ease.InOutQuad);
    }
}
