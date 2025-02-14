using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void LoadLevelA()
    {
        SceneManager.LoadScene("LevelA"); // ✅ Change "LevelA" to your actual scene name
    }

    public void LoadLevelB()
    {
        SceneManager.LoadScene("LevelB"); // ✅ Change "LevelB" to your actual scene name
    }

    public void LoadLevelC()
    {
        SceneManager.LoadScene("LevelC"); // ✅ Change "LevelC" to your actual scene name
    }
}

