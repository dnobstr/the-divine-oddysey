using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Fade")]
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private string fadeOutTrigger = "End";
    [SerializeField] private string fadeInTrigger = "Start";
    [SerializeField] private float transitionTime = 1f;

    [HideInInspector] public string targetSpawnID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // If you want an initial fade-in when the game starts, uncomment:
        // if (transitionAnim != null) transitionAnim.SetTrigger(fadeInTrigger);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void TransitionToScene(string sceneName, string spawnID)
    {
        targetSpawnID = spawnID;
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // Play fade-out
        float waitTime = transitionTime;
        if (transitionAnim != null)
        {
            transitionAnim.ResetTrigger(fadeInTrigger);
            transitionAnim.SetTrigger(fadeOutTrigger);

            // Try to use the longest clip length from the controller as a fallback for timing
            var controller = transitionAnim.runtimeAnimatorController;
            if (controller != null && controller.animationClips != null && controller.animationClips.Length > 0)
            {
                float longest = 0f;
                foreach (var clip in controller.animationClips)
                {
                    if (clip.length > longest) longest = clip.length;
                }

                if (longest > 0f) waitTime = longest;
            }
        }

        yield return new WaitForSeconds(waitTime);

        // Load the scene asynchronously and wait until it's done
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
        if (loadOp != null)
        {
            while (!loadOp.isDone)
            {
                yield return null;
            }
        }
        else
        {
            // fallback to synchronous load if async failed (shouldn't usually happen)
            SceneManager.LoadScene(sceneName);
            yield return null;
        }

        // Allow one frame for OnSceneLoaded to run and spawn placement to occur
        yield return null;

        // Play fade-in
        if (transitionAnim != null)
        {
            transitionAnim.ResetTrigger(fadeOutTrigger);
            transitionAnim.SetTrigger(fadeInTrigger);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerSpawnPoint[] spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

        foreach (PlayerSpawnPoint spawn in spawnPoints)
        {
            if (spawn.spawnID == targetSpawnID)
            {
                player.transform.position = spawn.transform.position;
                break;
            }
        }

    }
}