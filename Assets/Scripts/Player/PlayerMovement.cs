using UnityEngine;
using Assets.Scripts.GameInput;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    internal class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float crouchSpeed = 2f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float gravity = -19.62f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckDistance = 0.3f;

        [Header("References")]
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform orientation;

        // State
        private bool isGrounded;
        private bool isRunning;
        private bool isCrouching;
        private Vector3 velocity;

        // Events
        public event System.Action<bool> OnCrouchChanged;
        public event System.Action<bool> OnRunningChanged;

        // Dependencies
        private IInputReader inputReader;

        private void Awake()
        {
            inputReader = GetComponent<IInputReader>();
            if (controller == null)
                controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            UpdateGroundStatus();
            UpdateMovementState();

            Vector3 movement = CalculateMovement();
            ApplyGravity();

            controller.Move((movement + new Vector3(0, velocity.y, 0)) * Time.deltaTime);

            HandleJump();
        }

        private void UpdateMovementState()
        {
            // Handle crouch toggle
            bool newCrouchState = inputReader.IsCrouching;
            if (isCrouching != newCrouchState)
            {
                isCrouching = newCrouchState;
                OnCrouchChanged?.Invoke(isCrouching);
            }

            // Handle run (can't run while crouching)
            bool canRun = inputReader.IsRunning && !isCrouching;
            if (isRunning != canRun)
            {
                isRunning = canRun;
                OnRunningChanged?.Invoke(isRunning);
            }
        }

        private Vector3 CalculateMovement()
        {
            Vector2 input = inputReader.MoveAxis;
            Vector3 move = orientation.right * input.x + orientation.forward * input.y;

            if (move.magnitude > 1f)
                move.Normalize();

            float currentSpeed = GetCurrentSpeed();
            return move * currentSpeed;
        }

        private float GetCurrentSpeed()
        {
            if (isCrouching) return crouchSpeed;
            if (isRunning) return runSpeed;
            return walkSpeed;
        }

        private void ApplyGravity()
        {
            if (!isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                velocity.y = Mathf.Max(velocity.y, -20f); // Terminal velocity
            }
            else if (velocity.y < 0)
            {
                velocity.y = -2f; // Small downward force when grounded
            }
        }

        private void HandleJump()
        {
            if (isGrounded && inputReader.JumpPressed)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }

        private void UpdateGroundStatus()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                groundCheckDistance,
                groundLayers
            );

            // Just landed - reset velocity
            if (!wasGrounded && isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        // Public properties if other systems need to read state
        public bool IsGrounded => isGrounded;
        public bool IsRunning => isRunning;
        public bool IsCrouching => isCrouching;
        public float CurrentSpeed => GetCurrentSpeed();
    }
}