using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameInput
{
    public interface IInputReader
    {
        Vector2 MoveAxis { get; }
        Vector2 LookAxis { get; }
        bool IsRunning { get; }
        bool IsCrouching { get; }
        bool JumpPressed { get; }
        bool InteractPressed { get; }
    }
}
