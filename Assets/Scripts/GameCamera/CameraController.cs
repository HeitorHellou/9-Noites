using Assets.Scripts.GameInput;
using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GameCamera
{
    public class CameraController : MonoBehaviour
    {
        [Header("Mouse Sensitivity")]
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float mouseMultiplier = 1f;

        [Header("Camera Limits")]
        [SerializeField] private float minLookAngle = -90f;
        [SerializeField] private float maxLookAngle = 90f;

        [Header("Camera Effects")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float bobFrequency = 1.5f;
        [SerializeField] private float bobHorizontalAmplitude = 0.1f;
        [SerializeField] private float bobVerticalAmplitude = 0.1f;

        [Header("Height Settings")]
        [SerializeField] private float standingEyeHeight = 1.6f;
        [SerializeField] private float crouchingEyeHeight = 1.0f;
        [SerializeField] private float heightTransitionSpeed = 8f;

        [Header("References")]
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform cameraHolder;

        private IInputReader inputReader;
        private PlayerMovement playerState;

        private float xRotation = 0f;
        private float yRotation = 0f;

        private Vector3 originalCameraPosition;
        private float bobTimer = 0f;

        private void Awake()
        {
            inputReader = GetComponentInParent<IInputReader>();
            playerState = GetComponentInParent<PlayerMovement>();

            originalCameraPosition = transform.localPosition;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            if (orientation != null)
                orientation.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }

        private void Update()
        {
            HandleMouseLook();
            HandleCameraHeight();

            if (enableHeadBob)
                HandleHeadBob();
        }

        private void HandleMouseLook()
        {
            Vector2 lookInput = inputReader.LookAxis;

            float mouseX = lookInput.x * mouseSensitivity * mouseMultiplier * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * mouseMultiplier * Time.deltaTime;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

            if (orientation != null)
                orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        private void HandleCameraHeight()
        {
            float targetHeight = playerState.IsCrouching ? crouchingEyeHeight : standingEyeHeight;

            Vector3 currentPos = cameraHolder.localPosition;
            currentPos.y = Mathf.Lerp(currentPos.y, targetHeight, Time.deltaTime * heightTransitionSpeed);
            cameraHolder.localPosition = currentPos;
        }

        private void HandleHeadBob()
        {
            if (!playerState.IsGrounded)
                return;

            Vector2 moveInput = inputReader.MoveAxis;
            bool isMoving = moveInput.magnitude > 0.1f;

            if (isMoving)
            {
                float speedMultiplier = playerState.IsCrouching ? 0.5f : playerState.IsRunning ? 1.5f : 1f;

                bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;

                float horizontalBob = Mathf.Sin(bobTimer) * bobHorizontalAmplitude;
                float verticalBob = Mathf.Sin(bobTimer * 2f) * bobVerticalAmplitude;

                Vector3 bobOffset = new Vector3(horizontalBob, verticalBob, 0f);
                transform.localPosition = originalCameraPosition + bobOffset;
            }
            else
            {
                bobTimer = 0f;
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalCameraPosition, Time.deltaTime * 5f);
            }
        }

        public void ShakeCamera(float intensity, float duration)
        {
            StartCoroutine(CameraShakeCoroutine(intensity, duration));
        }

        private System.Collections.IEnumerator CameraShakeCoroutine(float intensity, float duration)
        {
            Vector3 originalPos = cameraHolder.localPosition;
            float timer = 0f;

            while (timer < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                cameraHolder.localPosition = originalPos + new Vector3(x, y, 0f);

                timer += Time.deltaTime;
                yield return null;
            }

            cameraHolder.localPosition = originalPos;
        }

        public void SetSensitivity(float newSensitivity)
        {
            mouseSensitivity = newSensitivity;
        }

        public void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
