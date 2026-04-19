using UnityEngine;

public class ExpandingHitbox : BaseHitbox
{
    public Vector2 startSize = new Vector2(1, 1);
    public Vector2 endSize = new Vector2(4, 2);
    private BoxCollider2D boxCol;
    private float age;

    protected override void Start()
    {
        base.Start(); // Runs the Destroy(lifetime) logic from Base
        boxCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        age += Time.deltaTime;
        boxCol.size = Vector2.Lerp(startSize, endSize, age / lifetime);
    }
}