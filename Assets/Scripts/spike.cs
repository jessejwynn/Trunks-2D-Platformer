using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Player hit spikes! Respawning...");
                player.ResetToSpawn(); // Reset player to last checkpoint
            }
        }
    }
}
