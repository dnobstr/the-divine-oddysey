using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BaseHitbox : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float lifetime;
    public string targetTag = "Player";

    protected readonly HashSet<Transform> alreadyHit = new();

    protected virtual void Start()
    {
        Destroy(gameObject, lifetime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        Transform root = other.transform.root;
        if (alreadyHit.Contains(root)) return;

        alreadyHit.Add(root);
        DealDamage(root);
    }

    // This can be overridden if different hitboxes do different things (poison, knockback, etc.)
    protected virtual void DealDamage(Transform target)
    {
        target.GetComponent<Health>().TakeDamage(damage);
    }
}