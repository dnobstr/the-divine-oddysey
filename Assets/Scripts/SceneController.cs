using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] private Animator transitionAnim;

    // Store the target destination data
    public string targetGateName;
    // Stores the player's offset in local space of the entry gate
    private Vector3 savedLocalOffset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Public API used by gates to request a load and spawn at a named gate
    // Accepts a local-space offset computed relative to the entry gate.
    public void LoadScene(string sceneName, string gateName, Vector3 localOffset)
    {
        targetGateName = gateName;
        savedLocalOffset = localOffset;
        StartCoroutine(LoadLevelRoutine(sceneName));
    }

    // Convenience method for callers that used a simple "NextLevel()" trigger
    // It advances to the next build-index scene (if present) using the same loading routine.
    public void NextLevel()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int total = SceneManager.sceneCountInBuildSettings;
        int next = current + 1;
        if (next >= total)
        {
            Debug.LogWarning("SceneController.NextLevel: No next scene in build settings.");
            return;
        }

        // Resolve next scene name from build settings and run the same routine
        string nextSceneName = SceneUtility.GetScenePathByBuildIndex(next);
        // SceneUtility.GetScenePathByBuildIndex returns path; convert to name
        nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextSceneName);
        StartCoroutine(LoadLevelRoutine(nextSceneName));
    }

    IEnumerator LoadLevelRoutine(string sceneName)
    {
        if (transitionAnim != null)
            transitionAnim.SetTrigger("End");

        // small buffer if animator exists
        yield return new WaitForSeconds(transitionAnim != null ? 1f : 0f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        SpawnAtGate();

        if (transitionAnim != null)
            transitionAnim.SetTrigger("Start");
    }

    void SpawnAtGate()
    {
        if (string.IsNullOrEmpty(targetGateName)) return;

        GameObject gate = GameObject.Find(targetGateName);
        if (gate == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Transform the saved local offset (relative to entry gate) into world space of the target gate.
        Vector3 spawnPosition = gate.transform.TransformPoint(savedLocalOffset);
        player.transform.position = spawnPosition;
    }
}