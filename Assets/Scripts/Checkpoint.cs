using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Checkpoint Triggered by: " + collision.gameObject.name); // Debug log for detection

        if (collision.CompareTag("Player") && !isActivated)
        {
            Debug.Log("Checkpoint detected Player, trying to set checkpoint...");

            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("PlayerController found! Setting checkpoint...");
                player.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint Activated at: " + transform.position);
                isActivated = true;
            }
            else
            {
                Debug.LogError("ERROR: PlayerController component NOT found on Player!");
            }
        }
    }
}
