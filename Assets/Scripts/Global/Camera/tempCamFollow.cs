using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempCamFollow : MonoBehaviour
{
    public Transform follow;
    public Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = follow.position + position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position + position;
    }
}
