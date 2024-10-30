using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class ButtonHoverTextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color hoverColor = Color.green; // ---- Color when hovered over ----
    public float hoverScale = 1.2f; // ---- Scale factor when hovered over ----
    public float transitionDuration = 0.2f; // ---- Duration of the scale transition ----

    private Color originalColor; // ---- Original color of the text ----
    private Vector3 originalScale; // ---- Original scale of the text ----
    private TMP_Text buttonText; // ---- Text component of the button ----
    private Coroutine scaleCoroutine; // ---- Coroutine for scaling ----

    void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            originalColor = buttonText.color;
            originalScale = buttonText.rectTransform.localScale;
        }
        else
        {
            Debug.LogError("ButtonHoverTextEffect script requires a TMP_Text component on a child GameObject.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = hoverColor;
        }

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleTo(hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = originalColor;
        }

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(ScaleTo(1f)); // Scale back to original size
    }

    private IEnumerator ScaleTo(float targetScale)
    {
        Vector3 targetScaleVector = originalScale * targetScale;
        Vector3 initialScale = buttonText.rectTransform.localScale;
        float time = 0f;

        while (time < transitionDuration)
        {
            buttonText.rectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, time / transitionDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        buttonText.rectTransform.localScale = targetScaleVector;
    }
}


