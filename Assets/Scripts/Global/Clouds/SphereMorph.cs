using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMorph : MonoBehaviour
{
    [SerializeField]
    private float UpperRange = 2f;
    [SerializeField]
    private float LowerRange = 0.01f;
    private float startTime;
    private float length;
    [SerializeField]
    private float speed = 0.5f;
    private Vector3 newScale;
    private Vector3 originalScale;
    private Vector3 startScale;
    private Vector3 velocity = Vector3.zero;
    private float time;


    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
        UpperRange = transform.localScale.x*UpperRange;
        LowerRange = transform.localScale.x*LowerRange;
        originalScale = transform.localScale;
        startScale = originalScale;
        newScale = originalScale*Random.Range(LowerRange,UpperRange);
        length = Vector3.Distance(originalScale, newScale);
        //Material mat = this.GetComponent<MeshRenderer>().material;
        
    }

    // Update is called once per frame
    void Update()
    {
        float distCovered = (Time.time - time)*speed;
        float fractionOfJourney = distCovered/length;
        transform.localScale = Vector3.Lerp(startScale, newScale,fractionOfJourney );
        if (transform.localScale == newScale){
            Reset();
        }
    }

    void Reset(){
        time = Time.time;
        startScale = newScale;
        newScale = originalScale*Random.Range(LowerRange,UpperRange);
        length = Vector3.Distance(transform.localScale, newScale);
    }
}
