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
         }

         // ---- Destroy the blob after reaching the player ----
         Destroy(gameObject);
     }
 }
