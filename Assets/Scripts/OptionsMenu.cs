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
        doubleJumpToggle.onValueChanged.AddListener(isOn => playerController.enableDoubleJump = isOn);
        wallSlideToggle.onValueChanged.AddListener(isOn => playerController.enableWallSlide = isOn);
        dashToggle.onValueChanged.AddListener(isOn => playerController.enableDash = isOn);
        slideToggle.onValueChanged.AddListener(isOn => playerController.enableSlide = isOn);
        wallJumpToggle.onValueChanged.AddListener(isOn => playerController.enableWallJump = isOn);
        poofToggle.onValueChanged.AddListener(isOn => playerController.enablePoof = isOn);

        // Add listeners for sliders
        gravityScaleSlider.onValueChanged.AddListener(value => playerController.gravityScale = value);
        jumpImpulseSlider.onValueChanged.AddListener(value => playerController.jumpImpulse = value);
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


    public void SetWallSlide(bool isOn) => playerController.enableWallSlide = isOn;
    public void SetDash(bool isOn) => playerController.enableDash = isOn;
    public void SetSlide(bool isOn) => playerController.enableSlide = isOn;
    public void SetWallJump(bool isOn) => playerController.enableWallJump = isOn;
    public void SetPoof(bool isOn) => playerController.enablePoof = isOn;

    // Public methods for sliders
    public void SetGravityScale(float value) => playerController.gravityScale = value;
    public void SetJumpImpulse(float value) => playerController.jumpImpulse = value;

}
