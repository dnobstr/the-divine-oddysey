using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad = "IntroCutscene";
    [SerializeField] private string targetSpawnID = "DefaultSpawn";

    [Header("Behavior")]
    [SerializeField] private bool useFadeTransition = true;

    public void StartGame()
    {
        if (useFadeTransition && SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, targetSpawnID);
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnExitClick()
    {
        UnityEditor.EditorApplication.isPlaying = false; 
    }
}
