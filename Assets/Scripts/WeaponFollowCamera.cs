using UnityEngine;

public class WeaponFollowCamera : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Offsets")]
    public Vector3 positionOffset = new Vector3(0.4f, -0.3f, 0.6f);
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Smoothing")]
    public float positionSmoothSpeed = 10f;
    public float rotationSmoothSpeed = 10f;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        Vector3 targetPosition = cameraTransform.position + cameraTransform.TransformDirection(positionOffset);
        Quaternion targetRotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / positionSmoothSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
