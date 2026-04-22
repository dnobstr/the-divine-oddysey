using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private PlayerSpawnPoint defaultSpawnPoint;

    private Vector3 currentCheckpointPosition;
    private string currentCheckpointID;
    private bool hasCheckpoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (defaultSpawnPoint != null)
        {
            SetCheckpoint(defaultSpawnPoint);
        }
    }

    public bool SetCheckpoint(PlayerSpawnPoint newCheckpoint)
    {
        if (newCheckpoint == null)
            return false;

        bool isNewCheckpoint = !hasCheckpoint || currentCheckpointID != newCheckpoint.spawnID;

        currentCheckpointPosition = newCheckpoint.transform.position;
        currentCheckpointID = newCheckpoint.spawnID;
        hasCheckpoint = true;

        return isNewCheckpoint;
    }

    public void RespawnPlayer(Transform player)
    {
        if (player == null)
            return;

        if (!hasCheckpoint)
        {
            Debug.LogWarning("No checkpoint has been saved yet.");
            return;
        }

        player.position = currentCheckpointPosition;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}