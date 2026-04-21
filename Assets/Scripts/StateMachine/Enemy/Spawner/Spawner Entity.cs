using UnityEngine;

public class SpawnerEntity : MonoBehaviour
{
    [Header("Stats Reference")]
    public EnemyStats stats; // Drag the Stats object or prefab here

    private Transform player;
    private float cooldownTimer;

    void Start()
    {
        stats = GetComponent<EnemyStats>();

        // Cache the player reference to avoid frequent Find calls
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Start with a ready cooldown
        cooldownTimer = stats.attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        float xDist = Mathf.Abs(transform.position.x - player.position.x);
        float yDist = Mathf.Abs(transform.position.y - player.position.y);

        // Check if player is within aggro/chaosAttack range
        if (xDist <= stats.attackRange && yDist <= stats.attackHeight)
        {
            HandleSpawning();
        }
    }

    private void HandleSpawning()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= stats.attackCooldown)
        {
            SpawnHitbox();
            cooldownTimer = 0f;
        }
    }

    private void SpawnHitbox()
    {
        if (stats.attackHitboxPrefab == null) return;

        float direction = player.position.x > transform.position.x ? 1 : -1;

        // Calculate position with offset
        Vector3 spawnPos = transform.position + new Vector3(stats.attackOffset * direction, 0, 0);

        GameObject hbObj = Instantiate(stats.attackHitboxPrefab, spawnPos, Quaternion.identity);

        // FIX: Get the original scale of the prefab so we don't force it to (1,1,1)
        Vector3 defaultScale = stats.attackHitboxPrefab.transform.localScale;

        // Apply the direction while preserving the original size
        hbObj.transform.localScale = new Vector3(defaultScale.x * direction, defaultScale.y, defaultScale.z);

        BaseHitbox hb = hbObj.GetComponent<BaseHitbox>();
        if (hb != null)
        {
            hb.damage = stats.attackDamage;
            hb.lifetime = stats.attackDuration;
        }
    }

    // Visualization for the Inspector
    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(stats.attackRange * 2, stats.attackHeight * 2, 1));
    }
}