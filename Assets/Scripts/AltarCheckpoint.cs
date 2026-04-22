using UnityEngine;

[RequireComponent(typeof(PlayerSpawnPoint))]
public class AltarCheckpoint : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string activateTriggerName = "isTrigger";
    [SerializeField] private bool playAnimationEveryInteract = false;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactPrompt;

    private PlayerSpawnPoint spawnPoint;
    private bool playerInRange;
    private bool alreadyActivated;

    private void Awake()
    {
        spawnPoint = GetComponent<PlayerSpawnPoint>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    public void TryInteract()
    {
        if (!playerInRange)
            return;

        if (CheckpointManager.Instance == null)
        {
            Debug.LogWarning("No CheckpointManager found in the scene.");
            return;
        }

        bool isNewCheckpoint = CheckpointManager.Instance.SetCheckpoint(spawnPoint);

        if (!alreadyActivated || playAnimationEveryInteract || isNewCheckpoint)
        {
            if (animator != null && !string.IsNullOrWhiteSpace(activateTriggerName))
            {
                animator.ResetTrigger(activateTriggerName);
                animator.SetTrigger(activateTriggerName);
            }
        }

        alreadyActivated = true;
        Debug.Log($"Checkpoint saved: {spawnPoint.spawnID}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}