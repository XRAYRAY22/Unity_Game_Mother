using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class c3rdMovement : MonoBehaviour
{
    private CharacterController controller;

    // MOVE SPEED
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8;
    [SerializeField] private float startSmoothing = 0.5f;
    [SerializeField] private float stopSmoothing = 0.2f;
    [SerializeField] private float sprintModifier = 1.8f;

    // JUMP VARIABLES
    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float maxJumpCharge = 2f;
    [SerializeField] private float jumpChargeSpeed = 0.1f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float glideStrength = 3.0f;

    // JUMP CHECKS
    [Header("Jump Checking")]
    [SerializeField] private Transform castFrom;
    [SerializeField] private float groundDistance = 0.4f;

    [Header("Sliding")]
    [SerializeField] private float slideAngle;
    [SerializeField] private float slideStrength;
    [SerializeField] private float slideSmoothing;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float uphillSlide;
    [SerializeField] private float counterSlopeBounce = -4.0f;

    // ADVANCED MOVEMENT
    [Header("Additional Options")]
    [SerializeField] private float tiltAngle = 15f;
    [SerializeField] private Transform shrinkable;
    [SerializeField] private Vector3 shrinkScale;
    [Range(0.0f, 0.5f)]
    [SerializeField] private float bobIntensity = 0.2f;
    [Range(0.0f, 0.1f)]
    [SerializeField] private float bobWavelength = 1;

    // PARTICLES
    [Header("Particles")]
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem impact;

    // SOUND
    [Header("Sound")]
    [SerializeField] private AudioSource jump;
    [SerializeField] private AudioSource land;

    // ALERTS
    [Header("Debug Alerts")]
    [SerializeField] private bool debug;

    // general
    Vector3 velocity;
    [SerializeField] private Vector3 baseVelocity;
    float g;
    RaycastHit hit;
    bool hasHit;
    bool isGrounded;

    // movement
    float moveVelocity = 0.0f;
    float smoothing;
    float currentSpeed = 0.0f;
    float targetSpeed = 0.0f;

    // jumping
    float jumpSpeed = 0.0f;
    float jumpTarget = 0.0f;
    float jumpVelocity = 0.0f;

    // sliding
    Vector3 currentSlide;
    Vector3 slideVelocity;

    // other
    bool gliding = false;
    bool canDoubleJump = false;
    float airTime;
    float jumpForce = 0.0f;
    Vector3 currentScale;
    Vector3 targetScale;
    Vector3 shrinkableInitialPosition;

    // sideways movement
    float currentSideSpeed;
    float targetSideSpeed;
    float sideVelocity;

    // Third person
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    Vector2 rotation = Vector2.zero;


    void Start()
    {
        controller = transform.GetComponent<CharacterController>();

        shrinkableInitialPosition = shrinkable.localPosition;
    }

// Update is called once per frame
    void Update()
    {
        g = gravity;

        targetSpeed = Input.GetAxis("Vertical") * moveSpeed;
        targetSideSpeed = Input.GetAxis("Horizontal") * moveSpeed / 2;

        if (Input.GetAxis("Vertical") > 0) targetSpeed = moveSpeed;
        else if (Input.GetAxis("Vertical") == 0) targetSpeed = 0.0f;
        else targetSpeed = -moveSpeed;

        // slow down faster than speeding up
        if (targetSpeed < 0.1) smoothing = stopSmoothing;
        else smoothing = startSmoothing;

        // charge jump, crouching and slowing movement
        if (Input.GetButton("Jump"))
        {
            if (isGrounded){
                targetScale = shrinkScale;
            }

            targetSpeed *= 0.8f;
            if (jumpForce < maxJumpCharge) jumpForce += jumpChargeSpeed * Time.deltaTime;
            //else // play a sound cue here?
        }
        else targetScale = Vector3.one;

        // grounded case
        if (isGrounded)
        {
            // call land() upon contact with the ground
            if (airTime > 0 && hasHit)
            {
                Land(hit, airTime);
                airTime = 0.0f;
            }

            if (Input.GetKey(KeyCode.LeftShift)) targetSpeed *= sprintModifier;

            canDoubleJump = true;

            if (Input.GetButtonUp("Jump") && !gliding)
            {
                Jump();
                if (hasHit) CreateDust(hit);
            }

            // if not jumping, hold the player on the slope
            else if (velocity.y < 0) velocity.y = counterSlopeBounce;

            // play particle effect
            if (Input.GetAxis("Vertical") != 0 && hasHit || Input.GetAxis("Horizontal") != 0 && hasHit) CreateDust(hit);
        }


        // in-air case
        else
        {
            // Glide (fall slowly) when holding jump - check for glide start to avoid unintentional jumps
            if (Input.GetButtonDown("Jump") && !canDoubleJump)
            {
                gliding = true;
                //if (cloth != null) cloth.externalAcceleration = windStrength * Vector3.up;
            }
            if (gliding && Input.GetButton("Jump")) g = gravity / glideStrength;

            airTime += Time.deltaTime;

            if (canDoubleJump && Input.GetButtonUp("Jump"))
            {
                Jump();
                canDoubleJump = false;
            }
        }

        // end glide
        if (gliding && Input.GetButtonUp("Jump"))
        {
            gliding = false;
            //if (cloth != null) cloth.externalAcceleration = Vector3.zero;
        }


    }

    // reduce jitter by moving actual movement into FixedUpdate
    void FixedUpdate()
    {
        velocity = baseVelocity;

        // check ground distance
        hasHit = Physics.Raycast(castFrom.position, Vector3.down, out hit);
        isGrounded = hit.distance < groundDistance;

        // get slide force (if valid raycast hit)
        Vector3 slide = (hasHit) ? Slide(hit, isGrounded) : Vector3.zero;

        // adjust velocity contribution from jump
        jumpTarget = Mathf.MoveTowards(jumpTarget, 0.0f, Mathf.Abs((g / 2) * Time.deltaTime));

        // leaning
        float leanAmount = currentSpeed / moveSpeed; // [-2, 2]
        shrinkable.localRotation = Quaternion.Euler(leanAmount * tiltAngle, 0, 0);

        // apply movement
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref moveVelocity, smoothing); // update move force
        currentSideSpeed = Mathf.SmoothDamp(currentSideSpeed, targetSideSpeed, ref sideVelocity, smoothing);
        jumpSpeed = Mathf.SmoothDamp(jumpSpeed, jumpTarget, ref jumpVelocity, smoothing); // update jump force
        shrinkable.localScale = Vector3.MoveTowards(shrinkable.localScale, targetScale, ((1 + moveSpeed * moveSpeed) / 4) * Time.deltaTime); // update scale
        Vector3 movement = currentSpeed * transform.forward + currentSideSpeed * Vector3.Cross(Vector3.up, transform.forward) + jumpSpeed * (2 * leanAmount * transform.forward + jumpHeight * Vector3.up) + slide; // combine and apply forces

        // bob (unless there is enough slide force to justify no bob)
        if (Input.GetAxis("Vertical") != 0 && isGrounded && 3 * slide.magnitude < movement.magnitude)
        {
            Bob();
        }

        velocity += movement;
        velocity.y += g;

        controller.Move(velocity * Time.deltaTime);

        if (debug) Debug.DrawRay(transform.position + 4 * Vector3.up, velocity);

        // Player and Camera rotation
        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
        playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        transform.eulerAngles = new Vector2(0, rotation.y);

    }

    void Jump()
    {
        // minimum jump strength equivalent to charging the jump for 0.5s
        if (jumpForce < jumpChargeSpeed) jumpForce = jumpChargeSpeed / 2;

        jumpSpeed = jumpForce;
        jumpTarget = jumpSpeed;
        jumpForce = 0.0f;

        jump.Play();
    }

    // slide along surface perpendicular to normal
    Vector3 Slide(RaycastHit hit, bool isGrounded)
    {
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        Vector3 downSlope = -Vector3.ProjectOnPlane(Vector3.up, hit.normal); // projection is already scaled by slope steepness
        Debug.DrawRay(transform.position + 4 * Vector3.up, downSlope * 5, Color.green);

        // scale slide power depending on whether player is facing up or downhill
        float strength = slideStrength;// * (((180f - Vector3.Angle(transform.forward, downSlope)) / 180) + uphillSlide); //Mathf.Min(

        // apply modifiers and extra scaling for high angles
        Vector3 targetSlide = (isGrounded && angle > slideAngle) ? strength * (angle / 4) * downSlope : Vector3.zero;

        float smooth = slideSmoothing;
        if (!isGrounded) smooth *= 10; // simulate friction - lose speed slower when in the air

        currentSlide = Vector3.SmoothDamp(currentSlide, targetSlide, ref slideVelocity, smooth);
        Debug.DrawRay(transform.position + 3 * Vector3.up, currentSlide * 5, Color.green);

        return currentSlide;
    }

    void Bob()
    {
        float amplitude = bobIntensity * Mathf.Min((currentSpeed / moveSpeed), 1.4f); // only allow slight increase in bob when sprinting
        float sin = amplitude * Mathf.Sin(Time.time / bobWavelength);

        shrinkable.localPosition = new Vector3(shrinkableInitialPosition.x, shrinkableInitialPosition.y + sin, shrinkableInitialPosition.z);
    }

    void Land(RaycastHit hit, float impactForce)
    {
        Color colour = hit.transform.GetComponent<MeshRenderer>()?.material.color ?? Color.white;

        var particleMain = impact.main;
        particleMain.startColor = colour;
        particleMain.startSize = Mathf.Min(impactForce / 2, 2.0f);
        impact.Play();
        land.Play();

        //shrinkable.localScale = shrinkScale;
    }

    void CreateDust(RaycastHit hit)
    {
        // get hit material's colour, or default to white
        Color colour = hit.transform.GetComponent<MeshRenderer>()?.material.color ?? Color.white;

        // maybe scale particle size to velocity magnitude?

        var particleMain = dust.main;
        particleMain.startColor = colour;
        dust.Play();
    }

    public void SetSlideStrength(float strength)
    {
        this.slideStrength = strength;
    }

    public void SetYVelocity(float vel)
    {
        this.baseVelocity = new Vector3(0, vel, 0);
    }
}
