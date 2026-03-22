using UnityEngine;

public class SceneGate : MonoBehaviour
{
    [Header("Destination Settings")]
    public string sceneToLoad;
    public string targetGateName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (SceneController.instance == null)
        {
            Debug.LogWarning("SceneController.instance is null. Cannot load scene.");
            return;
        }

        // Calculate player's position relative to THIS gate in the gate's local space
        Vector3 localOffset = transform.InverseTransformPoint(collision.transform.position);

        // Send the local offset to the SceneController along with the scene and gate names
        SceneController.instance.LoadScene(sceneToLoad, targetGateName, localOffset);
    }
}   