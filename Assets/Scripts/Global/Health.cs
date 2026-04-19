using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Events")]
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        OnDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
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
        OnDeath?.Invoke();

        // For enemies, this might trigger the despawn delay
        Destroy(gameObject);
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}