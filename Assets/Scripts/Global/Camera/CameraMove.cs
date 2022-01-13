using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public CameraPosition cameraPositionFrom;
    public CameraPosition cameraPositionTo;
    public float wait_1;
    private bool complete;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = cameraPositionFrom.transform.position;
        mainCamera.transform.rotation = cameraPositionFrom.transform.rotation;
        complete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (wait_1 > 0){
          wait_1 -= Time.deltaTime;
        }
        if (wait_1 <=0 && !complete){
            StartCoroutine(LerpFromTo(cameraPositionFrom, cameraPositionTo, cameraPositionFrom.speed, mainCamera));
        }
    }

    IEnumerator LerpFromTo(CameraPosition currentPosition, CameraPosition nextPosition, float speed, Camera camera) {
        for (float t=0f; t<speed; t += Time.deltaTime) {
            camera.transform.position = Vector3.Lerp(currentPosition.transform.position, nextPosition.transform.position, t / speed);
            camera.transform.rotation = Quaternion.Slerp(currentPosition.transform.rotation, nextPosition.transform.rotation, t / speed); 
            camera.fieldOfView = camera.fieldOfView + (camera.fieldOfView - nextPosition.FOV) * (t / speed);
            yield return 0;
            complete = true;
            //mainCamera.transform.position = cameraPositionTo.transform.position;
            //mainCamera.transform.rotation = cameraPositionTo.transform.rotation;
        }
    }
}
