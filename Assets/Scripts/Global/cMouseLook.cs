using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMouseLook : MonoBehaviour
{
    public Transform referenceTransform;

    // Detection disctance to prevent camera from clipping through objects
    public float collisionOffset = 0.3f;
    // Speed camera restores back to position
    public float camSpeed = 15f;

    public bool lockOnStart = false;
    bool lockSwitch = true;

    Vector3 defaultPos;
    Vector3 directionNormalized;
    Transform parentTransform;
    float defaultDistance;

    // layerMask index
    int layerMask = 6;

    void Start()
    {
        if (lockOnStart)
        { 
            LockCursor();
            bool lockSwitch = true;
        }

        defaultPos = transform.localPosition;
        directionNormalized = defaultPos.normalized;
        parentTransform = transform.parent;
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero);
    }

    // Lock and hide cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Unlock and show cursor
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    // This is called after Update
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnlockCursor();
            lockSwitch = false;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if(lockSwitch){
                UnlockCursor();
                lockSwitch = false;
            }
            else {
                LockCursor();
                lockSwitch = true;
            }
        }   

        // Shift the camera towards the parent point when the camera collides with objects
        Vector3 currentPos = defaultPos;
        RaycastHit hit;
        Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - referenceTransform.position;
        if (Physics.SphereCast(referenceTransform.position, collisionOffset, dirTmp, out hit, defaultDistance, ~(1<<layerMask)))
        {
            currentPos = (directionNormalized * (hit.distance - collisionOffset));
            transform.localPosition = currentPos;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * camSpeed);
        }
    }
}
