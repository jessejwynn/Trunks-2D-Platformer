using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    public Image cooldownFillImage; // Assign the CooldownFill image in the Inspector
    private PlayerController playerController; // Now private, so we find it dynamically

    private void Start()
    {
        FindPlayer(); // ✅ Find Trunks when UI starts
    }

    private void Update()
    {
        if (playerController == null)
        {
            FindPlayer(); // ✅ Try to find Trunks again if missing
        }

        if (playerController != null)
        {
            float cooldownRemaining = playerController.GetDashCooldownPercent();
            cooldownFillImage.fillAmount = cooldownRemaining;
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // ✅ Finds Trunks dynamically
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            Debug.Log("DashCooldownUI: Found Trunks!");
        }
        else
        {
            Debug.LogError("DashCooldownUI: Could not find Trunks in scene!");
        }
    }
}
