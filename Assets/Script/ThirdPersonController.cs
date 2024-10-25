using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    //Input fields
    private PlayerInput playerInputAsset;
    private InputAction move;
    private Rigidbody rb;
    [SerializeField] private float movementForce = 1f;
    private float jumpforce = 5f;
    [SerializeField] private float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    // Player Camera follow Movement
    [SerializeField] private Camera playerCamera;

    private void Awake()
    {
        // Getting the rigidbody component of the player;
        rb = this.GetComponent<Rigidbody>();
        playerInputAsset = new PlayerInput();
    }

    private void OnEnable()
    {
        // When you press the button and the player is moving the player jumps.
        playerInputAsset.Player.Jump.started += Dojump;
        move = playerInputAsset.Player.Move;
        playerInputAsset.Player.Enable();
    }

    private void OnDisable()
    {
        // When you press the button and the player is not moving the player doesn't jumps.
        playerInputAsset.Player.Jump.started -= Dojump;
        playerInputAsset.Player.Disable();
    }
    private void FixedUpdate()
    {
        // Input system values and connecting it with the camera viewpoint.
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        // This code applies an instant force to a Rigidbody(rb) in the direction specified by forceDirection, using the Impulse mode for a sudden push.
        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        // Allow the player if in the air to fall down as quickly as possible.
        if (rb.velocity.y < 0f)
            rb.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        // Control the Movement velocity.
        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0f;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
    }

    private void LookAt() 
    {
        // Don't Allow the rotation up or down just on the Vertical Axis.
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        // The camera follows the player transform cordinates.
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        // The camera follows the player transform cordinates.
        Vector3 right = playerCamera.transform.right;
        right.y = 0f;
        return right.normalized;
    }

    private void Dojump(InputAction.CallbackContext context)
    {
        // the jumping Mechanic if player on floor then jump.
        if (IsGrounded())
        {
            forceDirection += Vector3.up * jumpforce;
        }
    }

    private bool IsGrounded()
    {
        // the principle to check if player is on the ground :)
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            return true;
        else return false;
       
    }
}
