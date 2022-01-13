using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CameraPosition defaultPos;
    public CameraPosition targetPosition {get; set;}
    public bool Def {get; set;}
    private CameraPosition currentPosition {get; set;}
    private CameraPosition previousPosition {get; set;}
    private Camera mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = defaultPos;
        currentPosition = defaultPos;
        Def = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        previousPosition = currentPosition;
        if (!Def){
            currentPosition = targetPosition;
        }
        else {
            currentPosition = defaultPos;
        }
        if (currentPosition.lookAt){
            currentPosition.FollowObject();
        }
        if ((currentPosition != previousPosition) && (!currentPosition.sharpCut)){
                StartCoroutine(LerpFromTo(previousPosition, currentPosition, currentPosition.speed, mainCamera));
            }
        else {
            mainCamera.transform.position = currentPosition.transform.position;
            mainCamera.transform.rotation = currentPosition.transform.rotation;
            mainCamera.fieldOfView = currentPosition.FOV;
        }
    }

    public void EnterZone(CameraPosition position){
        targetPosition = position;
    }

    IEnumerator LerpFromTo(CameraPosition previousPosition, CameraPosition currentPosition, float speed, Camera camera) {
        for (float t=0f; t<speed; t += Time.deltaTime) {
            camera.transform.position = Vector3.Lerp(previousPosition.transform.position, currentPosition.transform.position, t / speed);
            camera.transform.rotation = Quaternion.Lerp(previousPosition.transform.rotation, currentPosition.transform.rotation, t / speed);
            camera.fieldOfView = Mathf.Lerp(previousPosition.FOV, currentPosition.FOV, t / speed);
            yield return 0;
        }
    }


}
