using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private int numberOfStates = 0;
    private int currentState = -1;

    private bool stateHasBegun = false;

    private Action[] stateEnterMethod;
    private Action[] stateExitMethod;
    private Func<int>[] stateUpdateMethod;
    private IEnumerator[] stateCoroutine;

    public void Initialise(int states)
    {
        numberOfStates = states;

        stateEnterMethod = new Action[numberOfStates];
        stateExitMethod = new Action[numberOfStates];
        stateUpdateMethod = new Func<int>[numberOfStates];
        stateCoroutine = new IEnumerator[numberOfStates];
    }
    
    public bool Active()
    {
        return numberOfStates > 0 && currentState >= 0;
    }

    public void SetCallbacks(int state, Action enter, Action exit, Func<int> update, IEnumerator coroutine)
    {
        stateEnterMethod[state] = enter;
        stateExitMethod[state] = exit;
        stateUpdateMethod[state] = update;
        stateCoroutine[state] = coroutine;
    }

    public void SetState(int state)
    {
        if (Active())
        {
            stateExitMethod[currentState]?.Invoke();
        }
        currentState = state;
        stateHasBegun = false;
    }
    
    void Update()
    {
        if (Active())
        {
            if (!stateHasBegun)
            {
                stateEnterMethod[currentState]?.Invoke();
                stateHasBegun = true;

                if (stateCoroutine[currentState] != null)
                {
                    StartCoroutine(stateCoroutine[currentState]);
                }
            }

            stateUpdateMethod[currentState]?.Invoke();
        }
    }
}
