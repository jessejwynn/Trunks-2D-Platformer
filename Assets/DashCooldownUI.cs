using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    public Image cooldownFillImage; // Assign the CooldownFill image in the Inspector
    public PlayerController playerController; // Reference to your PlayerController script

    private void Update()
    {
        if (playerController != null)
        {
            // Get the current cooldown progress
            float cooldownRemaining = playerController.GetDashCooldownPercent();
            // Update the fill amount of the UI
            cooldownFillImage.fillAmount = cooldownRemaining;
        }
    }
}
