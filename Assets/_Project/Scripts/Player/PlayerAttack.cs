using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public float attack1Duration = 1f;
    public float attack2Duration = 1.5f;
    public float attack2Delay = 0.3f;
    public Transform cameraTransform;
    public InputActionReference attackAction;
    public InputActionReference attack2Action;

    public GameObject attack2Prefab;
    public Transform attack2SpawnPoint;

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
        if (!IsAttacking)
        {
            if (attackAction != null && attackAction.action != null && attackAction.action.WasPressedThisFrame())
            {
                AlignPlayerWithCamera();
                StartCoroutine(AttackRoutine("Attack", false, attack1Duration));
            }
            else if (attack2Action != null && attack2Action.action != null && attack2Action.action.WasPressedThisFrame())
            {
                AlignPlayerWithCamera();
                StartCoroutine(AttackRoutine("Attack2", true, attack2Duration));
            }
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

    private IEnumerator AttackRoutine(string triggerName, bool spawnEffect, float currentAttackDuration)
    {
        IsAttacking = true;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger(triggerName);
        }

        if (spawnEffect && attack2Prefab != null && attack2SpawnPoint != null)
        {
            yield return new WaitForSeconds(attack2Delay);
            Instantiate(attack2Prefab, attack2SpawnPoint.position, attack2SpawnPoint.rotation);
            yield return new WaitForSeconds(currentAttackDuration - attack2Delay);
        }
        else
        {
            yield return new WaitForSeconds(currentAttackDuration);
        }

        IsAttacking = false;
    }
}