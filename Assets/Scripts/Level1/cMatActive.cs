using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class cMatActive : MonoBehaviour, IActivate
{
    private Renderer renderer;
    private bool active = false;
    [SerializeField] private Material inactiveMat;
    [SerializeField] private Material activeMat;

    void Start() { renderer = transform.GetComponent<Renderer>(); if (renderer == null) Debug.Log("uh oh"); }

    public bool isActive() { return active; }

    public void Activate()
    {
        renderer.material = activeMat;
        active = true;
    }

    public void Deactivate()
    {
        renderer.material = inactiveMat;
        active = false;
    }

}
