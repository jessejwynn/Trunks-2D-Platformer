using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle doubleJumpToggle;
    public Toggle wallSlideToggle;
    public Toggle dashToggle;
    public Toggle slideToggle;
    public Toggle wallJumpToggle;
    public Toggle poofToggle;

    // References to sliders
    public Slider gravityScaleSlider;
    public Slider jumpImpulseSlider;

    // Reference to the PlayerController
    public PlayerController playerController;

    public void Start()
    {
        Debug.Log("Checking PlayerController...");
        if (playerController == null) Debug.LogError("PlayerController is not assigned!");

        Debug.Log("Checking Toggles...");
        if (doubleJumpToggle == null) Debug.LogError("DoubleJumpToggle is not assigned!");
        if (wallSlideToggle == null) Debug.LogError("WallSlideToggle is not assigned!");
        if (dashToggle == null) Debug.LogError("DashToggle is not assigned!");
        if (slideToggle == null) Debug.LogError("SlideToggle is not assigned!");
        if (wallJumpToggle == null) Debug.LogError("WallJumpToggle is not assigned!");
        if (poofToggle == null) Debug.LogError("PoofToggle is not assigned!");

        Debug.Log("Checking Sliders...");
        if (gravityScaleSlider == null) Debug.LogError("GravityScaleSlider is not assigned!");
        if (jumpImpulseSlider == null) Debug.LogError("JumpImpulseSlider is not assigned!");

        // Initialize toggles with player controller values
        doubleJumpToggle.isOn = playerController.enableDoubleJump;
        wallSlideToggle.isOn = playerController.enableWallSlide;
        dashToggle.isOn = playerController.enableDash;
        slideToggle.isOn = playerController.enableSlide;
        wallJumpToggle.isOn = playerController.enableWallJump;
        poofToggle.isOn = playerController.enablePoof;

        // Initialize sliders with current values
        gravityScaleSlider.value = playerController.gravityScale;
        jumpImpulseSlider.value = playerController.jumpImpulse;

        // Add listeners for toggles
        doubleJumpToggle.onValueChanged.AddListener(SetDoubleJump);
        wallSlideToggle.onValueChanged.AddListener(SetWallSlide);
        dashToggle.onValueChanged.AddListener(SetDash);
        slideToggle.onValueChanged.AddListener(SetSlide);
        wallJumpToggle.onValueChanged.AddListener(SetWallJump);
        poofToggle.onValueChanged.AddListener(SetPoof);

        // Add listeners for sliders
        gravityScaleSlider.onValueChanged.AddListener(SetGravityScale);
        jumpImpulseSlider.onValueChanged.AddListener(SetJumpImpulse);
    }

    // Public methods for toggles
    public void SetDoubleJump(bool isOn)
    {
        Debug.Log("Double Jump toggled: " + isOn);
        if (playerController != null)
        {
            playerController.enableDoubleJump = isOn;
        }
    }

    public void SetWallSlide(bool isOn)
    {
        Debug.Log("Wall Slide toggled: " + isOn);
        if (playerController != null)
        {
            playerController.enableWallSlide = isOn;
        }
    }

    public void SetDash(bool isOn)
    {
        Debug.Log("Dash toggled: " + isOn);
        if (playerController != null)
        {
            playerController.enableDash = isOn;
        }
    }

    public void SetSlide(bool isOn)
    {
        Debug.Log("Slide toggled: " + isOn);
        if (playerController != null)
        {
            playerController.enableSlide = isOn;
        }
    }

    public void SetWallJump(bool isOn)
    {
        Debug.Log("Wall Jump toggled: " + isOn);
        if (playerController != null)
        {
            playerController.enableWallJump = isOn;
        }
    }

    public void SetPoof(bool isOn)
    {
        Debug.Log("Poof toggled: " + isOn);
        if (playerController != null)
        {
            playerController.enablePoof = isOn;
        }
    }

    // Public methods for sliders
    public void SetGravityScale(float value)
    {
        Debug.Log("Gravity Scale set to: " + value);
        if (playerController != null)
        {
            playerController.gravityScale = value;
        }
    }

    public void SetJumpImpulse(float value)
    {
        Debug.Log("Jump Impulse set to: " + value);
        if (playerController != null)
        {
            playerController.jumpImpulse = value;
        }
    }
}