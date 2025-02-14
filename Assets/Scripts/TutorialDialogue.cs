using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialDialogue : MonoBehaviour
{
    public GameObject dialogueBox; // UI Panel for dialogue
    public TextMeshProUGUI textComponent; // UI Text Component
    public float textSpeed = 0.05f; // Speed of text appearance

    private TutorialLine[] currentLines; // Dynamically set lines
    private int index = 0;
    private bool isDialogueActive = false;

    private void Start()
    {
        textComponent.text = "";
        dialogueBox.SetActive(false);
    }

    public void SetDialogue(TutorialLine[] newLines)
    {
        if (!isDialogueActive) // Prevent overriding active dialogue
        {
            currentLines = newLines;
            index = 0;
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueBox.SetActive(true);
        Time.timeScale = 0f; // Pause game
        StartCoroutine(TypeLine());
    }

    private void Update()
    {
        if (isDialogueActive && Input.anyKeyDown)
        {
            if (textComponent.text == currentLines[index].text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = currentLines[index].text;
            }
        }
    }

    private IEnumerator TypeLine()
    {
        textComponent.text = "";
        foreach (char c in currentLines[index].text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed); // Use Realtime to avoid being affected by pause
        }
    }

    private void NextLine()
    {
        if (index < currentLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }
}

[System.Serializable]
public class TutorialLine
{
    public string text;
}
