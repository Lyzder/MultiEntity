using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionNotifier : StateMachineBehaviour
{
    public static event Action OnChangeExit;
    public static event Action OnAttackExit;
    public static event Action OnDamageExit;

    // Called when the state machine starts transitioning out of the state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Atacar"))
            OnAttackExit?.Invoke();
        else if (stateInfo.IsName("Int Transition") || stateInfo.IsName("Dep Transition") || stateInfo.IsName("Base Transition"))
            OnChangeExit?.Invoke();
        else if (stateInfo.IsName("Hurt Int") || stateInfo.IsName("Hurt Dep") || stateInfo.IsName("Hurt Base"))
            OnDamageExit?.Invoke();
    }

}
