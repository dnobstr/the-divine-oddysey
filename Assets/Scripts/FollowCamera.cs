using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Added [SerializeField] so you can adjust the speed in the Unity Inspector
    [SerializeField] private float followSpeed = 0.1f;

    [Header("Camera Limits")]
    public float minX = -50f; // The furthest left the camera can go (adjust as needed)
    public float maxX = 10f;  // The furthest right (This is where your white line is!)

    void Start()
    {

    }

    // Changed to LateUpdate to prevent camera jitter
    void LateUpdate()
    {
        if (PlayerMovement.Instance != null)
        {
            // 1. Get the player's current X and Y
            float targetX = PlayerMovement.Instance.transform.position.x;
            float targetY = PlayerMovement.Instance.transform.position.y;

            // 2. Clamp the X position so it never goes past your white line
            targetX = Mathf.Clamp(targetX, minX, maxX);

            // 3. Create the new target destination for the camera
            Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);

            // 4. Smoothly Lerp to the clamped destination
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        }
    }
}