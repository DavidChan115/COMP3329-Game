using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is useful for coordinating animations with gameplay logic. For example, you might use it to:
// Set a boolean like isAttacking to true when entering an attack animation and false when exiting.
// Trigger other animations or gameplay states by changing Animator parameters at the right moment.
// Ex. in the state machine in Animator page (Ex. GroundAttack), 
// This script enables you to edit stuffs in the inspector window of a state machine
// Ex. enter Bool Name, check on value On Enter, Update On State, etc.

// Ex. we have a "canMove" parameter, in the GroundAttack state machine:
// 1. Bool Name = canMove    
// Meaning: The canMove boolean likely controls whether the player can move.  
// 2. Update On State Machine = (unchecked)    
// Meaning: the script will not set the canMove boolean when entering or exiting the entire "GroundStates" state machine.
// 3. Update On State = (checked)
// Meaning: the script will set the canMove boolean when entering or exiting the "GroundAttack" state specifically.
// 4. Value On Enter = (unchecked)
// Meaning: When the Animator enters the "GroundAttack" state, the script will set the canMove boolean to false.
// 5. Value On Exit = (checked)
// Meaning: When the Animator exits the "GroundAttack" state, the script will set the canMove boolean to true.

public class SetBoolBehaviour : StateMachineBehaviour
{
    public string boolName;
    public bool updateOnStateMachine;
    public bool updateOnState;
    public bool valueOnEnter, valueOnExit;


    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    // Refer to specific animation behaviour inside the state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnState)
        {
            animator.SetBool(boolName, valueOnEnter);
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    // Refer to specific animation behaviour inside the state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnState)
        {
            animator.SetBool(boolName, valueOnExit);
        }
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    // Refer to the whole state machine
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if(updateOnStateMachine)
        {
            animator.SetBool(boolName, valueOnEnter);
        }
            
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    // Refer to the whole state machine
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if(updateOnStateMachine)
        {
            animator.SetBool(boolName, valueOnExit);
        }
            
    }
}
