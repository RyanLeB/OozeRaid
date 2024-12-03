using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingDamageManager : MonoBehaviour
{
    public static FloatingDamageManager Instance;

    [SerializeField] private GameObject floatingDamageNumberPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowFloatingDamage(int damage, Vector3 position, bool isCrit)
    {
        //Debug.Log($"ShowFloatingDamage called with damage: {damage}, position: {position}, isCrit: {isCrit}");

        if (floatingDamageNumberPrefab == null)
        {
            Debug.LogError("FloatingDamageNumberPrefab is not assigned.");
            return;
        }

        GameObject floatingDamageNumber = Instantiate(floatingDamageNumberPrefab, position + Vector3.up * 1.5f, Quaternion.identity);
        floatingDamageNumber.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        TextMeshPro textMesh = floatingDamageNumber.GetComponentInChildren<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component not found on FloatingDamageNumberPrefab.");
            return;
        }

        textMesh.text = isCrit ? $"<color=red>{damage} Crit!</color>" : damage.ToString();
        StartCoroutine(BounceAndFadeOut(floatingDamageNumber));
    }

    private IEnumerator BounceAndFadeOut(GameObject floatingDamageNumber)
    {
        TextMeshPro textMesh = floatingDamageNumber.GetComponent<TextMeshPro>();
        Color originalColor = textMesh.color;
        float duration = .5f;
        float bounceHeight = 0.8f;
        float elapsedTime = 0f;

        Vector3 originalPosition = floatingDamageNumber.transform.position;
        Vector3 originalScale = floatingDamageNumber.transform.localScale;

        // ---- Generate a random angle between -45 and 45 degrees ----
        float randomAngle = Random.Range(-45f, 45f);
        Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        while (elapsedTime < duration)
        {
            float bounce = Mathf.Sin(Mathf.PI * elapsedTime / duration) * bounceHeight;
            floatingDamageNumber.transform.position = originalPosition + direction * bounce;

            // ---- Apply scaling effect ----
            float scale = Mathf.Lerp(1f, 1.5f, Mathf.PingPong(elapsedTime * 2f, 1f));
            floatingDamageNumber.transform.localScale = originalScale * scale;

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(floatingDamageNumber);
    }
}
