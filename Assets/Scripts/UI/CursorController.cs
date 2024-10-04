using System;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Bogadanul.Assets.Scripts.Utility {
  public class CursorController : MonoBehaviour {
    [SerializeField]
    private float speed = 5.0f;

    private Vector2 lastPos = Vector2.zero;
    private Camera mainCam;
    public event Action<Vector3> OnMovement;
    [SerializeField]
    private Vector3Event OnMovementEvent;

    [SerializeField]
    private PlayerInput playerInput;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    [SerializeField]
    private float padding = 35.0f;
    private bool previousMouseState;
    [SerializeField]
    private float controllerCursorSpeed = 1000.0f;
    private void OnEnable() {
      mainCam = Camera.main;
      currentMouse = Mouse.current;
      if (virtualMouse == null) {
        virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
      } else if (!virtualMouse.added) {
        InputSystem.AddDevice(virtualMouse);
      }

      InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

      InputSystem.onAfterUpdate += UpdateMotion;

      InputState.Change(virtualMouse.position, transform.position);
    }

    private void OnDisable() {
      if (virtualMouse != null && virtualMouse.added)
        InputSystem.RemoveDevice(virtualMouse);
      InputSystem.onAfterUpdate -= UpdateMotion;
      // playerInput.onControlsChanged -= OnControlsChanged;
    }
    private void UpdateMousePosition() {
      // Get the current mouse position
      Vector2 mousePos = Mouse.current.position.ReadValue();
      lastPos = mainCam.ScreenToWorldPoint(mousePos);

      // Trigger movement events
      OnMovementEvent?.Invoke(lastPos);
      OnMovement?.Invoke(mousePos);

      // Smoothly move the object towards the mouse position
      transform.position =
          Vector2.Lerp(transform.position, lastPos, speed * Time.unscaledDeltaTime);
    }

    private void UpdateGamepadPosition() {
      if (virtualMouse == null || Gamepad.current == null)
        return;

      Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
      deltaValue *= controllerCursorSpeed * Time.unscaledDeltaTime;

      Vector2 currentPosition = virtualMouse.position.ReadValue();
      Vector2 newPosition = currentPosition + deltaValue;

      // Clamp to screen bounds
      newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
      newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

      // Update the virtual mouse position
      InputState.Change(virtualMouse.position, newPosition);
      InputState.Change(virtualMouse.delta, deltaValue);

      // Convert to world position
      lastPos = mainCam.ScreenToWorldPoint(newPosition);

      // Smoothly move the cursor object
      transform.position =
          Vector2.Lerp(transform.position, lastPos, speed * Time.unscaledDeltaTime);

      // Trigger movement events
      OnMovementEvent?.Invoke(lastPos);
      OnMovement?.Invoke(newPosition);
    }
    private void UpdateMotion() {
      if (playerInput.currentControlScheme == gamepadScheme) {
        // Handle gamepad movement
        UpdateGamepadPosition();
      }
    }

    private string previosScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string keyboardScheme = "Keyboard&Mouse";

    public void OnControlsChanged(PlayerInput input) {
      if (currentMouse == null)
        return;
      if (playerInput.currentControlScheme == keyboardScheme && previosScheme != keyboardScheme) {
        currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        previosScheme = keyboardScheme;
      } else if (playerInput.currentControlScheme == gamepadScheme &&
                 previosScheme != gamepadScheme) {
        InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
        transform.position = mainCam.ScreenToWorldPoint(currentMouse.position.ReadValue());
        previosScheme = gamepadScheme;
      }
    }

    public void MousePosition(InputAction.CallbackContext context) {
      if (mainCam == null)
        return;
      var mousePos = context.ReadValue<Vector2>();
      lastPos = mainCam.ScreenToWorldPoint(mousePos);
      OnMovementEvent?.Invoke(lastPos);
      OnMovement?.Invoke(mousePos);
    }

    private void Update() {
      if (mainCam == null)
        return;

      if (playerInput.currentControlScheme == keyboardScheme) {
        // Continuously update mouse position
        UpdateMousePosition();
      } else if (playerInput.currentControlScheme == gamepadScheme) {
        // Continuously update gamepad cursor position (handled in UpdateMotion)
        UpdateGamepadPosition();
      }
    }

    private void Start() {
      Cursor.visible = false;
    }
  }
}
