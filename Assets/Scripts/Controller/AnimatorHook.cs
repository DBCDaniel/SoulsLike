using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    Animator anim;
    PlayerStateManager state;
    Vector3 velocity;

    public void Init(PlayerStateManager psm)
    {
        state = psm;
        anim = state.anim;
    }

    //Callback for processing animtion movements for modifying rootmotion
    //This callback will be invoked at each frame after the state machines
        //and the animations have been evaluated, but before OnAnimatorIK.
    void OnAnimatorMove()
    {
        if (state.isMoveable)
        {
            return;
        }

        state.rb.drag = 0;

        float multiplier = 1;

        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;

        velocity = (deltaPosition * multiplier) / state.deltaTime;

        state.rb.velocity = velocity;
    }
}
