// StateMeter.cs
using UnityEngine;
using UnityEngine.Events;

public class StateMeter : MonoBehaviour
{
    [Header("Scale")]
    public float minValue = -100f;  // Chaos limit
    public float maxValue = 100f;   // Order limit
    public float currentValue = 0f; // starts neutral

    [Header("Divine Thresholds")]
    public float orderThreshold = 100f;  // hit max → Divine Order
    public float chaosThreshold = -100f; // hit min → Divine Chaos

    [Header("Events")]
    public UnityEvent onDivineOrder;
    public UnityEvent onDivineOrderBroken; // chaos move used during Divine Order
    public UnityEvent onDivineChaos;
    public UnityEvent onDivineChaosBroken; // order move used during Divine Chaos

    public bool isDivineOrder = false;
    public bool isDivineChaos = false;

    public float Normalized => currentValue / maxValue; // -1 to 1

    // --- Called by Order actions (attack, etc) ---
    public void AddOrder(float amount)
    {
        if (isDivineChaos)
        {
            // Order move breaks Divine Chaos
            isDivineChaos = false;
            onDivineChaosBroken?.Invoke();
            return;
        }

        currentValue = Mathf.Clamp(currentValue + amount, minValue, maxValue);
        CheckThresholds();
    }

    // --- Called by Chaos actions (attack, jump, dash) ---
    public void AddChaos(float amount)
    {
        if (isDivineOrder)
        {
            // Chaos move breaks Divine Order
            isDivineOrder = false;
            onDivineOrderBroken?.Invoke();
            return;
        }

        currentValue = Mathf.Clamp(currentValue - amount, minValue, maxValue);
        CheckThresholds();
    }

    private void CheckThresholds()
    {
        if (!isDivineOrder && currentValue >= orderThreshold)
        {
            isDivineOrder = true;
            onDivineOrder?.Invoke();
        }
        else if (!isDivineChaos && currentValue <= chaosThreshold)
        {
            isDivineChaos = true;
            onDivineChaos?.Invoke();
        }
    }

    public void ResetToNeutral()
    {
        currentValue = 0f;
        isDivineOrder = false;
        isDivineChaos = false;
    }
}