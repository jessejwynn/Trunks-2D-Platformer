using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    public float wallSlideSpeed = 5f;
    public float gravityScale = 3f;

    public bool enableDoubleJump = true;
    public bool enableWallSlide = true;
    public bool enableDash = true;

    private bool canDoubleJump = false;

    private bool canDash = true;
    private bool isDashing;
    public float walkDashSpeed = 15f;
    public float runDashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    Vector2 moveInput;
    TouchingDirections touchingDirections;

    public float CurrentMoveSpeed { get
        {
            if(CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        if (IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        //air move
                        return airWalkSpeed;
                    }

                }
                else
                {
                    return 0;
                }
            } else
            {
                //movement locked
                return 0;
            }
            
        } }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving { get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField]
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
            animator.SetBool(AnimationStrings.isRunning, value );  
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
            if(_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            } 
            _isFacingRight = value;
             
        } }

    public bool CanMove { get
            {
            return animator.GetBool(AnimationStrings.canMove); 
            }
        }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            // Faster Fall
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravityScale * 1.5f;
            }

            // rb.velocity = new Vector2(moveInput.x * walkSpeed * Time.fixedDeltaTime, rb.velocity.y)

            // no wall glide
            // rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

            if (enableWallSlide && touchingDirections.IsOnWall && !touchingDirections.IsGrounded && rb.velocity.y < 0)
            {
                // Wall sliding
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                animator.SetBool(AnimationStrings.isOnWall, true);
            }
            else
            {
                rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
                animator.SetBool(AnimationStrings.isOnWall, false);
            }
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }
    
    public void OnMove(InputAction.CallbackContext context) 
    {
        // x and y movement input
        moveInput = context.ReadValue<Vector2>();

        //gives true or false statement
        IsMoving = moveInput != Vector2.zero;


        SetFacingDirection(moveInput);
    }


   
    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            // face right
            IsFacingRight = true;
        } else if ( moveInput.x < 0 && IsFacingRight) 
        {
            // face left
            IsFacingRight = false;
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started) 
        {
            IsRunning = true;
        } else if(context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //no double jump
        //if (context.started && touchingDirections.IsGrounded && CanMove) 
        //{
        //    animator.SetTrigger(AnimationStrings.jumpTrigger);
        //    rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);

        //}

        //double jump
        if (context.started && CanMove)
        {
            if (touchingDirections.IsGrounded)
            {
                // Regular jump
                animator.SetTrigger(AnimationStrings.jumpTrigger);
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
                if (enableDoubleJump)
                {
                    canDoubleJump = true;
                }
            }
            else if (enableDoubleJump && canDoubleJump)
            {
                // Double jump
                animator.SetTrigger(AnimationStrings.doubleJumpTrigger);
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
                canDoubleJump = false;
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !isDashing)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (enableDash && context.started && CanMove && IsMoving && !isDashing && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        animator.SetTrigger(AnimationStrings.dashTrigger);
        float currentDashSpeed = IsRunning ? runDashSpeed : walkDashSpeed;
        rb.velocity = new Vector2(IsFacingRight ? currentDashSpeed : -currentDashSpeed, 0f);
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.ResetTrigger(AnimationStrings.dashTrigger);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
