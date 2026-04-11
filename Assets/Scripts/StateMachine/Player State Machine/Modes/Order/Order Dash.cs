// OrderDash.cs
using System.Collections;
using UnityEngine;

public class OrderDash : MonoBehaviour
{
    private PlayerController player;
    private SpriteRenderer sr;

    public void init(PlayerController player)
    {
        this.player = player;
        sr = player.GetComponent<SpriteRenderer>();
    }

    public IEnumerator Execute()
    {
        SetVisibility(false);

        yield return new WaitUntil(() => !player.isDashing);

        player.stateMeter.AddOrder(player.stats.order.dash.meterGain);

        SetVisibility(true);
    }

    private void SetVisibility(bool visible)
    {
        if (!player.attackPressed)
            return;

        Color currentColor = sr.color;

        if (sr == null) 
            return;

        float targetAlpha = visible ? 1.0f : 0.5f;

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, targetAlpha);

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        // The third parameter 'ignore' should be the opposite of 'visible'
        // If visible is false, we WANT to ignore collision (true)
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, !visible);
    }
}