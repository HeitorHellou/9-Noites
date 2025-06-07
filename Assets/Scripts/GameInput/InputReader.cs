using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.GameInput
{
    public class InputReader : MonoBehaviour, IInputReader
    {
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction crouchAction;
        private InputAction jumpAction;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool isRunning;
        private bool isCrouching;
        private bool jumpPressed;

        public Vector2 MoveAxis => moveInput;
        public Vector2 LookAxis => lookInput;
        public bool IsRunning => isRunning;
        public bool IsCrouching => isCrouching;
        public bool JumpPressed => jumpPressed;

        public bool InteractPressed => throw new NotImplementedException();

        private void Start()
        {
            var actionMap = InputSystem.actions.FindActionMap("Player");

            moveAction = InputSystem.actions.FindAction("Move");
            lookAction = InputSystem.actions.FindAction("Look");
            runAction = InputSystem.actions.FindAction("Sprint");
            crouchAction = InputSystem.actions.FindAction("Crouch");
            jumpAction = InputSystem.actions.FindAction("Jump");

            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            runAction.performed += OnRun;
            runAction.canceled += OnRun;

            crouchAction.performed += OnCrouch;

            jumpAction.performed += OnJump;
            jumpAction.canceled += OnJump;

            moveAction.Enable();
            lookAction.Enable();
            runAction.Enable();
            crouchAction.Enable();
            jumpAction.Enable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            isRunning = context.phase != InputActionPhase.Canceled;
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                isCrouching = !isCrouching;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            jumpPressed = context.phase == InputActionPhase.Performed;
        }

        private void OnDestroy()
        {
            moveAction.Disable();
            lookAction.Disable();
            runAction.Disable();
            crouchAction.Disable();
            jumpAction.Disable();

            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;

            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;

            runAction.performed -= OnRun;
            runAction.canceled -= OnRun;

            crouchAction.performed -= OnCrouch;

            jumpAction.performed -= OnJump;
            jumpAction.canceled -= OnJump;
        }
    }
}
