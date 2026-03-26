using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public float attackDuration = 1f;
    public Transform cameraTransform;
    public InputActionReference attackAction;

    private Animator animator;
    public bool IsAttacking { get; private set; }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (attackAction != null && attackAction.action != null && attackAction.action.WasPressedThisFrame() && !IsAttacking)
        {
            AlignPlayerWithCamera();
            StartCoroutine(AttackRoutine());
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
        IsAttacking = true;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackDuration);

        IsAttacking = false;
    }
}