using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Knight : MonoBehaviour
{   
    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    Damageable damageable;
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone backDetectionZone;
    public DetectionZone cliffDetectionZone;
    
    private float lastFlipTime;
    public float flipCooldown = 0.7f;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {   
            if (_walkDirection != value)
            {
                // Flip the sprite by inverting the x-scale
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);

                // Update walkDirectionVector based on the new direction
                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private void FlipDirection()
    {
        if (Time.time - lastFlipTime < flipCooldown) return;
        
        WalkDirection = WalkDirection == WalkableDirection.Right ? WalkableDirection.Left : WalkableDirection.Right;
        lastFlipTime = Time.time;
    }

    public bool _hasTarget = false;

    public bool HasTarget 
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();

        // Initialize WalkDirection based on localScale.x
        if (gameObject.transform.localScale.x > 0)
        {
            _walkDirection = WalkableDirection.Right;
            walkDirectionVector = Vector2.right;
        }
        else
        {
            _walkDirection = WalkableDirection.Left;
            walkDirectionVector = Vector2.left;
        }
    }


    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); // will not decrease to below 0
        }
    }





    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0 || backDetectionZone.detectedColliders.Count > 0;


        // to detect player at the back and then flip
        if (HasTarget && CanMove && !damageable.LockVelocity)
        {
        // Find the player’s position from the detection zones
        Collider2D playerCollider = attackZone.detectedColliders.Count > 0 
            ? attackZone.detectedColliders[0] 
            : backDetectionZone.detectedColliders[0];

        // Determine if the player is behind the knight and flip if necessary
        Vector2 playerPosition = playerCollider.transform.position;
        bool isPlayerBehind = (WalkDirection == WalkableDirection.Right && playerPosition.x < transform.position.x) ||
                             (WalkDirection == WalkableDirection.Left && playerPosition.x > transform.position.x);

        if (isPlayerBehind)
        {
            FlipDirection();
        }
        }


        if(AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {   
        // Only check for flip when not in hit state and can move
        if (!damageable.LockVelocity && CanMove)
        {
            if (touchingDirections.IsOnWall && touchingDirections.IsGrounded)
            {
                FlipDirection();
            }
        }

        // Apply movement velocity
        if (!damageable.LockVelocity)
        {
            if (CanMove && touchingDirections.IsGrounded)
            {
                // Accelerate towards max Speed
                rb.linearVelocity = new Vector2(
                    Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), //???
                    rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }  
        }

        // Debug log to verify state
        // Debug.Log($"IsOnWall: {touchingDirections.IsOnWall}, WalkDirection: {WalkDirection}, LocalScaleX: {gameObject.transform.localScale.x}, Velocity: {rb.velocity}");
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if(touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
}

