using System.ComponentModel.Design.Serialization;
using System.Threading;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 12f;
    [SerializeField] private float gravity = -9.8f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 velocity;

    private Rigidbody rb;
    private bool isGrounded;

    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

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
            Jump();
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

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, 0f);
        }
    }


    void Update()
    {
        rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, 0f);
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
    }
}
