using UnityEngine;

public class CameraPivotFollowTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;
    public float FollowSpeed = 8f;

    void Start()
    {
        transform.position = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.transform.position;

        transform.position = Damp(transform.position, targetPos, FollowSpeed, Time.deltaTime);
    }

    private static Vector3 Damp(Vector3 current, Vector3 target, float decayConstant, float deltaTime)
    {
        return target + (current - target) * Mathf.Exp(-decayConstant * deltaTime);
    }
}
