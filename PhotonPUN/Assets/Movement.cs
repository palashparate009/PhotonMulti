using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float walkSpeed = 8f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;

    [Space]
    public float airControl = 0.5f;

    [Space]
    public float jumpHeight = 5f;

    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;

    private bool grounded = false;

    /// Dash variables
    [Space]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;

    private bool dashing = false;
    private float dashTime;
    private float nextDashTime;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");

        if (Input.GetKey(KeyCode.E) && Time.time >= nextDashTime)
        {
            Debug.Log("dash is activating");
            StartDash();
        }

    }

    private void StartDash()
    {
        dashing = true;
        dashTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }
    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }
    private void FixedUpdate()
    {
        if (dashing)
        {
            if (Time.time < dashTime)
            {
                rb.velocity = CalculateDashMovement();
            }
            else
            {
                dashing = false;
            }
        }
        else
        {
            if (grounded)
            {
                if (jumping)
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
                }
                else if (input.magnitude > 0.5f)
                {
                    rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);

                }
                else
                {
                    var velocity1 = rb.velocity;
                    velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                    rb.velocity = velocity1;
                }
            }
            else
            {
                if (input.magnitude > 0.5f)
                {
                    rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * airControl : walkSpeed * airControl), ForceMode.VelocityChange);

                }
                else
                {
                    var velocity1 = rb.velocity;
                    velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                    rb.velocity = velocity1;
                }
            }
            grounded = false;
        }
    }

    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x,0,input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;

        Vector3 velocity = rb.velocity;


        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = 0;

            return (velocityChange);
        }
        else
        {
            return new Vector3();
        }
    }
    Vector3 CalculateDashMovement()
    {
        Vector3 dashVelocity = new Vector3(input.x, 0, input.y);
        dashVelocity = transform.TransformDirection(dashVelocity);
        dashVelocity *= dashSpeed;

        return new Vector3(dashVelocity.x, rb.velocity.y, dashVelocity.z);
    }

    ///////////////////////////////////////////////////////////////////////////////////
}
