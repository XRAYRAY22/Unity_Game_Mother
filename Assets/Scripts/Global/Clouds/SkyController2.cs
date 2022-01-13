using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CloudGenerator), typeof(PerlinNoise))]
public class SkyController2 : MonoBehaviour
{
    [SerializeField]
    private int xRange;
    [SerializeField]
    private int zRange;
    [SerializeField]
    private float yRange =5f;
    [SerializeField]
    private Transform start;
    [SerializeField, Range(0.9f,1)]
    private float cutOff = 0.9f;
    [SerializeField]
    private int skip = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < xRange*2; i ++){
            if (i%skip != 0){
                continue;
            }
            for(int j = 0; j < zRange*2; j++){
                if(j%skip == 0){
                    if (GetComponent<PerlinNoise>().CalculateColor(i,j).r >= cutOff){
                        //Debug.Log("Perlin");
                        float yOffset = Random.Range(-yRange, yRange);
                        GetComponent<CloudGenerator>().GenerateCloud((new Vector3(start.position.x + (float)i-xRange, transform.position.y + yOffset, start.position.z + (float)j-zRange)));
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
