using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranistionNotifier : StateMachineBehaviour
{
    public static event Action OnAttackExit;

    // Called when the state machine starts transitioning out of the state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnAttackExit?.Invoke();
    }

}
