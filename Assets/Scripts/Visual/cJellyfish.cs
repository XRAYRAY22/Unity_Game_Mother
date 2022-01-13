using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class cJellyfish : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;
    private Rigidbody body;

    [SerializeField] private float bobTimer;
    [SerializeField] private float bobStrength;
    [SerializeField] private float stayAbove;
    [SerializeField] private Transform packLeader;

    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetComponent<Rigidbody>();
        // randomise size
        transform.parent.localScale = (packLeader == null) ? Random.Range(1.6f, 2.0f) * Vector3.one : Random.Range(1.1f, 1.5f) * Vector3.one;

        // bob up every x seconds
        InvokeRepeating("Bob", Random.value * bobTimer, bobTimer);
    }

    void Update()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            Vector3 previous;
            if (i == 0) previous = transform.position;
            else previous = parts[i - 1].transform.position;

            if (Vector3.Distance(parts[i].transform.position, previous) > 1.8f) parts[i].transform.position = Vector3.MoveTowards(parts[i].transform.position, previous, 0.15f);
        }
    }

    void Bob()
    {

        Vector3 force = Vector3.zero;

        // get random horizontal movement
        Vector2 xz = Random.insideUnitCircle;
        force.x = xz.x;
        force.z = xz.y;

        // or move towards pack leader
        if (packLeader != null)
        {
            force = (packLeader.position - transform.position).normalized;
            force.y += 0.5f; // slightly counteract gravity
        }

        else force.y = 10.0f; // increase upwards weighting 10:1 for pack leader

        force = force.normalized * bobStrength;

        // if below specified y-value, add extra upwards force to pack leader
        if (packLeader == null) // pack leader case
        {
            if (transform.position.y < stayAbove) force *= 2f;
            else force *= 0.5f;
        }
        else // follower case
        {
            force.y = (force.y > 0) ? force.y * 1.4f : force.y * 0.4f; // stop follower jellies dropping like the hindenburg
        }


        body.AddForce(force, ForceMode.Impulse);
    }
}
