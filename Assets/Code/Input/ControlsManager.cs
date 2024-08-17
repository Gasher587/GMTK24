using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public PlayerControls playerControls;

    private InputAction movement;
    private InputAction jump;
    private InputAction crouch;
    private InputAction buttonA;
    private InputAction buttonB;
    private InputAction bag;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            Debug.LogError("PlayerControls is not initialized.");
            return;
        }

        movement = playerControls.Player.Movement;
        jump = playerControls.Player.Jump;
        crouch = playerControls.Player.Crouch;
        buttonA = playerControls.Player.ButtonA;
        buttonB = playerControls.Player.ButtonB;
        bag = playerControls.Player.Bag;

        movement?.Enable();
        jump?.Enable();
        crouch?.Enable();
        buttonA?.Enable();
        buttonB?.Enable();
        bag?.Enable();
    }

    private void OnDisable()
    {
        movement?.Disable();
        jump?.Disable();
        crouch?.Disable();
        buttonA?.Disable();
        buttonB?.Disable();
        bag?.Disable();
    }

    public float GetMovement()
    {
        return movement?.ReadValue<float>() ?? 0f;
    }

    public bool GetJump()
    {
        return jump?.ReadValue<float>() > 0.5f;
    }

    public bool GetCrouch()
    {
        return crouch?.ReadValue<float>() > 0.5f;
    }

    public bool GetButtonA()
    {
        return buttonA?.ReadValue<float>() > 0.5f;
    }

    public bool GetButtonB()
    {
        return buttonB?.ReadValue<float>() > 0.5f;
    }

    public bool GetBag()
    {
        return bag?.ReadValue<float>() > 0.5f;
    }

    public bool IsMoveRight()
    {
        return GetMovement() > 0.5f;
    }

    public bool IsMoveLeft()
    {
        return GetMovement() < -0.5f;
    }
}
