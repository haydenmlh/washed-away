using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public float moveSpeed = 5f;
    private PlayerInputActions playerInput;
    private Vector2 currentMovementInput;

    private void Awake()
  {
    playerInput = new PlayerInputActions();
    playerInput.Player.Move.performed += ctx => currentMovementInput = ctx.ReadValue<Vector2>();
    playerInput.Player.Move.canceled += ctx => currentMovementInput = Vector2.zero;
  }

  private void OnEnable()
  {
    playerInput.Player.Enable();
  }

  private void OnDisable()
  {
    playerInput.Player.Disable();
  }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(currentMovementInput.x, currentMovementInput.y, 0f);
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
    }
}
