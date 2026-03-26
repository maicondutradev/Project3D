using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float sensitivity = 0.5f;
    public float pitchMin = -40f;
    public float pitchMax = 80f;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    public bool invertX = false;
    public bool invertY = false;

    public InputActionReference lookAction;

    private float yaw = 0f;
    private float pitch = 0f;
    private Transform lockOnTarget;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (lockOnTarget != null)
        {
            Vector3 directionToTarget = (lockOnTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            pitch = lookRotation.eulerAngles.x;
            yaw = lookRotation.eulerAngles.y;

            if (pitch > 180f) pitch -= 360f;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }
        else if (lookAction != null && lookAction.action != null)
        {
            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            float inputX = invertX ? -lookInput.x : lookInput.x;
            float inputY = invertY ? lookInput.y : -lookInput.y;

            yaw += inputX * sensitivity;
            pitch += inputY * sensitivity;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = target.position + targetOffset;
        Vector3 position = targetPosition - (rotation * Vector3.forward * distance);

        transform.position = position;
        transform.rotation = rotation;
    }

    public void SetLockOnTarget(Transform newTarget)
    {
        lockOnTarget = newTarget;
    }

    public void ClearLockOnTarget()
    {
        lockOnTarget = null;
    }
}