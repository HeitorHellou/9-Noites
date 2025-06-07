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
    internal class PlayerState : MonoBehaviour
    {
        private IInputReader inputReader;
        private MovementModel movementModel;

        public event Action<bool> OnCrouchChanged;
        public event Action<bool> OnRunningChanged;

        public MovementModel Model => movementModel;

        private void Awake()
        {
            inputReader = GetComponent<IInputReader>();
            movementModel = new MovementModel
            {
                WalkSpeed = 3f,
                RunSpeed = 6f,
                CrouchSpeed = 2f,
                JumpForce = 1f,
                GravityValue = -19.62f
            };
        }

        private void Update()
        {
            HandleCrouch();
            HandleRun();
        }

        private void HandleCrouch()
        {
            if (movementModel.IsCrouching != inputReader.IsCrouching)
            {
                movementModel.IsCrouching = inputReader.IsCrouching;
                OnCrouchChanged?.Invoke(movementModel.IsCrouching);
            }
        }

        private void HandleRun()
        {
            bool wantsToRun = inputReader.IsRunning && !movementModel.IsCrouching;
            if (movementModel.IsRunning != wantsToRun)
            {
                movementModel.IsRunning = wantsToRun;
                OnRunningChanged?.Invoke(wantsToRun);
            }
        }
    }
}
