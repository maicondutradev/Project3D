using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float attackDuration = 1f;
    public Transform cameraTransform;

    public InputActionReference moveAction;
    public InputActionReference attackAction;

    private CharacterController controller;
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (attackAction != null && attackAction.action.WasPressedThisFrame() && !isAttacking)
        {
            AlignPlayerWithCamera();
            StartCoroutine(AttackRoutine());
        }

        if (isAttacking)
        {
            return;
        }

        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion toRotation = Quaternion.Euler(0f, targetAngle, 0f);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);

            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void AlignPlayerWithCamera()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(cameraForward.normalized);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }
}