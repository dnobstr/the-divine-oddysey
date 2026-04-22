using System.Collections;
using UnityEngine;

public class PoisonedWaterScript : MonoBehaviour
{
    public GameObject player;
    public Transform respawnPoint;

    [Tooltip("Delay (seconds) before respawning the player")]
    public float respawnDelay = 0.5f;

    private bool isRespawning = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player && !isRespawning)
        {
            StartCoroutine(HandlePoisonedCollision());
        }
    }

    private IEnumerator HandlePoisonedCollision()
    {
        isRespawning = true;

        // Trigger screen fade-out if available
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PlayFadeOut();
        }

        // Wait for respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Move player to respawn point
        if (player != null && respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        // Trigger fade-in
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PlayFadeIn();
        }

        // Small buffer to avoid immediate re-triggering
        yield return new WaitForSeconds(0.1f);
        isRespawning = false;
    }
}
