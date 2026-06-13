using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotBehaviour : StateMachineBehaviour
{   
    public AudioClip soundToPlay;
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false, playContinuously = false;

    // Delayed sound timer
    public float playDelay = 0.25f;
    private float timeSinceEntered = 0;
    private bool hasDelayedSoundPlayed = false;

    // Continuous sound timer
    public float repeatInterval = 0.5f;
    private float timeSinceLastPlay = 0;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter)
        {
            AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, Mathf.Clamp01(volume));
        }


        timeSinceEntered = 0f;
        timeSinceLastPlay = 0f;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeSinceEntered += Time.deltaTime;
        timeSinceLastPlay += Time.deltaTime;

        // Handle delayed sound
        if (playAfterDelay && !hasDelayedSoundPlayed)
        {
            if (timeSinceEntered > playDelay)
            {
                AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, Mathf.Clamp01(volume));
                hasDelayedSoundPlayed = true;
            }
        }

        // Handle continuous sound
        if (playContinuously)
        {
            if (timeSinceLastPlay >= repeatInterval)
            {
                AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, Mathf.Clamp01(volume));
                timeSinceLastPlay = 0f;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit)
        {
            AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, Mathf.Clamp01(volume));
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
