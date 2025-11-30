using System;
using System.Linq.Expressions;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MoveController : MonoBehaviour
{
    [SerializeField] public static MoveController instance { get; private set; } // for restarting position
    [SerializeField] private Animator animator;


    [SerializeField] private float moveSpeed = 5f;
    // [SerializeField] private float jumpPower = 0.3f;
    // [SerializeField] private float gravityForce = -10f;
    // [SerializeField] private float yVelocity = -1f;
    [SerializeField] private Vector3 velocity = new Vector3(0, 0, 0);
    Vector3 movement;
    [SerializeField] Transform groundCheck; //empty Child of Player acts as "Feet" to check if on Ground
    [SerializeField] LayerMask groundMask;

    void Update()
    {


        float moveX = Input.GetAxisRaw("Horizontal");
        // print("moveX");
        // print(moveX);

        float moveZ = Input.GetAxisRaw("Vertical");
        // print("moveZ");
        // print(moveZ);
        if (moveX == 1)
        {
            animator.SetBool("isRight", true);
            animator.SetBool("isLeft", false);
        }
        else if (moveX == -1)
        {
            animator.SetBool("isRight", false);
            animator.SetBool("isLeft", true);
        }
        else
        {
            animator.SetBool("isRight", false);
            animator.SetBool("isLeft", false);
        }


        if (moveZ == 1)
        {
            animator.SetBool("isUp", true);
            animator.SetBool("isDown", false);
        }
        else if (moveZ == -1)
        {
            animator.SetBool("isUp", false);
            animator.SetBool("isDown", true);
        }
        else
        {
            animator.SetBool("isUp", false);
            animator.SetBool("isDown", false);
        }



        // print(moveX.ToString() + moveZ.ToString() + player.sprite.ToString());

        movement = new Vector3(moveX, 0f, moveZ).normalized;



        float speed = moveSpeed; //New Var, to double when sprint
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))
        {
            speed = 1.7f * moveSpeed;
        }

        Vector3 targetPos = transform.position + movement * speed * Time.deltaTime;


        transform.position = targetPos;


        // Commented out Jumping Logic

        // Collider2D col = Physics2D.OverlapCircle(targetPos, 0.4f, LayerMask.GetMask("Border"));
        // if(col == null){
        //     transform.position = targetPos;  
        // }

        // // Jumping Logic
        // if (isGrounded() && Input.GetKeyDown(KeyCode.Space))
        // {
        //     yVelocity = jumpPower * -2f * gravityForce;
        // }
        // if (!isGrounded())
        // {
        //     yVelocity += gravityForce * Time.deltaTime;
        // }
        // else // isGrounded(), cancel its negative y velocity after landing on floor
        // {
        //     if (yVelocity < 0f)
        //     {
        //         yVelocity = 0f;
        //     }
        // }
        // float playerY = transform.position.y + yVelocity * Time.deltaTime;


        // transform.position = new Vector3(transform.position.x, playerY, transform.position.z);

    }

    public bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);
    }

    void Start()
    {
        instance = this; // singleton pattern - assign instance static variable to itself at game start
        
        // new method to set sanity/health/money

        // UIHandler.instance.SetSanityValue(80);
        // UIHandler.instance.SetHealthValue(80);
        // UIHandler.instance.SetMoneyValue(20);

    }

    public void RestartPosition()
    {
        transform.position = new Vector3(-7f, 1.55f, -24f);
    }

}
