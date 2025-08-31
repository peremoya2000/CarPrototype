using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLookatController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float maxPivotRotation = 45f;
    [SerializeField, Range(0, 50)] private float lookDampSpeed = 10f;

    private Quaternion leftMaxRotation, rightMaxRotation;
    private float steeringInput;

    public void OnLookatInput(InputAction.CallbackContext context)
    {
        steeringInput = context.ReadValue<float>();
    }

    private void Start()
    {
        Vector3 leftLook = Vector3.RotateTowards(Vector3.forward, Vector3.left, Mathf.Deg2Rad * maxPivotRotation, 0f);
        Vector3 rightLook = Vector3.RotateTowards(Vector3.forward, Vector3.right, Mathf.Deg2Rad * maxPivotRotation, 0f);
        leftMaxRotation = Quaternion.LookRotation(leftLook, Vector3.up);
        rightMaxRotation = Quaternion.LookRotation(rightLook, Vector3.up);
    }

    private void Update()
    {
        Vector3 targetUp = Vector3.RotateTowards(Vector3.up, target.transform.up, Mathf.PI / 4f, 0f);

        Quaternion targetRotation = Quaternion.Lerp(leftMaxRotation, rightMaxRotation, steeringInput * .5f + .5f) * Quaternion.LookRotation(target.transform.forward, targetUp);

        transform.rotation = DampRotation(transform.rotation, targetRotation, lookDampSpeed, Time.deltaTime);
    }

    private static Quaternion DampRotation(Quaternion current, Quaternion target, float decayConstant, float deltaTime)
    {
        float t = Mathf.Clamp01(1f - Mathf.Exp(-decayConstant * deltaTime));

        float angle = Quaternion.Angle(current, target);
        if (angle <= 0f) return target;

        float maxDegreesDelta = angle * t;

        return Quaternion.RotateTowards(current, target, maxDegreesDelta);
    }
}
