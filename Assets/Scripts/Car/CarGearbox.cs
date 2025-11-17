using UnityEngine;

public class CarGearbox : MonoBehaviour
{
    [SerializeField] private float[] gearRatios = new float[5];
    [SerializeField] private float finalDriveRatio = 3.42f;

    private int currentGearIndex = 0;

    public float GetCurrentGearRatio()
    {
        Debug.Assert(gearRatios != null && gearRatios.Length > 0, "Gear ratios array is empty!", this);
        if (currentGearIndex < 0)
        {
            return 0f;
        }
        Debug.Assert(currentGearIndex > -1 && currentGearIndex < gearRatios.Length, "Current gear index is out of bounds!", this);
        return gearRatios[currentGearIndex] * finalDriveRatio;
    }

    public float GetOutputTorque(float inputTorque)
    { 
        return inputTorque * GetCurrentGearRatio();
    }

    public void UpShift()
    {
        if (currentGearIndex < gearRatios.Length - 1)
        {
            currentGearIndex++;
        }
    }
    public void DownShift()
    {
        if (currentGearIndex > -1)
        {
            currentGearIndex--;
        }
    }
    public void SetNeutral()
    {
        currentGearIndex = -1;
    }
}
