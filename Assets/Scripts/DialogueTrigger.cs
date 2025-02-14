using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialDialogue tutorialDialogue;
    public TutorialLine[] dialogueLines;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            tutorialDialogue.SetDialogue(dialogueLines);
        }
    }
}
