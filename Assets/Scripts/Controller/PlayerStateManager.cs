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

    [Header("States")]
    public bool isSprinting = false;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public float delta;

    public void Init()
    {
        SetupAnimator();
        
        rb = GetComponent<Rigidbody>();
        rb.angularDrag = 999;
        rb.drag = 4;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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

    public void FixedTick(float d)
    {
        delta = d;

        Movement();
    }

    private void Movement()
    {
        rb.drag = (moveAmount > 0) ? 0 : 4;

        float targetSpeed = movementSpeed;

        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
        }

        rb.velocity = moveDirection * (targetSpeed * moveAmount);
    }
}
