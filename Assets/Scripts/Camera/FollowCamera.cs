using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;

    [Header("Camera Limits")]
    public float minX = -0f;
    public float maxX = 100f;
    public float minY = -1.2f;
    public float maxY = -1.2f;

    void LateUpdate()
    {
        var player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogWarning("FollowCamera: PlayerController.Instance is null");
            return;
        }

        float targetX = Mathf.Clamp(player.transform.position.x, minX, maxX);
        float targetY = Mathf.Clamp(player.transform.position.y, minY, maxY);

        Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
    }
}