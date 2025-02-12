using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector timeline; // Drag your Timeline PlayableDirector here
    public string nextSceneName; // Set this in the Inspector to your desired scene

    void Start()
    {
        if (timeline != null)
            timeline.stopped += OnCutsceneEnd;
    }

    void OnCutsceneEnd(PlayableDirector director)
    {
        if (director == timeline)
        {
            SceneManager.LoadScene(nextSceneName); // Load the next scene
        }
    }
}
