using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cLightFlicker : MonoBehaviour
{
    private Light light;
    private float initialIntensity;
    [SerializeField] private float amplitude = 10;
    [SerializeField] private float speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        light = transform.GetComponent<Light>();
        initialIntensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = initialIntensity + amplitude * Mathf.Sin(Time.timeSinceLevelLoad * speed);
    }
}
