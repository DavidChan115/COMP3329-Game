using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(3f,0);
    public Vector2 knockback = new Vector2(1f,0);
    

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }



    // Start is called before the first frame update
    void Start()
    {   
        // If you want the projectile to be effected by gravity by default, make it dynamic mode rigidbody
        rb.linearVelocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();
        

        if(damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            // Hit the target
            bool gotHit = damageable.Hit(damage, deliveredKnockback, false);  // third parameter = false = projectile hit won't trigger enemy's got hit animation

            if (gotHit)
            {
                // debug will show things in message in console if correctly run
                Debug.Log(collision.name + "hit for " + damage);
                Destroy(gameObject); // after hit destroy the projectile
            }

        }
    }
}
