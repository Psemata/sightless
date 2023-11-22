using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTurn : StateMachineBehaviour
{

    private CharacterController2D script;
    private Transform karna;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (script == null)
        {
            this.script = animator.GetComponent<CharacterController2D>();
            this.karna = animator.transform;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        script.m_IsFlipping = false;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = karna.localScale;
        theScale.x *= -1;
        karna.localScale = theScale;
        // The scale for the F touch key
        theScale = this.script.touchKey.transform.localScale;
        theScale.x *= -1;
        this.script.touchKey.transform.localScale = theScale;

        this.script.FlipCorrection();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
