using Assets.Scripts.GameInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interaction
{
    internal class InteractionManager : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float maxInteractionDistance = 3f;
        [SerializeField] private LayerMask interactableLayers = -1;

        private IInputReader inputReader;
        private IInteractable currentInteractable;

        public event Action<string> OnInteractionPromptChanged;

        private void Update()
        {
            CheckForInteractable();
            HandleInteractionInput();
        }

        private void CheckForInteractable()
        {
            IInteractable newInteractable = null;

            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, maxInteractionDistance, interactableLayers))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(gameObject))
                    newInteractable = interactable;
            }

            if (currentInteractable != newInteractable)
            {
                currentInteractable = newInteractable;
                string prompt = currentInteractable?.GetInteractionPrompt() ?? "";
                OnInteractionPromptChanged?.Invoke(prompt);
            }
        }

        private void HandleInteractionInput()
        {
            if (inputReader.InteractPressed && currentInteractable != null)
                currentInteractable.Interact(gameObject);
        }
    }
}
