using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private const float MPS_TO_KPH = 3.6f;

    [SerializeField] private WheelCollider frontLeft, frontRight, rearLeft, rearRight;
    [SerializeField] private Rigidbody carBody;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private FloatProvider speedDataChannel;
    [SerializeField][Range(100f, 500f)] private float maxTorque = 200f;
    [SerializeField][Range(50f, 3000f)] private float maxBreakingTorque = 1000f;
    [SerializeField][Range(10f, 90f)] private float maxSteeringAngle = 45f;
    [SerializeField][CurveRange(0, 0, 400, 1)] private AnimationCurve steeringCurve;
    [SerializeField][CurveRange(0, 2, 250, 12)] private AnimationCurve steeringResponsivenessMult;
    [SerializeField][Range(0f, 1f)] private float rearPowerBias = .75f;
    [Tooltip(".5 is 50/50, larger numbers bias braking towards the rear")]
    [SerializeField][Range(0f, 1f)] private float brakesBias = .5f;
    private float throttleState = 0;
    private float brakesState = 0;
    //positive is right
    private float steeringInput = 0;
    private float currentSteeringAngle = 0;
    private Vector3 currentSpeedVector;
    private float kphSpeed;

    public void OnAccelerationInput(InputAction.CallbackContext context)
    {
        throttleState = context.ReadValue<float>();
    }

    public void OnBrakingInput(InputAction.CallbackContext context)
    {
        brakesState = context.ReadValue<float>();
    }

    public void OnSteeringInput(InputAction.CallbackContext context)
    {
        steeringInput = context.ReadValue<float>();
    }

    private void Start()
    {
        if (centerOfMass)
        {
            carBody.centerOfMass = centerOfMass.localPosition;
        }

        speedDataChannel.SetDataSource(GetKphSpeed);
    }

    private void FixedUpdate()
    {
        currentSpeedVector = carBody.velocity;
        currentSpeedVector.y = 0;
        kphSpeed = currentSpeedVector.magnitude * MPS_TO_KPH;

        float engineTorque = throttleState * maxTorque;
        float brakingTorque = brakesState * maxBreakingTorque;
        engineTorque /= (brakesState + 1f);

        UpdateSteeringAngle();

        ApplyEngineTorqueToWheels(engineTorque);
        ApplyBrakingTorqueToWheels(brakingTorque);
        ApplyRotationToWheels();
    }

    private float GetKphSpeed()
    {
        return kphSpeed;
    }

    private void UpdateSteeringAngle()
    {
        float driftAngle = Vector3.Dot(carBody.transform.right, currentSpeedVector.normalized);
        float driftAmmount = Mathf.Abs(driftAngle);
        bool isCountersteering = Mathf.Abs(steeringInput) > .2f &&
            Mathf.Sign(driftAngle) == Mathf.Sign(steeringInput) &&
            Mathf.Abs(driftAngle - steeringInput) < .5f;

        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, steeringInput * maxSteeringAngle, Time.fixedDeltaTime * steeringResponsivenessMult.Evaluate(kphSpeed));
        if (driftAmmount > .2f && isCountersteering)
        {
            currentSteeringAngle *= Mathf.Lerp(steeringCurve.Evaluate(kphSpeed * .5f), 1f, driftAmmount);
        }
        else
        {
            currentSteeringAngle *= steeringCurve.Evaluate(kphSpeed);
        }
    }

    private void ApplyEngineTorqueToWheels(float engineTorque)
    {
        float steering = (steeringInput + currentSteeringAngle / maxSteeringAngle) * .25f + .5f;
        float leftSideBias = Mathf.Lerp(.8f, 1.2f, steering);
        float rightSideBias = Mathf.Lerp(.8f, 1.2f, 1f - steering);

        rearLeft.motorTorque = engineTorque * rearPowerBias * leftSideBias * (rearLeft.isGrounded ? 1 : .1f);
        rearRight.motorTorque = engineTorque * rearPowerBias * rightSideBias * (rearRight.isGrounded ? 1 : .1f);

        frontLeft.motorTorque = engineTorque * (1f - rearPowerBias) * leftSideBias * (frontLeft.isGrounded ? 1 : .1f);
        frontRight.motorTorque = engineTorque * (1f - rearPowerBias) * rightSideBias * (frontRight.isGrounded ? 1 : .1f);
    }

    private void ApplyBrakingTorqueToWheels(float brakingTorque)
    {
        rearLeft.brakeTorque = brakingTorque * (brakesBias * 2f) * GetAbsBrakeTorqueReductionForWheel(rearLeft);
        rearRight.brakeTorque = brakingTorque * (brakesBias * 2f) * GetAbsBrakeTorqueReductionForWheel(rearRight);

        frontLeft.brakeTorque = brakingTorque * (1 - brakesBias) * 2f * GetAbsBrakeTorqueReductionForWheel(frontLeft);
        frontRight.brakeTorque = brakingTorque * (1 - brakesBias) * 2f * GetAbsBrakeTorqueReductionForWheel(frontRight);
    }

    private void ApplyRotationToWheels()
    {
        frontLeft.steerAngle = currentSteeringAngle;
        frontRight.steerAngle = currentSteeringAngle;

        float curve = steeringCurve.Evaluate(kphSpeed);
        float rearSteeringMultiplier = curve * curve * -.1f;
        rearLeft.steerAngle = currentSteeringAngle * rearSteeringMultiplier;
        rearRight.steerAngle = currentSteeringAngle * rearSteeringMultiplier;
    }

    private float GetAbsBrakeTorqueReductionForWheel(WheelCollider wheel)
    {
        float idealSpeedForWheelRpm = WheelRpmToIdealSpeed(wheel.rpm);
        if (kphSpeed > 1 && idealSpeedForWheelRpm < kphSpeed * .75f && wheel.isGrounded)
        {
            return (idealSpeedForWheelRpm < kphSpeed * .5f ? .1f : 1f) * Mathf.Clamp01(idealSpeedForWheelRpm / kphSpeed);
        }
        else
        {
            return 1f;
        }
    }

    private float WheelRpmToIdealSpeed(float rpm)
    {
        return 2f * Mathf.PI * frontLeft.radius * rpm * (60f / 1000f);
    }
}
