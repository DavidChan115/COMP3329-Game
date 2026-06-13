using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    Damageable damageable;
    ProjectileLauncher projectileLauncher;
    public BossDetectionZone detectionZone; // Large zone covering the room
    public BossDetectionZone meleeAttackZone; // Smaller zone for melee attacks

    public float walkSpeed = 3f;
    public float maxSpeed = 4f;
    public float walkAcceleration = 5f;
    public float walkStopRate = 0.05f;
    public float jumpImpulse = 6f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;
    public float meleeAttackCooldown = 2f;
    public float rangedAttackCooldown = 3f;
    public float jumpCooldown = 5f;
    public float dashCooldown = 4f;

    private float lastMeleeAttackTime;
    private float lastRangedAttackTime;
    private float lastJumpTime;
    private float lastDashTime;
    private bool isDashing = false;

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
                gameObject.transform.localScale = new Vector2(-gameObject.transform.localScale.x, gameObject.transform.localScale.y);
                walkDirectionVector = value == WalkableDirection.Right ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive
    {
        get { return animator.GetBool(AnimationStrings.isAlive); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        projectileLauncher = GetComponent<ProjectileLauncher>();

        if (detectionZone == null) Debug.LogError("DetectionZone is not assigned on " + gameObject.name);
        if (meleeAttackZone == null) Debug.LogError("MeleeAttackZone is not assigned on " + gameObject.name);
        if (projectileLauncher == null) Debug.LogError("ProjectileLauncher is not assigned on " + gameObject.name);

        // Initialize WalkDirection based on localScale
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

    private void Update()
    {
        // Update target detection
        HasTarget = detectionZone != null && detectionZone.detectedColliders.Count > 0;

        if (!IsAlive || !HasTarget || !CanMove || isDashing || damageable.LockVelocity)
            return;

        // Find player's position
        Vector2 playerPosition = Vector2.zero;
        if (detectionZone != null && detectionZone.detectedColliders.Count > 0)
        {
            Collider2D playerCollider = detectionZone.detectedColliders[0];
            if (playerCollider != null)
            {
                playerPosition = playerCollider.transform.position;
            }
        }
        else
        {
            // No player detected, skip position-based logic
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

        // Update facing direction
        bool isPlayerToRight = playerPosition.x > transform.position.x;
        WalkDirection = isPlayerToRight ? WalkableDirection.Right : WalkableDirection.Left;

        // Decide action based on distance and cooldowns
        if (meleeAttackZone != null && meleeAttackZone.detectedColliders.Count > 0 && Time.time - lastMeleeAttackTime >= meleeAttackCooldown)
        {
            // Melee attack
            animator.SetTrigger(AnimationStrings.attackTrigger);
            lastMeleeAttackTime = Time.time;
        }
        else if (distanceToPlayer > 2f && Time.time - lastRangedAttackTime >= rangedAttackCooldown)
        {
            // Ranged attack
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
            lastRangedAttackTime = Time.time;
        }
        else if (Time.time - lastDashTime >= dashCooldown && Random.value < 0.3f && distanceToPlayer > 2f)
        {
            // Dash
            StartCoroutine(Dash(playerPosition));
        }
        else if (touchingDirections.IsGrounded && Time.time - lastJumpTime >= jumpCooldown && Random.value < 0.2f)
        {
            // Jump
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
            lastJumpTime = Time.time;
        }
        else
        {
            // Move toward player
            IsMoving = true;
        }
    }

    private void FixedUpdate()
    {
        if (!IsAlive || isDashing || damageable.LockVelocity || !CanMove)
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            return;
        }

        if (HasTarget && touchingDirections.IsGrounded)
        {
            // Accelerate toward player
            rb.linearVelocity = new Vector2(
                Mathf.Clamp(rb.linearVelocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed),
                rb.linearVelocity.y
            );
        }
        else
        {
            // Slow down
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
        }

        // Update animator
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    private IEnumerator Dash(Vector2 targetPosition)
    {
        isDashing = true;
        animator.SetTrigger(AnimationStrings.dashTrigger);
        float startTime = Time.time;

        // Calculate dash direction
        Vector2 dashDirection = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isDashing = false;
        lastDashTime = Time.time;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    public void FireRangedAttack()
    {
        // Called by animation event
        projectileLauncher.numberOfProjectiles = 3; // Fire 3 projectiles
        projectileLauncher.spreadAngle = 20f; // 20-degree spread
        projectileLauncher.FireProjectile();
    }

    public void OnMeleeAttack()
    {
        // Called by animation event if needed
    }
}