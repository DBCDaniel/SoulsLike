using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is only used to test Humanoid Animations & Animation Controllers on Humanoid Models
/// </summary>
public class HumanoidAnimationTester : MonoBehaviour
{
    [Range(-1, 1)]
    public float vertical;
    [Range(-1, 1)]
    public float horizontal;

    public string animationName;

    public string[] oh_attackAnimations;
    public string[] th_attackAnimations;

    public bool isLockedOn;

    public bool playAnim;

    public bool crossfadeAnim;

    public bool useItem;

    public bool isInteracting;

    public bool useAttack;

    public bool isTwoHanded;

    public bool enableRootMotion;

    private Animator anim;

    private int attackIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //used to test one-hand, two-hand toggle
        anim.SetBool("isTwoHanded", isTwoHanded);

        //used to test rootmotion
        enableRootMotion = !anim.GetBool("isMoveable");
        anim.applyRootMotion = enableRootMotion;

        //used to test interactions
        isInteracting = !anim.GetBool("isInteracting");

        //used to test lock movement
        anim.SetBool("isLockedOn", isLockedOn);

        if (!isLockedOn)
        {
            horizontal = 0;
            vertical = Mathf.Clamp01(vertical);
        }

        //used to test use item animation
        if (useItem)
        {
            anim.Play("useItem");
            useItem = false;
        }

        if (isInteracting)
        {
            playAnim = false;
            useAttack = false;
            //clamps movement while doing interations
            vertical = Mathf.Clamp(vertical, 0, 0.5f);
        }

        //used to test animation play and animtion crossfade
        if (playAnim)
        {
            vertical = 0;
            if (crossfadeAnim)
            {
                anim.CrossFade(animationName, 0.2f);
            }
            else
            {
                anim.Play(animationName);
            }
            playAnim = false;
        }

        //Used to test attacks and attack combos
        if (useAttack && enableRootMotion)
        {
            useAttack = false;
        }
        
        if (useAttack && !enableRootMotion)
        {

            vertical = 0;

            attackIndex++;
            
            string targetAnim;
            
            if (isTwoHanded)
            {
                if (attackIndex > th_attackAnimations.Length -1)
                {
                    attackIndex = 0;
                }

                targetAnim = th_attackAnimations[attackIndex];
            }
            else
            {
                if(attackIndex > oh_attackAnimations.Length - 1)
                {
                    attackIndex = 0;
                }

                targetAnim = oh_attackAnimations[attackIndex];
            }

            anim.CrossFade(targetAnim, 0.2f);

            useAttack = false;
        }

        //used to test idle, walk, run & straf
        anim.SetFloat("vertical", vertical);
        anim.SetFloat("horizontal", horizontal);
    }
}
