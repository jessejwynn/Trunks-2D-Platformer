using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float runSpeed = 15f;
    public float airWalkSpeed = 8f;
    public float jumpImpulse = 25f;
    public float wallSlideSpeed = 7f;
    public float gravityScale = 3f;
    public float maxFallSpeed = -15f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float lerpSpeed = 10f;
    public float apexGravityReduction = 0.5f;
    public float bonusApexSpeedMultiplier = 1.2f;
    public float slideAccel = 10f;
    public float jumpCutMultiplier = 0.5f; // Multiplier to control jump height when releasing early

    public float acceleration = 5f; // ✅ Acceleration strength
    public float friction = 5f; // ✅ Friction strength
    private float currentSpeed = 0f; // ✅ Stores the current movement speed


    public bool enableAcceleration = true; // ✅ Toggle for acceleration
    public bool enableFriction = true; // ✅ Toggle for friction

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

    // Climbing variables
    public float climbSpeed = 8f;
    private bool isLadder = false;
    private bool isClimbing = false;

    public LayerMask oneWayPlatformLayer; // Assign the OneWayPlatform layer in the Inspector
    private Collider2D playerCollider;

    public ParticleSystem dust;
    public ParticleSystem landingPoof;
    public ParticleSystem dashTrail;
    private bool wasAirborne = false;

    // SPAWN & RESET
    public Transform spawnPoint; // Assign this in the Inspector
    private Vector2 lastCheckpointPosition;
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
                if (isClimbing) // Add this condition for climbing
                {
                    return walkSpeed; // Use normal walk speed while climbing
                }
                else if (IsMoving && !touchingDirections.IsOnWall)
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

    private static PlayerController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate player
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        playerCollider = GetComponent<Collider2D>();

        if (spawnPoint != null)
        {
            lastCheckpointPosition = spawnPoint.position; // Default spawn point
        }
        else
        {
            lastCheckpointPosition = transform.position; // Fallback to current position
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // If in the tutorial, reset all checkpoint data
        if (currentScene == "Tutorial")
        {
            Debug.Log("Tutorial started! Resetting all checkpoint data.");
            PlayerPrefs.DeleteAll(); // Clears all saved checkpoints
            PlayerPrefs.Save();
        }

        Vector3 spawnPosition;

        // Check if there's a checkpoint for THIS specific level
        if (PlayerPrefs.HasKey(currentScene + "_CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat(currentScene + "_CheckpointX");
            float y = PlayerPrefs.GetFloat(currentScene + "_CheckpointY");
            spawnPosition = new Vector3(x, y, 0);
            Debug.Log($"Respawning at checkpoint for {currentScene}: {spawnPosition}");
        }
        else
        {
            // Default to SpawnPoint
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            spawnPosition = spawnPoint ? spawnPoint.transform.position : Vector3.zero;
            Debug.Log($"No checkpoint found, spawning at default position: {spawnPosition}");
        }

        transform.position = spawnPosition;
    }



    private void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;

        if (touchingDirections.IsGrounded)
        {
            if (wasAirborne)
            {
                CreateLandingPoof();
            }
            lastOnGroundTime = coyoteTime;
            canDoubleJump = true;
            wasAirborne = false;
        }
        else
        {
            wasAirborne = true;
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
            isClimbing = false;
            animator.SetBool("isClimbing", false);
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
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
        }

        if (transform.position.y < fallThreshold)
        {
            ResetToSpawn();
        }

        if (isLadder)
        {
            if (Mathf.Abs(moveInput.y) > 0f)
            {
                isClimbing = true;
                animator.SetBool("isClimbing", true);
                rb.gravityScale = 0f;
            }
            else if (isClimbing)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.gravityScale = 0f;
            }
        }
        else
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.gravityScale = gravityScale;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Collider2D platform = Physics2D.OverlapCircle(transform.position, 0.1f, oneWayPlatformLayer);
            if (platform != null)
            {
                StartCoroutine(DisableCollision(platform));
            }
        }

        if (!isDashing)
        {
            if (Mathf.Abs(currentSpeed) > 0.1f)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
        
    // ✅ **IGNORE ACCELERATION & FRICTION IF DASHING**
        if (!isDashing)
        {
            if (moveInput.x != 0)
            {
                if (enableAcceleration)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, moveInput.x * CurrentMoveSpeed, acceleration * Time.deltaTime);
                }
                else
                {
                    currentSpeed = moveInput.x * CurrentMoveSpeed;
                }
            }
            else
            {
                if (enableFriction)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0, friction * Time.deltaTime);
                }
                else
                {
                    currentSpeed = 0;
                }
            }

            // ✅ Apply Normal Movement ONLY if NOT Dashing
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        }

        // ✅ Handle Facing Direction
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // ✅ Update Animation States
        animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
        animator.SetBool("isGrounded", touchingDirections.IsGrounded);
    }



    void FixedUpdate()
    {
    if (isClimbing)
        {
            rb.gravityScale = 0f; // Completely disable gravity

            if (Mathf.Abs(moveInput.y) > 0f) // If there's vertical input
            {
                rb.velocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed); // Move up or down
            }
            else // If idle while climbing
            {
                rb.velocity = new Vector2(rb.velocity.x, 0); // Stop vertical movement
            }

            // Allow normal horizontal movement while climbing
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }
        else
        {
            rb.gravityScale = gravityScale; // Restore gravity when not climbing
        }

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

            // Wall sliding (only on the Wall layer)
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
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse); // Apply jump force
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        animator.SetTrigger(AnimationStrings.jumpTrigger);
        CreateDust();
        Debug.Log("Jump executed. Velocity: " + rb.velocity);
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

    private float lastDashTime;

    private IEnumerator DashCoroutine()
    {
        lastDashTime = Time.time;
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f; // ✅ Disable gravity temporarily

        animator.SetTrigger(AnimationStrings.dashTrigger); // ✅ Set Dash Animation Trigger

        float dashDirection = IsFacingRight ? 1f : -1f;
        float currentDashSpeed = IsRunning ? runDashSpeed : walkDashSpeed;

        rb.velocity = new Vector2(dashDirection * currentDashSpeed, 0f); // ✅ Apply dash velocity

        // ✅ Play dash VFX
        if (enablePoof && dashTrail != null)
        {
            dashTrail.Play();
        }

        yield return new WaitForSeconds(dashDuration); // ✅ Maintain dash speed

        // ✅ **Fix: Ensure the animation properly resets**
        animator.ResetTrigger(AnimationStrings.dashTrigger);
        animator.SetBool("isDashing", false);  // ✅ Reset dashing state in animation

        rb.gravityScale = originalGravity; // ✅ Restore gravity
        isDashing = false;

        // ✅ Stop dash VFX
        if (dashTrail != null)
        {
            dashTrail.Stop();
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }



    private bool CanJump()
    {
        return (lastOnGroundTime > 0 || isClimbing) && !isJumping;
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

    public void ResetToSpawn()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Vector3 respawnPosition;

        if (PlayerPrefs.HasKey(currentScene + "_CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat(currentScene + "_CheckpointX");
            float y = PlayerPrefs.GetFloat(currentScene + "_CheckpointY");
            respawnPosition = new Vector3(x, y, 0);
            Debug.Log($"Respawning at last checkpoint: {respawnPosition}");
        }
        else
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            respawnPosition = spawnPoint ? spawnPoint.transform.position : Vector3.zero;
            Debug.Log($"No checkpoint found, respawning at start position: {respawnPosition}");
        }

        transform.position = respawnPosition;
        rb.velocity = Vector2.zero; // Reset movement
    }


    public void SetCheckpoint(Vector2 checkpointPosition)
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        PlayerPrefs.SetFloat(currentScene + "_CheckpointX", checkpointPosition.x);
        PlayerPrefs.SetFloat(currentScene + "_CheckpointY", checkpointPosition.y);
        PlayerPrefs.SetString("LastCheckpointScene", currentScene);
        PlayerPrefs.Save();

        Debug.Log($"Checkpoint set at: {checkpointPosition} in scene: {currentScene}");
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
            Debug.Log("Entered ladder area. isLadder: " + isLadder);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            Debug.Log("Exited ladder area. isLadder: " + isLadder);
        }
    }

    private IEnumerator DisableCollision(Collider2D platform)
    {
        // Disable collision between the player and the platform
        Physics2D.IgnoreCollision(playerCollider, platform, true);

        // Wait for a short time (e.g., 0.5 seconds)
        yield return new WaitForSeconds(0.5f);

        // Re-enable collision
        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }

    public float GetDashCooldownPercent()
    {
        if (enableDash && !canDash)
        {
            // Calculate the cooldown percentage (time remaining / total cooldown duration)
            return Mathf.Clamp01((Time.time - lastDashTime) / dashCooldown);
        }
        return 1; // Fully ready if cooldown is complete
    }

}
