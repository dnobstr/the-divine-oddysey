// ModeManager.cs
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the player's active stance.
///
/// Cycle is always 3 slots — the Order and Chaos slots upgrade to their
/// Divine variants while that state is active:
///
///   No divine       →  Normal  →  Order       →  Chaos
///   Divine Order    →  Normal  →  DivineOrder →  Chaos       (Chaos breaks Divine Order)
///   Divine Chaos    →  Normal  →  Order       →  DivineChaos (Order breaks Divine Chaos)
///
/// Pressing cycle while already on a Divine slot steps to the opposite type
/// so the next action can break the divine state.
/// </summary>
public class ModeManager : MonoBehaviour
{
    public MoveVariant currentVariant;

    [Header("Events")]
    public UnityEvent<MoveVariant> onStanceChanged;

    private StateMeter stateMeter;

    private void Awake()
    {
        stateMeter = GetComponent<StateMeter>();
    }

    // ── Cycle ────────────────────────────────────────────────────────────────
    public void cycleStance()
    {
        MoveVariant[] cycle = BuildCycle();

        int idx = System.Array.IndexOf(cycle, currentVariant);
        if (idx < 0) idx = 0;   // current stance not in new cycle → snap to Normal

        MoveVariant next = cycle[(idx + 1) % cycle.Length];
        SetStance(next);

        Debug.Log($"[Stance] → {currentVariant}  |  Cycle: [{string.Join(" → ", cycle)}]");
    }

    /// Builds the 3-slot cycle, upgrading the appropriate slot while divine is active.
    private MoveVariant[] BuildCycle()
    {
        MoveVariant orderSlot = stateMeter.isDivineOrder ? MoveVariant.DivineOrder : MoveVariant.Order;
        MoveVariant chaosSlot = stateMeter.isDivineChaos ? MoveVariant.DivineChaos : MoveVariant.Chaos;
        return new[] { MoveVariant.Normal, orderSlot, chaosSlot };
    }

    // ── Automatic entry (wired from StateMeter events) ───────────────────────
    public void OnDivineOrderReached() => SetStance(MoveVariant.DivineOrder);
    public void OnDivineChaosReached() => SetStance(MoveVariant.DivineChaos);

    // ── Automatic exit (wired from StateMeter events) ────────────────────────
    // Only resets stance if the player is still on the divine slot —
    // if they already switched away to break it, leave them where they are.
    public void OnDivineOrderBroken()
    {
        if (currentVariant == MoveVariant.DivineOrder)
            SetStance(MoveVariant.Normal);
    }

    public void OnDivineChaosBroken()
    {
        if (currentVariant == MoveVariant.DivineChaos)
            SetStance(MoveVariant.Normal);
    }

    private void SetStance(MoveVariant newVariant)
    {
        if (currentVariant == newVariant) return;
        currentVariant = newVariant;
        onStanceChanged?.Invoke(currentVariant);
    }
}