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
    public bool toogleLockOn, lockOnWasToogled;
    public bool toogleTwoHand, twoHandWasToogled;
    public bool attackPressed, heavyAttackPressed, blockPressed, parryPressed;

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
    public bool isTwoHanded = false;
    public bool isMoveable = true;
    public bool isInAction = false;
    
    [Header("Animation Names")]
    public string[] oh_attackAnimations;
    public string[] th_attackAnimations;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public AnimatorHook animHook;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public float deltaTime;
    [HideInInspector]
    public LayerMask ignoredLayers;
    [HideInInspector]
    public float actionDelay;

    public void Init()
    {
        SetupAnimator();
        
        rb = GetComponent<Rigidbody>();
        rb.angularDrag = 999;
        rb.drag = 4;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animHook = activeModel.AddComponent<AnimatorHook>();
        animHook.Init(this);

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
        
       DetectAction();

       if (isInAction)
       {

            anim.applyRootMotion = true;
            actionDelay += deltaTime;

            if (actionDelay > 0.3f)
            {
                isInAction = false;
                actionDelay = 0;
            }
            else
            {
                return;
            }

       }

       isMoveable = anim.GetBool("isMoveable");
                
       if (!isMoveable)
       {
            return;
       }

       anim.applyRootMotion = false;
                    
       HandleMovement();
       HandleRotation();
       HandleMovementAnimations();
    }

    public void DetectAction()
    {
        if (!isMoveable)
        {
            return;
        }

        if (!attackPressed && !heavyAttackPressed && !blockPressed && !parryPressed)
        {
            return;
        }

        string targetAnim = null;

        if (attackPressed)
        {
            targetAnim = "ohAttack1";
        }

        if (heavyAttackPressed)
        {
            targetAnim = "ohAttack2";
        }

        if (blockPressed)
        {
            targetAnim = "ohAttack3";
        }

        if (parryPressed)
        {
            targetAnim = "thAttack2";
        }

        if (!string.IsNullOrEmpty(targetAnim))
        {
            isMoveable = false;
            isInAction = true;
            anim.CrossFade(targetAnim, 0.2f);
        }

    }

    public void Tick(float deltaTime)
    {
        this.deltaTime = deltaTime;

        isGrounded = CheckGrounded();
        isTwoHanded = CheckIsTwoHanded();
        isLockedOn = CheckLockOn();

    }

    public bool CheckLockOn()
    {
        bool r = isLockedOn;

        if (toogleLockOn && !lockOnWasToogled)
        {
            r = !isLockedOn;
            lockOnWasToogled = !lockOnWasToogled;
        }

        if (!toogleLockOn && lockOnWasToogled)
        {
            lockOnWasToogled = !lockOnWasToogled;
        }

        return r;
    }

    public bool CheckIsTwoHanded()
    {
        bool r = isTwoHanded;

        if (toogleTwoHand && !twoHandWasToogled)
        {
            r = !isTwoHanded;
            twoHandWasToogled = !twoHandWasToogled;
        }

        if (!toogleTwoHand && twoHandWasToogled)
        {
            twoHandWasToogled = !twoHandWasToogled;
        }

        return r;
    }

    public bool CheckGrounded()
    {
        bool r = false;

        Vector3 origin = transform.position + (Vector3.up * distanceToGround);
        Vector3 dir = -Vector3.up;
        float castDistance = distanceToGround + groundSnappingDistance + distanceToGround;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(origin.x, origin.y + distanceToGround, origin.z), dir, out hit, castDistance, ignoredLayers))
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
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
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSprinting", isSprinting);
        anim.SetBool("isTwoHanded", isTwoHanded);
        anim.SetBool("isLockedOn", isLockedOn);
        anim.SetFloat("vertical", moveAmount, 0.4f, deltaTime);
    }
}
