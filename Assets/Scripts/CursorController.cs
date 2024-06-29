using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bogadanul.Assets.Scripts.Utility
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private float speed = 5.0f;

        private Vector2 lastPos = Vector2.zero;
        private Camera mainCam;
        public event Action<Vector3> OnMovement;

        public void MousePosition(InputAction.CallbackContext context)
        {

            var mousePos = context.ReadValue<Vector2>();
            lastPos=mainCam.ScreenToWorldPoint(mousePos);
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