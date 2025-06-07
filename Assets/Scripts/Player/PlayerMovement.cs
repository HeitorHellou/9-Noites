using Assets.Scripts.GameInput;
using Assets.Scripts.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    internal class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform orientation;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckDistance = 0.3f;

        private IInputReader inputReader;
        private PlayerState playerState;
        private MovementModel movementModel;

        private Vector3 playerVelocity;
        private Vector3 horizontalMovement;

        private void Awake()
        {
            inputReader = GetComponent<IInputReader>();
            playerState = GetComponent<PlayerState>();
        }

        private void Start()
        {
            movementModel = playerState.Model;
            CheckGroundStatus();
        }

        void Update()
        {
            CheckGroundStatus();
            HandleMovement();
            ApplyGravity();

            Vector3 finalMovement = horizontalMovement + new Vector3(0, playerVelocity.y, 0);
            controller.Move(finalMovement * Time.deltaTime);
        }

        private void HandleMovement()
        {
            Vector2 moveInput = inputReader.MoveAxis;
            Vector3 move = orientation.right * moveInput.x + orientation.forward * moveInput.y;

            if (move.magnitude > 1f)
                move.Normalize();
            move.y = 0;

            horizontalMovement = move * movementModel.GetCurrentSpeed();
        }

        private void ApplyGravity()
        {
            if (!movementModel.IsGrounded)
            {
                playerVelocity.y += movementModel.GravityValue * Time.deltaTime;
                playerVelocity.y = Mathf.Max(playerVelocity.y, -20f);
            }
            else if (playerVelocity.y < 0)
            {
                playerVelocity.y = -2f;
            }
        }

        private void CheckGroundStatus()
        {
            bool wasGrounded = movementModel.IsGrounded;
            bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayers);
            movementModel.IsGrounded = isGrounded;

            if (!wasGrounded && isGrounded)
                playerVelocity.y = -2f;
        }
    }
}
