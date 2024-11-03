using System.Collections;
using UnityEngine;
using TMPro;

public class BlobCount : MonoBehaviour
{
    private PlayerCurrency playerCurrency; // ---- Reference to the PlayerCurrency script ----
    public TMP_Text blobCountText; // ---- Reference to the UI Text element for displaying the blob count ----
    public TMP_Text blobCollectedText; // ---- Reference to the UI Text element for displaying the "+ number of blobs" message ----

    void Start()
    {
        GameObject player = GameManager.Instance.player; // ---- Access the player object through the GameManager singleton ----
        if (player != null)
        {
            playerCurrency = player.GetComponent<PlayerCurrency>(); // ---- Get the PlayerCurrency component ----
        }

        if (playerCurrency == null)
        {
            Debug.LogError("PlayerCurrency component not found on the player.");
        }

        UpdateBlobCount();
        blobCollectedText.gameObject.SetActive(false); // ---- Hide the "+ number of blobs" message initially ----
    }

    void Update()
    {
        if (playerCurrency != null)
        {
            UpdateBlobCount();
        }
    }

    public void OnBlobCollected(int amount)
    {
        if (playerCurrency != null)
        {
            playerCurrency.AddCurrency(amount);
            UpdateBlobCount();
            StartCoroutine(ShowBlobCollectedMessage(amount));
        }
    }

    void UpdateBlobCount()
    {
        if (playerCurrency != null)
        {
            blobCountText.text = "" + playerCurrency.GetCurrency();
        }
    }

    IEnumerator ShowBlobCollectedMessage(int amount)
    {
        Debug.Log("Showing blob collected message: + " + amount);
        blobCollectedText.text = "+ " + amount;
        blobCollectedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f); // ---- Show the message for 1 second ----
        blobCollectedText.gameObject.SetActive(false);
        Debug.Log("Hiding blob collected message");
    }
}