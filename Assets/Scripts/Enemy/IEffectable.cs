// IEffectable.cs
public interface IEffectable
{
    void Freeze(float duration);   // Order dash + Divine Order dash
    void Ignite(float dps);        // Chaos trail DOT
    void LoseAggro();              // Order vanish
}