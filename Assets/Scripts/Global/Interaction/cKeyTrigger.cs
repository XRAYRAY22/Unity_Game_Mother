using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cKeyTrigger : cTrigger
{
    [SerializeField]
    private KeyCode keyCode = KeyCode.E;
    public bool trigger;

    private bool objectInside = false;

    public override void Activate()
    {
        objectInside = true;
    }

    public override void Deactivate()
    {
        objectInside = true;
    }

    void Update()
    {
        if (objectInside && Input.GetKeyDown(keyCode))
        {
            if (!activated)
            {
                onActions.Invoke(); // invoke actions only when object within zone AND input received AND not already activated
                activated = true;
            }

            else if (enableOffActions)
            {
                offActions.Invoke();
                activated = false;
                trigger = false;
            }
        }
        else if (trigger){
            onActions.Invoke();
            activated = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject) objectInside = false;
    }
}
