using UnityEngine;

public class VisualWheelUpdater : MonoBehaviour
{
    [SerializeField] private WheelCollider wheelCollider;
    private Vector3 newWheelPos;
    private Quaternion newWheelRot;
    private Vector3 posVector = new Vector3();

    private void Start()
    {
        TryGetParentWheelCollider();
    }

    private void Update()
    {
        wheelCollider.GetWorldPose(out newWheelPos, out newWheelRot);
        transform.position = newWheelPos;
        posVector.y = transform.localPosition.y;
        transform.localPosition = posVector;

        transform.rotation = newWheelRot;
    }

    private void OnValidate()
    {
        TryGetParentWheelCollider();
    }

    private void TryGetParentWheelCollider()
    {
        if (!wheelCollider)
        {
            wheelCollider = transform.parent.GetComponent<WheelCollider>();
        }
    }
}
