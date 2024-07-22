using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Bogadanul.Assets.Scripts.Utility
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private float speed = 5.0f;

        private Vector2 lastPos = Vector2.zero;
        private Camera mainCam;
        public event Action<Vector3> OnMovement;

        [SerializeField]
        private PlayerInput playerInput;
        private Mouse virtualMouse;
        [SerializeField] private float padding = 35.0f;
        private bool previousMouseState;
        [SerializeField] private float controllerCursorSpeed = 1000.0f;
        private void OnEnable()
        {
            if (virtualMouse == null)
            {
                virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            }
            else if (!virtualMouse.added)
            {
                InputSystem.AddDevice(virtualMouse);
            }

            InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

            InputSystem.onAfterUpdate += UpdateMotion;

            InputState.Change(virtualMouse.position, transform.position);
        }

        private void OnDisable()
        {
            InputSystem.RemoveDevice(virtualMouse);
            InputSystem.onAfterUpdate -= UpdateMotion;
            //playerInput.onControlsChanged -= OnControlsChanged;

        }
        private void UpdateMotion()
        {
            if (virtualMouse == null || Gamepad.current == null)
                return;
            Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
            deltaValue *= controllerCursorSpeed * Time.unscaledDeltaTime;

            Vector2 currentPosition = virtualMouse.position.ReadValue();
            Vector2 newPosition = currentPosition + deltaValue;

            newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

            InputState.Change(virtualMouse.position, newPosition);
            InputState.Change(virtualMouse.delta, deltaValue);

            //bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
            //if (previousMouseState != aButtonIsPressed)
            //{
            //    virtualMouse.CopyState<MouseState>(out var mouseState);
            //    mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
            //    InputState.Change(virtualMouse, mouseState);
            //    previousMouseState = aButtonIsPressed;
            //}

            AnchorCursor(newPosition);
        }

        private void AnchorCursor(Vector2 position)
        {
            lastPos = mainCam.ScreenToWorldPoint(position);
            OnMovement?.Invoke(position);
        }
        public void MousePosition(InputAction.CallbackContext context)
        {

            var mousePos = context.ReadValue<Vector2>();
            lastPos = mainCam.ScreenToWorldPoint(mousePos);
            OnMovement?.Invoke(mousePos);
        }

        private void Update()
        {
            transform.position = Vector2.Lerp(transform.position, lastPos, speed * Time.unscaledDeltaTime);
        }

        private void Start()
        {
            mainCam = Camera.main;
        }
    }
}