using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public bool LockedOn = false;

    public float followSpeed = 9;
    public float mouseSpeed = 2;
    public float controllerSpeed = 7;

    public Transform target;

    [HideInInspector]
    public Transform pivot;
    [HideInInspector]
    public Transform cameraTransform;

    private float turnSmoothing = 0.1f;
    public float minAngle = -35;
    public float maxAngle = 35;

    private Vector2 smooth;
    private Vector2 smoothVelocity;

    [SerializeField]
    private float lookAngle;
    [SerializeField]
    private float tiltAngle;

    public void Init(Transform t)
    {
        target = t;

        cameraTransform = Camera.main.transform;
        pivot = cameraTransform.parent;
    }

    public void Tick(float deltaTime)
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        float controllerHorizontal = Input.GetAxis("Right Axis X");
        float controllerVertical = Input.GetAxis("Right Axis Y");

        float targetSpeed = mouseSpeed;

        if (controllerHorizontal != 0 || controllerVertical != 0)
        {
            horizontal = controllerHorizontal;
            vertical = controllerVertical; 
            targetSpeed = controllerSpeed;
        }

        FollowTarget(deltaTime);
        HandleRotations(deltaTime, vertical, horizontal, targetSpeed);
    }

    private void FollowTarget(float deltaTime)
    {
        float speed = deltaTime * followSpeed;
        Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
        transform.position = targetPosition;
    }

    private void HandleRotations(float deltaTime, float vertical, float horizontal, float targetSpeed)
    {
        if (turnSmoothing > 0)
        {
            smooth.x = Mathf.SmoothDamp(smooth.x, horizontal, ref smoothVelocity.x, turnSmoothing);
            smooth.y = Mathf.SmoothDamp(smooth.y, vertical, ref smoothVelocity.y, turnSmoothing);
        }
        else
        {
            smooth.x = horizontal;
            smooth.y = vertical;
        }

        if (LockedOn)
        {

        }

        lookAngle += smooth.x * targetSpeed;
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);

        tiltAngle -= smooth.y * targetSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
    }
}
