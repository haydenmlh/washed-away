using UnityEngine;
using TMPro;  // For TextMeshProUGUI

public class Interactable : MonoBehaviour
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
    public bool shouldEnd = false;

    private void Awake()
    {
        
        if (promptText != null)
            promptText.text = promptMessage;

    }

    public void Interact()
    {
        if (dialogueUI != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            dialogueUI.lines = dialogueLines;
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.shouldEnd = shouldEnd;
        }
    }

    public void ShowPrompt(bool show)
    {
        if (promptCanvas != null)
            promptCanvas.gameObject.SetActive(show);
    }
}