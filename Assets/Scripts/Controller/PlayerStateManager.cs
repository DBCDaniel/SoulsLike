using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    
    [Header("Init")]
    public GameObject activeModel;
    
    [Header("Inputs")]
    public float vertical, horizontal, moveAmount;
    public Vector3 moveDirection;

    [Header("Stats")]
    public float movementSpeed = 2;
    public float sprintSpeed = 3.75f;
    public float rotationSpeed = 5f;
    public float distanceToGround = 0.5f;
    public float groundSnappingDistance = 0.3f;

    [Header("States")]
    public bool isSprinting = false;
    public bool isGrounded = false;
    public bool isLockedOn = false;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public float deltaTime;
    [HideInInspector]
    public LayerMask ignoredLayers;

    public void Init()
    {
        SetupAnimator();
        
        rb = GetComponent<Rigidbody>();
        rb.angularDrag = 999;
        rb.drag = 4;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        gameObject.layer = 8;
        ignoredLayers = ~(1 << 9);

        anim.SetBool("isGrounded", true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupAnimator()
    {
        if (activeModel == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.Log("No model found");
            }
            else
            {
                activeModel = anim.gameObject;
            }
        }

        if (anim == null)
        {
            anim = activeModel.GetComponent<Animator>();
        }
    }

    public void FixedTick(float deltaTime)
    {
        this.deltaTime = deltaTime;

        HandleMovement();
        HandleRotation();
        HandleMovementAnimations();
    }

    public void Tick(float deltaTime)
    {
        this.deltaTime = deltaTime;

        isGrounded = CheckGrounded(); 
        anim.SetBool("isGrounded", isGrounded);

    }

    public bool CheckGrounded()
    {
        bool r = false;

        Vector3 origin = transform.position + (Vector3.up * distanceToGround);
        Vector3 dir = -Vector3.up;
        float castDistance = distanceToGround + groundSnappingDistance;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, castDistance))
        {
            r = true;
            Vector3 targetPosition = hit.point;
            transform.position = targetPosition;
        }

        return r;
    }

    private void HandleMovement()
    {
        rb.drag = (moveAmount > 0 || !isGrounded) ? 0 : 4;

        float targetSpeed = movementSpeed;

        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
            isLockedOn = false;
        }

        if (isGrounded)
        {
            rb.velocity = moveDirection * (targetSpeed * moveAmount);
        }

    }

    private void HandleRotation()
    {
        if (!isLockedOn)
        {
            Vector3 targetDir = moveDirection;
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }

            Quaternion targetLookRotation = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, targetLookRotation, deltaTime * moveAmount * rotationSpeed);
            transform.rotation = targetRotation;
        }

    }

    private void HandleMovementAnimations()
    {
        anim.SetBool("isSprinting", isSprinting);
        anim.SetBool("isLockedOn", isLockedOn);
        anim.SetFloat("vertical", moveAmount, 0.4f, deltaTime);
    }
}
