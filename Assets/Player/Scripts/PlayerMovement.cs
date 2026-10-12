using System.ComponentModel.Design.Serialization;
using System.Threading;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 40f;
    [SerializeField] private float gravity = -9.8f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 velocity;

    private Rigidbody rb;
    private bool isGrounded;

    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;  

    public float fallMultiplier = 4.0f;
    public float lowJumpMultiplier = 3.5f; 

    public float midairRotationSpeed = 140f; // How fast they tilt in midair
    public float maxLeanAngle = 45f; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate; 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 

        isGrounded = true;

        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => {
            moveInput = ctx.ReadValue<Vector2>();
            if (moveInput.x > 0) {moveInput.x = 1;}
        };
       
        controls.Gameplay.Move.canceled += ctx => {
            moveInput = Vector2.zero;
            UnityEngine.Debug.Log("movement stopped");
        };


        controls.Gameplay.Jump.started += ctx => {
            jumpBufferCounter = jumpBufferTime;
        };
    }

     void OnEnable()
    {
        if (controls != null)
        {
            controls.Gameplay.Enable();
            UnityEngine.Debug.Log("enabled");
        }
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Gameplay.Disable();
            UnityEngine.Debug.Log("disabled");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }


    void Update()
    {
        float targetYVelocity = rb.linearVelocity.y;
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
        if (jumpBufferCounter > 0f && isGrounded)
        {
            targetYVelocity = jumpHeight;
            jumpBufferCounter = 0;
            
        }
        rb.linearVelocity = new Vector3(moveInput.x * speed, targetYVelocity, 0f);
        jumpBufferCounter -= Time.deltaTime;
        ModifyPhysicsGravity();
    }

    void ModifyPhysicsGravity()
    {   
    
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    
        else if (rb.linearVelocity.y > 0 && jumpBufferCounter<=0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    
}
