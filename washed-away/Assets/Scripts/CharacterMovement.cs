using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour  // Add: Rigidbody2D if using physics movement
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Interaction")]
    public float interactDistance = 2f;
    public LayerMask interactableLayer = -1;  // Set to Interactable layer

    [Header("Prompt")]
    public string defaultPrompt = "Press E to interact";

    private PlayerInputActions playerInput;
    private Vector2 currentMovementInput;
    [HideInInspector] public bool canMove = true;

    private Interactable currentInteractable;
    [SerializeField] private Animator animator;
    

    private void Awake()
    {
        playerInput = new PlayerInputActions();
        playerInput.Player.Move.performed += ctx => currentMovementInput = ctx.ReadValue<Vector2>();
        playerInput.Player.Move.canceled += ctx => currentMovementInput = Vector2.zero;
        playerInput.Player.Interact.performed += OnInteract;
    }

    private void OnEnable() => playerInput.Player.Enable();
    private void OnDisable() => playerInput.Player.Disable();

    private void Update()
    {
        if (!canMove)
        {
            HideCurrentPrompt();
            return;
        }

        FindAndHighlightClosest();
    }

    private void FixedUpdate()  // For smooth 2D movement
    {
        if (!canMove) return;

        Vector3 movement = new Vector3(currentMovementInput.x, currentMovementInput.y, 0f);
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
        
        float moveX = currentMovementInput.x;
        // print("moveX");
        // print(moveX);
        float moveY = currentMovementInput.y;
        // print("moveY");
        // print(moveY);
        
        if (moveX > 0)
        {
            animator.SetBool("isRight", true);
            animator.SetBool("isLeft", false);
        }
        else if (moveX < 0)
        {
            animator.SetBool("isRight", false);
            animator.SetBool("isLeft", true);
        }
        else
        {
            animator.SetBool("isRight", false);
            animator.SetBool("isLeft", false);
        }

        if (moveY > 0)
        {
            animator.SetBool("isUp", true);
            animator.SetBool("isDown", false);
        }
        else if (moveY < 0)
        {
            animator.SetBool("isUp", false);
            animator.SetBool("isDown", true);
        }
        else
        {
            animator.SetBool("isUp", false);
            animator.SetBool("isDown", false);
        }
        // OR: Use Rigidbody2D.velocity for physics: rb.velocity = movement * moveSpeed;
    }

    // 2D-SPECIFIC: Circle overlap + Vector2
    private void FindAndHighlightClosest()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactDistance, interactableLayer);
        Interactable closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                }
            }
        }

        if (closest != currentInteractable)
        {
            HideCurrentPrompt();
            currentInteractable = closest;
            ShowCurrentPrompt();
        }
    }

    private void ShowCurrentPrompt() => currentInteractable?.ShowPrompt(true);
    private void HideCurrentPrompt()
    {
        if (currentInteractable != null)
        {
            currentInteractable.ShowPrompt(false);
            currentInteractable = null;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!canMove || currentInteractable == null) return;
        currentInteractable.Interact();
    }

    public void DisableMovement() => canMove = false;
    public void EnableMovement() => canMove = true;

    // 2D Gizmo: Circle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }

    private void OnDestroy() => HideCurrentPrompt();
}