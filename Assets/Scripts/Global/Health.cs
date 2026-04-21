using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;
    public bool isVulnerable { get; set; } = true;

    [Header("Events")]
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void takeDamage(float amount)
    {
        if (!isVulnerable) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void applyDOT(float totalDamage, float duration, float totalTicks)
    {
        // Check to prevent division by zero or infinite loops
        if (totalTicks <= 0 || duration <= 0) return;

        StartCoroutine(DOTCoroutine(totalDamage, duration, totalTicks));
    }

    private IEnumerator DOTCoroutine(float totalDamage, float duration, float totalTicks)
    {
        float damagePerTick = totalDamage / totalTicks;
        float timeBetweenTicks = duration / totalTicks;

        for (int i = 0; i < totalTicks; i++)
        {
            // Use the takeDamage method you already have
            takeDamage(damagePerTick);

            // Wait for the next interval
            yield return new WaitForSeconds(timeBetweenTicks);
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount}. HP: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // For enemies, this might trigger the despawn delay
        Destroy(gameObject);
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}