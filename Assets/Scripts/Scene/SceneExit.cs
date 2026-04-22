using UnityEngine;

public class SceneExit : MonoBehaviour
{
    [Header("Where to go")]
    public string targetSceneName;

    [Header("Which spawn point in the next scene")]
    public string targetSpawnID;

    private bool canTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("SceneTransitionManager is missing!");
            return;
        }

        canTrigger = false;
        Debug.Log("Loading scene: " + targetSceneName);
        SceneTransitionManager.Instance.TransitionToScene(targetSceneName, targetSpawnID);
    }
}