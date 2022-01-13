using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPushable : MonoBehaviour
{
    [SerializeField]
    private Vector3 direction;
    [Range(0.0f, 10.0f)] [SerializeField]
    private float speed;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float currentDistance;

    // Start is called before the first frame update
    void Start()
    {
        direction = direction.normalized;
        if (currentDistance > distance) distance = currentDistance;
    }

    public void Push()
    {
        if (currentDistance < distance)
        {
            transform.position += direction * (speed * (distance / 10000));
        }
    }

    public void PushBack()
    {
        if (currentDistance > 0)
        {
            transform.position -= direction * (speed * (distance / 10000));
        }
    }
}
