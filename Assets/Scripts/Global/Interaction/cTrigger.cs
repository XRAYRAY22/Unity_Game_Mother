using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cTrigger : MonoBehaviour
{
    public UnityEvent onActions;
    public UnityEvent offActions;
    public GameObject targetObject;

    [Space(6)]
    public bool enableOffActions = false;

    [Space(6)] [SerializeField]
    private bool showInGame = false;

    [HideInInspector]
    public bool activated = false;

    void Start()
    {
        if (!showInGame) transform.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            if (activated && enableOffActions) this.Deactivate();
            else if (!activated) this.Activate();
        }
    }

    public virtual void Activate()
    {
        onActions.Invoke();
        activated = true;
    }

    public virtual void Deactivate()
    {
        offActions.Invoke();
        activated = false;
    }
}
