using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats stats;
    [SerializeField] private ControlsManager controlsManager; // Reference to ControlsManager

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Vector2 frameVelocity;

    private bool cachedQueryStartInColliders;
    private float time;
    private bool grounded;
    private float frameLeftGrounded;
    private bool jumpToConsume;
    private bool bufferedJumpUsable;
    private bool endedJumpEarly;
    private bool coyoteUsable;
    private float timeJumpWasPressed;

    // Interface implementation
    public Vector2 FrameInput => new Vector2(controlsManager.IsMoveRight() ? 1 : (controlsManager.IsMoveLeft() ? -1 : 0), 0);
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

        if (controlsManager == null)
        {
            Debug.LogError("ControlsManager is not assigned.");
        }
    }

    private void Update()
    {
        GatherInput();
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime; // Ensure time is updated consistently with FixedUpdate
        CheckCollisions();
        HandleJump();
        HandleDirection();
        HandleGravity();
        ApplyMovement();
    }

    #region Input Handling

    private void GatherInput()
    {
        bool jumpDown = controlsManager.GetJump();
        bool jumpHeld = controlsManager.GetJump(); // Adjust if needed
        Vector2 move = FrameInput;

        // Snap input based on thresholds
        if (stats.SnapInput)
        {
            move.x = Mathf.Abs(move.x) < stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(move.x);
        }

        if (jumpDown)
        {
            jumpToConsume = true;
            timeJumpWasPressed = time;
        }

        frameVelocity.x = move.x * stats.MaxSpeed;
    }

    #endregion

    #region Collision Detection

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling checks
        bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, stats.GrounderDistance, ~stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, stats.GrounderDistance, ~stats.PlayerLayer);

        // Handle ceiling hit
        if (ceilingHit)
        {
            frameVelocity.y = Mathf.Min(0, frameVelocity.y);
        }

        // Handle ground detection
        if (!grounded && groundHit)
        {
            grounded = true;
            coyoteUsable = true;
            bufferedJumpUsable = true;
            endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y));
        }
        else if (grounded && !groundHit)
        {
            grounded = false;
            frameLeftGrounded = time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = cachedQueryStartInColliders;
    }

    #endregion

    #region Jumping

    private bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + stats.JumpBuffer;
    private bool CanUseCoyote => coyoteUsable && !grounded && time < frameLeftGrounded + stats.CoyoteTime;

    private void HandleJump()
    {
        if (!endedJumpEarly && !grounded && !controlsManager.GetJump() && rb.velocity.y > 0)
        {
            endedJumpEarly = true;
        }

        if (!jumpToConsume && !HasBufferedJump) return;

        if (grounded || CanUseCoyote)
        {
            ExecuteJump();
        }

        jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        endedJumpEarly = false;
        timeJumpWasPressed = 0;
        bufferedJumpUsable = false;
        coyoteUsable = false;
        frameVelocity.y = stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal Movement

    private void HandleDirection()
    {
        float targetSpeed = FrameInput.x * stats.MaxSpeed;
        float deceleration = grounded ? stats.GroundDeceleration : stats.AirDeceleration;

        // Smoothly transition to target speed
        frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, targetSpeed, deceleration * Time.fixedDeltaTime);
    }

    #endregion

    #region Gravity Handling

    private void HandleGravity()
    {
        if (grounded && frameVelocity.y <= 0f)
        {
            frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, stats.GroundingForce, Mathf.Abs(stats.GroundingForce) * Time.fixedDeltaTime);
        }
        else
        {
            float inAirGravity = stats.FallAcceleration;
            if (endedJumpEarly && frameVelocity.y > 0)
            {
                inAirGravity *= stats.JumpEndEarlyGravityModifier;
            }
            frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement()
    {
        rb.velocity = frameVelocity;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (stats == null)
        {
            Debug.LogWarning("Please assign a ScriptableStats asset to the Player Movement's Stats slot", this);
        }
        if (controlsManager == null)
        {
            Debug.LogWarning("Please assign a ControlsManager to the Player Movement's Controls Manager slot", this);
        }
    }
#endif
}

// Struct for frame input data
public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move; // This is no longer used for movement but kept for future use if needed
}

// Interface for player controller
public interface IPlayerController
{
    event Action<bool, float> GroundedChanged;
    event Action Jumped;
    Vector2 FrameInput { get; }
}
