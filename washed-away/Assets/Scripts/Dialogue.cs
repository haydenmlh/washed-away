using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;   // Required for Input System

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.05f;

    [Header("Optional: Auto-disable on finish")]
    public bool disableOnFinish = true;

    [Header("Player Control")]
    public CharacterMovement playerMovement; // Assign in Inspector!

    private PlayerInputActions inputActions;

    public SceneController sceneController;
    private int index;

    public bool shouldEnd = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // Auto-find if not assigned
        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<CharacterMovement>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Submit.performed += OnSubmitPerformed;

        // Disable player movement when dialogue starts
        if (playerMovement != null)
            playerMovement.DisableMovement();

        // FIX: clean slate every time the dialogue opens
        StopAllCoroutines();
        textComponent.text = string.Empty;
        index = 0;
        StartDialogue();
    }

    private void OnDisable()
    {
        inputActions.UI.Submit.performed -= OnSubmitPerformed;
        inputActions.Disable();

        if (shouldEnd)
        {
          sceneController.LoadLevel("EndGame");
        }

        // Re-enable movement when dialogue ends
        if (playerMovement != null)
            playerMovement.EnableMovement();
    }

    // This method is called every time the player presses Submit (Space/Enter/A button)
    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        if (textComponent.text == lines[index])
        {
            NextLine();
        }
        else
        {
            // Instantly finish typing current line
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            // Dialogue finished
            if (disableOnFinish)
                gameObject.SetActive(false);
        }
    }

    // Optional: Visual debugging in Inspector
#if UNITY_EDITOR
    private void Reset()
    {
        // Auto-assign TextMeshProUGUI if in same GameObject
        if (textComponent == null)
            textComponent = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}