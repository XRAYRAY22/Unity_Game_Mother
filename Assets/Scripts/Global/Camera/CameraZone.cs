using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    private CameraController camera;
    public CameraPosition position;
    public CameraPosition secondaryPosition;
    public List<CameraZone> adjacencies;
    public bool on;
    private bool In {get; set;}
    private bool Current {get; set;}


    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.GetComponent(typeof(CameraController)) as CameraController;
        In = false;
        Current = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"){
            foreach (CameraZone zone in adjacencies){
                if (zone.Current == true){
                zone.Current = false;
                }
            }
                In = true;
                camera.EnterZone(position);
                camera.Def = false;
                Current = true;
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player"){
            if (!Current){
                foreach (CameraZone zone in adjacencies){
                    if (zone.Current == true){
                    return;
                    }
                }
                camera.EnterZone(position);
                Current = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player"){
            Current = false;
            In = false;
            foreach (CameraZone zone in adjacencies){
                if (zone.In == true){
                    return;
                }
            }
            camera.Def = true;
        }
    }



}
