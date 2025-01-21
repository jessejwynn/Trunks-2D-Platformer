using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    public float wallSlideSpeed = 5f;
    public float gravityScale = 3f;
    public float maxFallSpeed = -15f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float lerpSpeed = 10f;
    public float apexGravityReduction = 0.5f;
    public float bonusApexSpeedMultiplier = 1.2f;
    public float slideAccel = 10f;
    public float jumpCutMultiplier = 0.5f; // Multiplier to control jump height when releasing early

    public bool enableDoubleJump = true;
    public bool enableWallSlide = true;
    public bool enableDash = true;
    public bool enableSlide = true;
    public bool enableWallJump = true;

    private bool canDoubleJump = false;
    private bool canDash = true;
    private bool isDashing;
    private bool isSliding;
    private bool isJumping;
    private bool isJumpCut;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private float dashCooldown = 1f;
    private bool isWallJumping;
    private float wallJumpStartTime;
    private float lastOnWallTime;

    public float walkDashSpeed = 15f;
    public float runDashSpeed = 25f;
    public float dashDuration = 0.2f;

    public ParticleSystem dust;
    public ParticleSystem landingPoof;

    // SPAWN & RESET
    public Transform spawnPoint; // Assign this in the Inspector
    public float fallThreshold = -10f; // Y-level below which the player resets

    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Rigidbody2D rb;
    Animator animator;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
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
                        // Air move
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                // Movement locked
                return 0;
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get { return _isMoving; }
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
        get { return _isRunning; }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove) && !isDashing; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }


    private void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;

        if (touchingDirections.IsGrounded && lastOnGroundTime <= 0)
        {
            CreateLandingPoof();
            lastOnGroundTime = coyoteTime;
            canDoubleJump = true; // Reset double jump when grounded
        }

        if (Input.GetButtonDown("Jump"))
        {
            lastPressedJumpTime = jumpBufferTime;
        }

        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;
            isJumpCut = true;
        }

        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            Jump();
        }
        else if (enableWallJump && CanWallJump() && lastPressedJumpTime > 0)
        {
            WallJump();
        }
        else if (enableDoubleJump && canDoubleJump && lastPressedJumpTime > 0 && !touchingDirections.IsGrounded)
        {
            DoubleJump();
        }

        if (isSliding && enableSlide && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, maxFallSpeed, 0));
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            // Apply jump cut multiplier when the jump button is released
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
        }

        if (transform.position.y < fallThreshold)
        {
            ResetToSpawn();
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravityScale * 1.5f;
            }
            else if (isJumpCut)
            {
                rb.gravityScale = gravityScale * 2f;
            }
            else if (isJumping && Mathf.Abs(rb.velocity.y) < 0.1f)
            {
                rb.gravityScale = gravityScale * apexGravityReduction;
                moveInput.x *= bonusApexSpeedMultiplier;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }

            if (enableWallSlide && touchingDirections.IsOnWall && !touchingDirections.IsGrounded && rb.velocity.y < 0)
            {
                // Wall sliding
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                animator.SetBool(AnimationStrings.isOnWall, true);
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y), lerpSpeed * Time.fixedDeltaTime);
                animator.SetBool(AnimationStrings.isOnWall, false);
            }
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
            CreateDust();
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            CreateDust();
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
        if (context.started)
        {
            lastPressedJumpTime = jumpBufferTime;
        }
        if (context.canceled && rb.velocity.y > 0)
        {
            isJumpCut = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier); // Apply jump cut when jump button is released early
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        animator.SetTrigger(AnimationStrings.jumpTrigger);
        CreateDust();
    }

    private void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        canDoubleJump = false;
        animator.SetTrigger(AnimationStrings.doubleJumpTrigger);
        CreateDust();
    }

    private void WallJump()
    {
        isWallJumping = true;
        wallJumpStartTime = Time.time;
        rb.velocity = new Vector2(IsFacingRight ? -walkSpeed : walkSpeed, jumpImpulse);
        animator.SetTrigger(AnimationStrings.jumpTrigger);
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

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanWallJump()
    {
        return lastOnWallTime > 0 && !isJumping;
    }

    private void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    public bool enablePoof = true; 
    void CreateDust()
{
    if (enablePoof && dust != null)
    {
        dust.Play();
    }
}

    void CreateLandingPoof()
    {
        if (enablePoof && landingPoof != null)
        {
            landingPoof.Play();
        }
    }

    private void ResetToSpawn()
    {
        Debug.Log("Player fell below the threshold. Resetting to spawn.");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            rb.velocity = Vector2.zero; // Stop any ongoing movement
        }
        else
        {
            Debug.LogError("Spawn point is not assigned!");
        }
    }
}
