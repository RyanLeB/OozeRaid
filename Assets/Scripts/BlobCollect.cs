using UnityEngine;
using System.Collections;
public class BlobCollect : MonoBehaviour
{
    public float moveAwayDistance = 1f; // ---- Distance to move away from the player ----
    public float moveDuration = 0.5f; // ---- Duration of the move away animation ----
    public float returnDuration = 0.5f; // ---- Duration of the return animation ----

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameManager.Instance.player.transform;
        StartCoroutine(AnimateBlob());
    }

    
    // ---- Coroutine to animate the blob, gives it a magnetic effect that goes to the player ----
    public IEnumerator AnimateBlob()
    {
         Vector3 originalPosition = transform.position;
         Vector3 moveAwayPosition = originalPosition + (originalPosition - playerTransform.position).normalized * moveAwayDistance;

         // ---- Move away from the player ----
         float elapsedTime = 0f;
         while (elapsedTime < moveDuration)
         {
             transform.position = Vector3.Lerp(originalPosition, moveAwayPosition, elapsedTime / moveDuration);
             elapsedTime += Time.deltaTime;
             yield return null;
         }

         // ---- Move back to the player ----
         elapsedTime = 0f;
         while (elapsedTime < returnDuration)
         {
             transform.position = Vector3.Lerp(moveAwayPosition, playerTransform.position, elapsedTime / returnDuration);
             elapsedTime += Time.deltaTime;
             yield return null;
         } GameManager.Instance.audioManager.PlaySFXWithRandomPitch("BlobPickup", 0.4f, 1.5f); // ---- Play the blob pickup sound ----
         // ---- Destroy the blob after reaching the player ----
         Destroy(gameObject);
     }
 }
