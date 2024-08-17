using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableStats", menuName = "ScriptableObjects/ScriptableStats", order = 1)]
public class ScriptableStats : ScriptableObject
{
    [Header("Movement")]
    public float MaxSpeed = 10f;
    public float Acceleration = 10f; // Acceleration rate when moving
    public float GroundDeceleration = 10f;
    public float AirDeceleration = 5f; // Deceleration when in the air
    
    [Header("Jumping")]
    public float JumpPower = 15f;
    public float JumpBuffer = 0.2f; // Time window to buffer jumps
    public float CoyoteTime = 0.2f; // Time after leaving the ground to still allow jumps
    
    [Header("Gravity")]
    public float FallAcceleration = 30f;
    public float MaxFallSpeed = 50f;
    public float GroundingForce = -1f; // Gravity applied when grounded
    public float JumpEndEarlyGravityModifier = 2f; // Modifier for gravity when ending a jump early
    
    [Header("Input Settings")]
    public bool SnapInput = true; // Whether to snap inputs to zero if below thresholds
    public float HorizontalDeadZoneThreshold = 0.1f; // Dead zone for horizontal input
    public float VerticalDeadZoneThreshold = 0.1f; // Dead zone for vertical input
    
    [Header("Collision")]
    public float GrounderDistance = 0.1f; // Distance to check for ground
    public LayerMask PlayerLayer; // Layer mask to ignore when checking collisions

    // Optionally add methods to validate or manipulate these stats if needed
}