using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    float vertical, horizontal, deltaTime;

    bool mouseLeft_Input;//attack
    bool mouseMiddle_Input;//[+]lock on toggle
    bool mouseRight_Input;//heavy attack
    float mouseScroll_Input;//up spell swap & down item swap

    bool shift_Input; //[+]sprint
    bool leftAlt_Input; //not used
    bool leftCtrl_Input; //[+]parry
    bool tab_Input; //[+]lock on toggle

    bool keypad7_Input;//attack
    bool keypad9_Input;//heavy attack
    bool q_Input;//[+]one-hand/two-hand toggle
    bool f_Input;//[+]block
    bool r_Input;//item use
    bool space_Input;//Dash/Roll/Backstep

    bool b_Input;//[+]sprint
    bool a_Input;
    bool x_Input;//item use
    bool y_Input;//one-hand/two-hand toggle

    bool rb_Input;//attack
    float rt_Input;//heavy attack
    bool r3_Input;//[+]lock on toggle

    bool lb_Input;//[+]block
    float lt_Input;//[+]parry
    bool l3_Input;//not used

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

        mouseLeft_Input = Input.GetButton("Mouse Left");
        mouseMiddle_Input = Input.GetButton("Mouse Middle");
        mouseRight_Input = Input.GetButton("Mouse Right");

        shift_Input = Input.GetButton("Shift Input");
        leftAlt_Input = Input.GetButton("Left Alt Input");
        leftCtrl_Input = Input.GetButton("Left Ctrl Input");
        tab_Input = Input.GetButton("Tab Input");
        keypad7_Input = Input.GetButton("Keypad7 Input");
        keypad9_Input = Input.GetButton("Keypad9 Input");
        f_Input = Input.GetButton("F Input");
        q_Input = Input.GetButton("Q Input");
        
        b_Input = Input.GetButton("B Input");
        a_Input = Input.GetButton("A Input");
        x_Input = Input.GetButton("X Input");
        y_Input = Input.GetButton("Y Input");

        lb_Input = Input.GetButton("LB Input");
        rb_Input = Input.GetButton("RB Input");

        l3_Input = Input.GetButton("L3 Input");
        r3_Input = Input.GetButton("R3 Input");

        //*Note(Windows)* LT should use 9th joystick axis 
        //and RT should use 10th if independed use is needed
        // if not then the 3rd axis will do fine for both LT and RT
        lt_Input = Input.GetAxis("LT Input");
        rt_Input = Input.GetAxis("RT Input");
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

        if (b_Input || shift_Input)
        {
            stateManager.isSprinting = (stateManager.moveAmount > 0);
        }
        else
        {
            stateManager.isSprinting = false;
        }
        
        stateManager.toogleTwoHand = (y_Input || q_Input) ? true : false;

        stateManager.toogleLockOn = (r3_Input || mouseMiddle_Input || tab_Input) ? true : false;

        stateManager.attackPressed = (rb_Input || keypad7_Input || mouseLeft_Input) ? true : false;
        stateManager.heavyAttackPressed = (rt_Input != 0 || keypad9_Input || mouseRight_Input) ? true : false;
        stateManager.blockPressed = (lb_Input || f_Input) ? true : false;
        stateManager.parryPressed = (lt_Input != 0 || leftCtrl_Input) ? true : false;
    }
}
