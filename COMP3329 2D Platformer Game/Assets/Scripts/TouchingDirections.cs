using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Purpose of this script: use the CapsuleCollider2D on the GameObject to check its surroundings,
// and determine if it’s: on the ground / touching a wall / touching a ceiling
// useful for controlling player movement (e.g., preventing movement through walls, enabling jumps only when grounded),
// and triggering animations (e.g., playing a "grounded" animation when the player is on the ground).




// The script inherits from MonoBehaviour, making it a component that can be attached to a GameObject in Unity.
public class TouchingDirections : MonoBehaviour 
{   
    // A ContactFilter2D used to filter what the raycasts will collide with (e.g., specific layers like "Ground" or "Walls").
    // You can set this in the Unity Inspector to define which layers the raycasts should detect. 
    public ContactFilter2D castFilter;


    // These float define how far the raycasts will extend when checking for collisions in each direction 
    // (down for ground, sideways for walls, up for ceiling). 
    // Smaller values (like 0.05f) mean the object must be very close to a surface to register as "touching."
    public float groundDistance = 0.05f;
    public float wallDistance = 0.1f;
    public float ceilingDistance = 0.05f;


    CapsuleCollider2D touchingCol;
    Animator animator;


    // These Arrays to store the results of raycasts in each direction. 
    // Each array can hold up to 5 hits (e.g., if the raycast hits multiple objects). 
    // This is a precaution to handle edge cases where multiple collisions might occur.
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];



    // This property determines the direction to cast rays for wall detection.
    // If transform.localScale.x > 0 is true, then we are facing right. If x < 0, we are facing left
    // This is a common technique in 2D games to handle character facing direction based on sprite flipping.
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;


    
    [SerializeField]
    private bool _isGrounded = true;

    public bool IsGrounded 
    {
        get
        {
            return _isGrounded;
        }

        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }


    [SerializeField]
    private bool _isOnWall;

    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }


    [SerializeField]
    private bool _isOnCeiling;

    public bool IsOnCeiling
    {
        get
        {
            return _isOnCeiling;
        }
        private set
        {
            _isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);
        }
    }


    // Called as soon as TouchingDirections exists in our scene 
    // If you something to be found asap, put it in Awake() function
    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>(); 
        animator = GetComponent<Animator>();
    }
    

    // Called at fixed time intervals (used for physics updates in Unity, typically 50 times per second)
    void FixedUpdate()
    {   
        // Casts rays downward from the collider.
        // If the ray hits something within groundDistance (0.05f), 
        // the result will be greater than 0, meaning the object is grounded.
        // This sets IsGrounded to true, which also updates the Animator.
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;

        // Casts rays in the direction determined by wallCheckDirection (left or right).
        // If a hit occurs within wallDistance (0.2f), IsOnWall is set to true.
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;

        // Casts rays upward.
        // If a hit occurs within ceilingDistance (0.05f), IsOnCeiling is set to true.
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }

}
