using System;
using UnityEngine;

public class CarSmartDifferentials : MonoBehaviour
{
    [SerializeField] private WheelCollider frontLeft, frontRight, rearLeft, rearRight;
    [SerializeField][Range(0f, 1f)] private float rearPowerBias = .75f;

    /// <summary>
    /// Distribute torque to wheels based on steering angle and ground contact
    /// </summary>
    /// <param name="torque">Input torque</param>
    /// <param name="signedNormalizedSteeringAngle"> Steering angle normalized from -1 to 1</param>
    /// <param name="steeringInput">Stering input, must range from -1 to 1</param>
    public void ApplyTorqueToWheels(float torque, float signedNormalizedSteeringAngle, float steeringInput)
    {
        float steering = (steeringInput + signedNormalizedSteeringAngle) * .25f + .5f;
        float leftSideBias = Mathf.Lerp(.8f, 1.2f, steering);
        float rightSideBias = Mathf.Lerp(.8f, 1.2f, 1f - steering);

        rearLeft.motorTorque = torque * rearPowerBias * leftSideBias * (rearLeft.isGrounded ? 1 : .1f);
        rearRight.motorTorque = torque * rearPowerBias * rightSideBias * (rearRight.isGrounded ? 1 : .1f);

        frontLeft.motorTorque = torque * (1f - rearPowerBias) * leftSideBias * (frontLeft.isGrounded ? 1 : .1f);
        frontRight.motorTorque = torque * (1f - rearPowerBias) * rightSideBias * (frontRight.isGrounded ? 1 : .1f);
    }

    public float GetAverageWheelRPM()
    {
        float[] rpms = { frontLeft.rpm, frontRight.rpm, rearLeft.rpm, rearRight.rpm };
        Array.Sort(rpms);
        return (rpms[1] + rpms[2]) * 0.5f;
    }
}
