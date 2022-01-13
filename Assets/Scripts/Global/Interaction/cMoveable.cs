using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMoveable : MonoBehaviour
{
    [SerializeField]
    private Vector3 targetPosition;
    [SerializeField]
    private Quaternion targetRotation;

    [SerializeField] [Range(0.0f, 30.0f)]
    private float moveTime;

    [SerializeField] [Range(0.0f, 5.0f)]
    private float accelerationMult = 1.0f;

    [SerializeField]
    private bool damped = true;
    [SerializeField]
    private bool accelerate = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Vector3 moveTo;
    private Quaternion rotateTo;
    private bool moving = false;

    private float acceleration = 0.0f;
    private Vector3 currentVelocity;

    private float distance;

    void Start()
    {
        this.initialPosition = transform.position;
        this.initialRotation = transform.rotation;
    }

    public void Move()
    {
        acceleration = 0.0f;
        moving = true;
        moveTo = initialPosition + targetPosition;
        rotateTo = targetRotation;

        distance = Vector3.Distance(transform.position, moveTo);
    }

    public void Unmove()
    {
        acceleration = 0.0f;
        moving = true;
        moveTo = initialPosition;
        rotateTo = initialRotation;

        distance = Vector3.Distance(transform.position, moveTo);
    }

    public void Update()
    {
        float pStep;
        float rStep;

        if (accelerate)
        {
            acceleration += accelerationMult * Time.deltaTime;
            pStep = acceleration + Time.deltaTime * distance / moveTime;
            rStep = acceleration + Time.deltaTime / moveTime * (Quaternion.Angle(transform.rotation, rotateTo) / Quaternion.Angle(initialRotation, rotateTo));
        }
        else
        {
            pStep = Time.deltaTime * distance / moveTime;
            rStep = Time.deltaTime * Quaternion.Angle(transform.rotation, rotateTo) / moveTime;
        }

        if (moving)
        {
            if (damped) transform.position = Vector3.SmoothDamp(transform.position, moveTo, ref currentVelocity, moveTime);
            else transform.position = Vector3.MoveTowards(transform.position, moveTo, pStep);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rStep);
        }
    }
}
