using Assets.Scripts.GameInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Movement
{
    internal class MovementModel
    {
        public bool IsRunning { get; set; }
        public bool IsCrouching { get; set; }
        public float WalkSpeed { get; set; }
        public float RunSpeed { get; set; }
        public float CrouchSpeed { get; set; }
        public float JumpForce { get; set; } = 8f;
        public float GravityValue { get; set; } = -9.81f;
        public bool IsGrounded { get; set; }
        public bool IsJumping { get; set; }
        public float JumpCooldown { get; set; } = 0.1f;

        public float GetCurrentSpeed() =>
            IsCrouching ? CrouchSpeed :
            IsRunning ? RunSpeed : WalkSpeed;
    }
}
