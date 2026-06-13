using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// You can't add this Playercontroller script to a game object unless Rigidbody2D exists (optional)
[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(Animator))] 
[RequireComponent(typeof(TouchingDirections))] 
[RequireComponent(typeof(Damageable))] 

// Any components you want to attach to a gamne object needs to be a MonoBehaviour,
// which gives functionality like Start() and Update()
public class PlayerController : MonoBehaviour 
{
    public float walkSpeed = 5f; 
    public float runSpeed = 10f; 
    public float jumpImpulse = 5f;
    public float airwalkSpeed = 2f;
    Vector2 moveInput; // Vector2 will have X and Y values as floats move input (Stores input direction (x,y))
    TouchingDirections touchingDirections;
    Damageable damageable;
  

    public float CurrentMoveSpeed 
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall) // Imagine just walking on a plain ground
                {
                    if (touchingDirections.IsGrounded) // Imagine just walking on a plain ground
                    {
                        if(IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else // Imagine jumping and in air
                    {
                        return walkSpeed + airwalkSpeed; 
                    }
                }
                else if (IsMoving && touchingDirections.IsOnWall)
                {
                    return 0;
                }
                else // Idle speed is 0 (!IsMoving)
                {
                    return 0;
                }
            }
            else
            {
                // Movement locked if CanMove is false (Ex. CanMove is false when the character is attacking)
                return 0;
            }
        }
    }


    [SerializeField]
    // Private boolean flags tracking whether the player is moving or running. These are used to control animations.
    private bool _isMoving = false; 

    // Property
    // When IsMoving = true is executed in OnMove, it calls the set accessor of the IsMoving property, 
    // Now IsMoving's parameter named "value" is set to "true"
    public bool IsMoving 
    { 
        get // Returns the current _isMoving value (true if moving, false if not).
        {
            return _isMoving; 
        }
        
        // Updates _isMoving with the provided value and sets the Animator’s "isMoving" 
        // boolean parameter to the same value. This allows the Animator to transition to 
        // a walking animation when IsMoving is true.
        set 
        {   
            _isMoving = value;  // since "value" is set to "true", _isMoving = true
            animator.SetBool(AnimationStrings.isMoving, value); // value = true enables animator 
            // name inside " " must match the parameter's name in animator
        }
    } 

    [SerializeField]
    // Private boolean flags tracking whether the player is moving or running. These are used to control animations.
    private bool _isRunning = false;

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }

        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }


    public bool _isFacingRight = true;

    public bool IsFacingRight 
    { 
        get 
        { 
            return _isFacingRight; 
        } 
        private set
        {
            // Before _isFacingRight = value, we want to check whether value changed
            if(_isFacingRight != value) 
            {
               // Flip the local scale to make the player face the opposite direction 
               transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    // CanMove bool is set up to indicate the character is moving, when attacking this bool is set to false, 
    // which means the character can't move while attacking (set in above current move speed)
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    
    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }










    Rigidbody2D rb; // Reference to physics component
    Animator animator;


    // Called as soon as PlayerController exists in our scene 
    // If you something to be found asap, put it in Awake() function
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // GetComponent is assuming the game object this script on, takes no parameter
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

    }


    // Runs at fixed time intervals (for physics calculations)
    private void FixedUpdate()
    {   

        if(!damageable.LockVelocity)
        {
            // .velocity means you already set speed per second and it updates
            // rb.velocity.y times nothing = the fall speed will be controlled completely by gravity
            // Vector2 here takes two inputs, x and y
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }


    // When you press "D", the Input System detects this and calls the OnMove method, 
    // passing an InputAction.CallbackContext object that describes the input event.
    public void OnMove(InputAction.CallbackContext context) 
    {
        // context.ReadValue<Vector2>() reads the input as a Vector2. For example:
        // D Key: moveInput = (1, 0).
        // A Key: moveInput = (-1, 0).
        // Up/down may affect moveInput.y (though this script only uses x for movement).
        moveInput = context.ReadValue<Vector2>();
        
        if(IsAlive) // if alive then you can move
        {
            // The line IsMoving = moveInput != Vector2.zero evaluates whether moveInput is non-zero.
            // Since moveInput = (1, 0) is not (0, 0), the expression moveInput != Vector2.zero is true.
            // This results in IsMoving = true, which invokes the IsMoving property’s setter. (Go Up)
            IsMoving = moveInput != Vector2.zero; 

            SetFacingDirection(moveInput);
        }
        else // If not alive then block moving function
        {
            IsMoving = false;
        }


    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight) // I am pressing D key and my character is not facing right
        {
            IsFacingRight = true; // set it to face right
        }
        else if (moveInput.x < 0 && IsFacingRight) // I am pressing A key and my character is facing right
        {
            IsFacingRight = false; // set it to NOT face right
        }
    }


    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }

        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {   
        //TO-DO: Check if the player is alive as well
        // If the character is on ground and he can move, then this jump trigger can be triggered
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {   
            // This trigger will be used for ground and air attack so we don't need condition to distinguish them
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {   
            // This trigger will be used for ground and air attack so we don't need condition to distinguish them
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }
    }






    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
