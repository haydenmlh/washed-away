using UnityEngine;
using TMPro;  // For TextMeshProUGUI
using UnityEngine.UI;  // For Image fade
using System.Collections;  // For Coroutines

public class InteractableEnd : MonoBehaviour
{
  [Header("Dialogue")]
  [TextArea(3, 10)]
  public string[] dialogueLines;

  [Header("UI Reference")]
  public Dialogue dialogueUI;

  [Header("Prompt")]
  public Canvas promptCanvas;
  public TMPro.TextMeshProUGUI promptText;
  public string promptMessage = "Press 'E' to interact";

  [Header("Player")]
  public CharacterMovement playerMovement;

  [Header("End Fade")]
  public Image fadeImage;  // Assign a full-screen black UI Image (alpha 0 initially)
  public float fadeDuration = 3f;

  private void Awake()
  {
    if (promptText != null)
      promptText.text = promptMessage;

    // Auto-find player movement if not assigned
    if (playerMovement == null)
      playerMovement = FindAnyObjectByType<CharacterMovement>();
  }

  public void Interact()
  {
    ShowPrompt(false);  // Hide prompt immediately

    if (dialogueUI != null && dialogueLines != null && dialogueLines.Length > 0)
    {
      dialogueUI.lines = dialogueLines;
      dialogueUI.gameObject.SetActive(true);
      StartCoroutine(EndGameSequence());
    }
  }

  private IEnumerator EndGameSequence()
  {
    // Wait for all dialogue to finish (Dialogue will auto-deactivate)
    yield return new WaitUntil(() => !dialogueUI.gameObject.activeInHierarchy);

    // Permanently disable player movement
    if (playerMovement != null)
      playerMovement.DisableMovement();

    // Start fade to black
    if (fadeImage != null)
    {
      fadeImage.gameObject.SetActive(true);
      Debug.Log("hello");
      yield return StartCoroutine(FadeToBlack());
    }

    // End the game
    Application.Quit();
  }

  private IEnumerator FadeToBlack()
  {
    Color color = fadeImage.color;
    color.a = 0f;
    fadeImage.color = color;
    Debug.Log("Starting fade to black.");
    float elapsed = 0f;
    while (elapsed < fadeDuration)
    {
      elapsed += Time.unscaledDeltaTime;
      float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
      color.a = alpha;
      fadeImage.color = color;
      yield return null;
    }
    Debug.Log("FadeToBlack complete.");
    color.a = 1f;
    fadeImage.color = color;
  }

  public void ShowPrompt(bool show)
  {
    if (promptCanvas != null)
      promptCanvas.gameObject.SetActive(show);
  }
}