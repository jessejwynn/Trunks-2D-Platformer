using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    public GameObject optionsPanel; // Reference to the OptionsPanel GameObject
    private bool isOptionsOpen = false; // Tracks whether the OptionsPanel is open

    void Update()
    {
        // Listen for the Esc key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsPanel();
        }
    }

    public void ToggleOptionsPanel()
    {
        // Toggle the active state of the OptionsPanel
        isOptionsOpen = !isOptionsOpen;
        optionsPanel.SetActive(isOptionsOpen);

        // Pause or unpause the game when the menu is toggled
        if (isOptionsOpen)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }
    }

    public void CloseOptionsPanel()
    {
        // Ensures the OptionsPanel closes and unpauses the game
        isOptionsOpen = false;
        optionsPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}
