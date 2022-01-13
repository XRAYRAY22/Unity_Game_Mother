using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cContinuousTrigger : cTrigger
{
    [Space(5)]
    [Header("On Activation")]
    [SerializeField]
    private UnityEvent singleActions;
    private bool objectInside;

    public override void Activate()
    {
        objectInside = true;
        singleActions.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            objectInside = false;
            offActions.Invoke();
        }
    }

    void Update()
    {
        if (objectInside) onActions.Invoke();
    }
}
