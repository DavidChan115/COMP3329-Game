using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is the player or a knight
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Knight") || collision.gameObject.CompareTag("keyholder") )
        {
            // Get the Damageable component from the colliding object
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();

            if (damageable != null)
            {
                // Set health to 0 to instantly kill the object
                damageable.Health = 0;
                Debug.Log($"{collision.gameObject.name} hit trap and died!");
            }
        }
    }
}
