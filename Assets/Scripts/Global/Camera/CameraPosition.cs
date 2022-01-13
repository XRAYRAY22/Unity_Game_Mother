using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public float FOV;
    public bool alwaysOn = false;
    public float drawDistance = 10f;
    [SerializeField]
    private GameObject targetObject;
    public float speed;
    public bool sharpCut;
    public bool lookAt;
    public bool follow;
    public bool rotateWith;
    public Vector3 offset = Vector3.zero;
    public bool autoOffset;
    private Vector3 baseOffset;

    // Start is called before the first frame update
    void Start()
    {
        if (offset == Vector3.zero){
            offset = new Vector3(0, 1, -5);
        }

        if (autoOffset) offset = transform.position - targetObject.transform.position;

        baseOffset = offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (follow) FollowObject();
        if (lookAt) transform.LookAt(targetObject.transform);

        if (rotateWith)
        {
            float rotation = Vector3.Angle(Vector3.forward, targetObject.transform.forward);
            if (targetObject.transform.forward.x < 0) rotation = -rotation;
            offset = Quaternion.Euler(0, rotation, 0) * baseOffset;
        }
    }

    public void FollowObject() {
        if (follow){
            transform.position = targetObject.transform.position + offset;
        }
        //transform.LookAt(targetObject.transform);
    }


    void OnDrawGizmosSelected()
    {
        if (!alwaysOn)
        {
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawFrustum(Vector3.zero, FOV, drawDistance, 0.1f, 1f/1f);
        }
    }

    public void Swing(Vector3 newOffset){
        this.offset = newOffset;
    }

    void OnDrawGizmos()
    {
        if (alwaysOn)
        {
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawFrustum(Vector3.zero, FOV, drawDistance, 0.1f, 1f/1f);
        }
    }



}
