using NaughtyAttributes;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    [SerializeField][CurveRange(0, 0, 20000, 2000), Tooltip("In Newton Meters")] private AnimationCurve torqueCurve;
    [SerializeField] private float idleRPM = 1000f;
    [SerializeField] private float maxRPM = 7000f;

    public float CurrentRPM => currentRPM;

    private float currentRPM;
    private float throttleInput;

    public float GetTorqueAtRPM(float rpm)
    {
        rpm = Mathf.Clamp(rpm, idleRPM, maxRPM);
        return torqueCurve.Evaluate(rpm);
    }

    public float GetCurrentTorqueOutput()
    {
        if (currentRPM > .98 * maxRPM)
        {
            return 0f;
        }
        return GetTorqueAtRPM(currentRPM) * throttleInput;
    }

    public void SetThrottleInput(float input)
    {
        throttleInput = Mathf.Clamp01(input);
    }

    public void SetRPM(float crankRPM)
    { 
        currentRPM = Mathf.Clamp(crankRPM, idleRPM, maxRPM);
    }

    private void Start()
    {
        currentRPM = idleRPM;
    }
}
