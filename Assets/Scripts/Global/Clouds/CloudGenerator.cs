using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject cloudMesh;
    [SerializeField]
    private float minSize;
    [SerializeField]
    private float maxSize;
    [SerializeField]
    private float minScale;
    [SerializeField]
    private float maxScale;
    [SerializeField]
    private Material cloudMat;
    [SerializeField]
    private Transform start;
    private Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
    [SerializeField]
    private float cloudSpeed = 0.01f;
    
    // Start is called before the first frame update
    void Start()
    {
        //GenerateCloud();
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - cloudSpeed);
    }

    //method for testing
    public void GenerateCloud(){
        int numSpheres = (int)Random.Range(minSize,maxSize);
        int scalefactor = (int)Random.Range(minScale,maxScale);
        Vector3 scale = new Vector3(scalefactor,scalefactor,scalefactor);
        transform.localScale = scale;
        start = transform;
        GameObject lastSphere;
        lastSphere = Instantiate(cloudMesh, start.position, start.rotation, transform);
        //lastSphere.transform.localScale = scale;
        for (int i = 0; i < numSpheres; i++){
            int direction = (int)Random.Range(0,directions.Length-1);
            lastSphere = Instantiate(cloudMesh, lastSphere.transform.position+(scalefactor/2*directions[direction]), start.rotation, transform);
            //lastSphere.transform.localScale = scale;
        }
        return;
    }

    public void GenerateCloud(Vector3 start){
        int numSpheres = (int)Random.Range(minSize,maxSize);
        float scalefactor = Random.Range(minScale,maxScale);
        GameObject cloud = CreateCloud(start, scalefactor);
        
        GameObject lastSphere;
        lastSphere = Instantiate(cloudMesh, cloud.transform.position, cloud.transform.rotation, cloud.transform);
        for (int i = 0; i < numSpheres; i++){
            int direction = (int)Random.Range(0,directions.Length-1);
            lastSphere = Instantiate(cloudMesh, lastSphere.transform.position+(scalefactor/2*directions[direction]), cloud.transform.rotation, cloud.transform);
        }
        return;
    }

    private GameObject CreateCloud(Vector3 start, float scaleFactor){
        GameObject cloud = new GameObject("cloud");
        cloud.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        cloud.transform.parent = transform;
        cloud.transform.position = start;
        cloud.transform.rotation = Quaternion.identity;
        return cloud;
    }

}
