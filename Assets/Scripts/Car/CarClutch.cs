using UnityEngine;

public class CarClutch : MonoBehaviour
{
    [SerializeField] private float clutchForceDampingConstant = 10f;

    //When the cluth is engaged, power is transmitted from the engine to the gearbox
    private bool clutchEngaged = true;

    public void EngageClutch()
    {
        clutchEngaged = true;
    }
    public void DisengageClutch()
    {
        clutchEngaged = false;
    }

    public float GetGearboxInputTorque(float torque)
    { 
        return clutchEngaged ? torque : 0f;
    }

    public float GetUpdatedEngineRPM(float engineRPM, float gearboxRPM)
    {
        return Damp(engineRPM, gearboxRPM, clutchForceDampingConstant, Time.deltaTime);
    }

    private static float Damp(float current, float target, float decayConstant, float deltaTime)
    {
        return target + (current - target) * Mathf.Exp(-decayConstant * deltaTime);
    }
}
