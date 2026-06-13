using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{

    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChanged;
    Animator animator;


    [SerializeField]
    private int _maxHealth = 100;     // if you want the damage to be float like 5.5 then use float,

    public int MaxHealth             // if you want the damage to be integer like 5 then use int,
    {   
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }


    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            // If health drops below 0, character is no longer alive
            if(_health <= 0)
            {
                IsAlive = false;
            }
        }
    }


    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    private bool isInvincible = false;



    public UnityEvent OnDeath;


    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;   // Ex. 10 damage per hit, if each frame hit once, 10 x 0.25 = need 2.5 second for the character to die

    public bool IsAlive 
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);    // for testing

            if (!_isAlive)
            {
                OnDeath?.Invoke(); // Trigger the death event when character dies

                // Check if this is the keyholder
                if (gameObject.CompareTag("keyholder"))
                {
                    GameManager.Instance.SetKeyholderDefeated();
                }
            }

        }
    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        private set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }
















    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(isInvincible)
        {
            if(timeSinceHit > invincibilityTime)
            {
                //Remove invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    
    // return whether the damageable take damage or not
    public bool Hit(int damage, Vector2 knockback, bool triggerAnimation = true)
    {
        if(IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;  // take damage then will be hit for a short period of time, need a timer
            if (triggerAnimation)
            {
                // Notify other subscribed components that the damageable was hit to handle the knockback
                animator.SetTrigger(AnimationStrings.hitTrigger);
                LockVelocity = true;
                damageableHit?.Invoke(damage, knockback);
            }
            Debug.Log("Damage event invoked for " + gameObject.name); // Debug
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);
            // anything subscribes to this event can be notified with this information, we just be notified that this event happens

            return true;
        }
        //unable to be hit
        return false;
    }

    public void Heal(int healthRestore)
    {
        if(IsAlive)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);  // maxHeal value is MaxHealth - Health, lowest is 0(assume you are full health)
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            Debug.Log("Heal event invoked for " + gameObject.name); // Debug
            CharacterEvents.characterHealed(gameObject, actualHeal);
        }
    }


} 
