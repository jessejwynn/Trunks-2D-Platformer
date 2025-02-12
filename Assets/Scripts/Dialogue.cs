using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ✅ NEW: Needed for scene transitions

public class CutsceneDialogue : MonoBehaviour
{
    public GameObject dialogueBox;
    public CanvasGroup dialogueCanvasGroup;
    public TextMeshProUGUI textComponent;
    public Image characterImage;
    public Image cutsceneImage;
    public Image fadeFilter;
    public CutsceneLine[] cutsceneLines;
    public float textSpeed;
    public float fadeDuration = 1f;
    public string nextSceneName; // ✅ NEW: Set this in the Inspector to define the next scene

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        dialogueCanvasGroup.alpha = 0;
        fadeFilter.color = new Color(fadeFilter.color.r, fadeFilter.color.g, fadeFilter.color.b, 1);
        StartCoroutine(StartCutsceneWithFade());
    }

    IEnumerator StartCutsceneWithFade()
    {
        yield return StartCoroutine(FadeFilter(1, 0));
        StartCutscene();
    }

    void StartCutscene()
    {
        index = 0;
        UpdateCutsceneUI();
        if (!string.IsNullOrEmpty(cutsceneLines[index].text))
        {
            StartCoroutine(ShowDialogueBox());
            StartCoroutine(TypeLine());
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (textComponent.text == cutsceneLines[index].text)
            {
                StartCoroutine(NextLineWithConditionalFade());
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = cutsceneLines[index].text;
            }
        }
    }

    IEnumerator TypeLine()
    {
        textComponent.text = "";
        foreach (char c in cutsceneLines[index].text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator NextLineWithConditionalFade()
    {
        bool shouldFade = false;

        if (index < cutsceneLines.Length - 1 && cutsceneLines[index].cutsceneSprite != cutsceneLines[index + 1].cutsceneSprite)
        {
            shouldFade = true;
        }

        if (shouldFade)
        {
            yield return StartCoroutine(FadeFilter(0, 1));
        }

        NextLine();

        if (shouldFade)
        {
            yield return StartCoroutine(FadeFilter(1, 0));
        }
    }

    void NextLine()
    {
        if (index < cutsceneLines.Length - 1)
        {
            index++;
            StopAllCoroutines();
            UpdateCutsceneUI();

            if (!string.IsNullOrEmpty(cutsceneLines[index].text))
            {
                StartCoroutine(ShowDialogueBox());
                StartCoroutine(TypeLine());
            }
            else
            {
                StartCoroutine(HideDialogueBox());
            }
        }
        else
        {
            StartCoroutine(EndCutscene()); // ✅ NEW: Ends cutscene and loads next scene
        }
    }

    IEnumerator EndCutscene()
    {
        StartCoroutine(HideDialogueBox());
        yield return StartCoroutine(FadeFilter(0, 1)); // ✅ Fade out to black

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName); // ✅ Load the next scene
        }
    }

    void UpdateCutsceneUI()
    {
        textComponent.text = "";

        if (!string.IsNullOrEmpty(cutsceneLines[index].text))
        {
            StartCoroutine(ShowDialogueBox());
        }
        else
        {
            StartCoroutine(HideDialogueBox());
        }

        if (cutsceneLines[index].characterSprite != null)
        {
            characterImage.sprite = cutsceneLines[index].characterSprite;
            characterImage.enabled = true;
        }
        else
        {
            characterImage.enabled = false;
        }

        if (cutsceneLines[index].cutsceneSprite != null)
        {
            cutsceneImage.sprite = cutsceneLines[index].cutsceneSprite;
            cutsceneImage.enabled = true;
        }
        else
        {
            cutsceneImage.enabled = false;
        }
    }

    IEnumerator FadeFilter(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color fadeColor = fadeFilter.color;

        while (elapsedTime < fadeDuration)
        {
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeFilter.color = fadeColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeColor.a = endAlpha;
        fadeFilter.color = fadeColor;
    }

    IEnumerator ShowDialogueBox()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            dialogueCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        dialogueCanvasGroup.alpha = 1;
    }

    IEnumerator HideDialogueBox()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            dialogueCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        dialogueCanvasGroup.alpha = 0;
    }
}

[System.Serializable]
public class CutsceneLine
{
    public string text;
    public Sprite characterSprite;
    public Sprite cutsceneSprite;
}
