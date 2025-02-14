using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    public string nextSceneName; // ✅ Set this in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ✅ Make sure Trunks has the tag "Player"
        {
            Debug.Log("Level Complete! Loading next scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName); // ✅ Load the next scene
        }
    }
}

