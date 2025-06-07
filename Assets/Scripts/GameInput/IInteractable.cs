using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameInput
{
    public interface IInteractable
    {
        string GetInteractionPrompt();
        bool CanInteract(GameObject player);
        void Interact(GameObject player);
        float InteractionDistance { get; }
    }
}
