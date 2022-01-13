using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class cUIButton : MonoBehaviour
{
    public UnityEvent clickEvent;
    public Camera camera;

    [Header("Animation")]
    [SerializeField] private Vector3 clickScale;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float animationSpeed;
    [SerializeField] private Color highlightColour;

    private Collider collider;
    private TextMeshPro text;
    private Color initialColour;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private bool clicked = false;

    void Awake()
    {
        collider = transform.GetComponent<Collider>();
        text = transform.GetComponent<TextMeshPro>();
        initialColour = text.color;
        initialScale = transform.localScale;
        targetScale = initialScale;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == this.collider)
            {
                if (text != null) text.color = highlightColour;

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //Debug.Log("Click down");
                    targetScale = clickScale;
                    clicked = true;
                }

                // only activate if click is also released on button
                else if (clicked && Input.GetKeyUp(KeyCode.Mouse0))
                {
                    //Debug.Log("Click up (on)");
                    targetScale = initialScale;
                    clicked = false;
                    clickEvent.Invoke();
                }
            }

            // allow release off button to cancel press
            else if (clicked && Input.GetKeyUp(KeyCode.Mouse0))
            {
                //Debug.Log("Click up (off)");
                targetScale = initialScale;
                text.color = initialColour;
                clicked = false;
            }

            else text.color = initialColour;
        }

        transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
    }
}
