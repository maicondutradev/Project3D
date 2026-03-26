using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravityMultiplier = 1f;
    public Transform cameraTransform;
    public InputActionReference moveAction;

    private CharacterController controller;
    private Animator animator;
    private PlayerAttack playerAttack;
    private float velocityY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerAttack = GetComponent<PlayerAttack>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (controller.isGrounded && velocityY < 0f)
        {
            velocityY = -2f;
        }

        velocityY += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

        if (playerAttack != null && playerAttack.IsAttacking)
        {
            controller.Move(new Vector3(0f, velocityY, 0f) * Time.deltaTime);
            return;
        }

        if (moveAction == null || moveAction.action == null)
        {
            controller.Move(new Vector3(0f, velocityY, 0f) * Time.deltaTime);
            return;
        }

        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
        Vector3 moveDirection = Vector3.zero;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDirection = moveDirection.normalized * moveSpeed;

            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
        }

        moveDirection.y = velocityY;
        controller.Move(moveDirection * Time.deltaTime);
    }
}