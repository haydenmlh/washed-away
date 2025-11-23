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

    private PlayerInputActions inputActions;  // Your generated class
    private int index;

    private void Awake()
    {
        // Create instance once in Awake (better than Start)
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Always enable input when this object is active
        inputActions.Enable();
        
        // Optional: Subscribe to Submit action via event (cleaner than polling)
        inputActions.UI.Submit.performed += OnSubmitPerformed;
    }

    private void OnDisable()
    {
        // Always clean up
        inputActions.UI.Submit.performed -= OnSubmitPerformed;
        inputActions.Disable();
    }

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
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