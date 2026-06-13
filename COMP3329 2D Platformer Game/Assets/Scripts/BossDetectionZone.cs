using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossDetectionZone : MonoBehaviour
{
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D is missing on " + gameObject.name);
        }
        else
        {
            boxCollider.isTrigger = true; // Ensure the collider is a trigger
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!detectedColliders.Contains(collision))
            {
                detectedColliders.Add(collision);
                Debug.Log("Player entered detection zone: " + collision.gameObject.name);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (detectedColliders.Contains(collision))
            {
                detectedColliders.Remove(collision);
                Debug.Log("Player exited detection zone: " + collision.gameObject.name);
            }
        }
    }
}