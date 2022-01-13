using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeOnDeath : MonoBehaviour
{

    [SerializeField]
    private Image fade;
    [SerializeField]
    private Canvas canvas;
    
    private Vector2 size;
    // Start is called before the first frame update
    void Start()
    {
        fade.canvasRenderer.SetAlpha(0.0f);
        size = canvas.renderingDisplaySize;
        fade.rectTransform.sizeDelta = size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fade(float fadetime){
        fade.CrossFadeAlpha(1,fadetime,false);
        
    }

    public void Reset(float fadetime){
        fade.CrossFadeAlpha(0,fadetime,false);
    }
}
