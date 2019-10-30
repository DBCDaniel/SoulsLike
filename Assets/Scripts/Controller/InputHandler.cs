using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    float vertical, horizontal, deltaTime;
    bool sprintInput;

    PlayerStateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        stateManager.Init();

        CameraManager.Instance.Init(this.transform);
    }

    // Update is called once per frame

    void Update()
    {
        deltaTime = Time.deltaTime;
        stateManager.Tick(deltaTime);
    }

    private void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime;
        GetInput();
        UpdateStates();
        stateManager.FixedTick(deltaTime);
        CameraManager.Instance.Tick(deltaTime);
    }

    private void GetInput()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        sprintInput = Input.GetButton("Sprint Input");
    }

    private void UpdateStates()
    {
        stateManager.horizontal = horizontal;
        stateManager.vertical = vertical;

        Vector3 v = vertical * CameraManager.Instance.transform.forward;
        Vector3 h = horizontal * CameraManager.Instance.transform.right;
        stateManager.moveDirection = (v + h).normalized;
        float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        stateManager.moveAmount = Mathf.Clamp01(m);

        if (sprintInput)
        {
            stateManager.isSprinting = (stateManager.moveAmount > 0);
        }
        else
        {
            stateManager.isSprinting = false;
        }
    }
}
