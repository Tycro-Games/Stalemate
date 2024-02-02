using System;
using UnityEngine;

namespace Bogadanul.Assets.Scripts.Utility
{
    public class CursorController : MonoBehaviour
    {
        private Transform cursorTransform = null;

        [SerializeField] private float speed = 5.0f;

        private Vector2 lastPos = Vector2.zero;
        private Camera mainCam;
        public event Action<Vector3> OnMovement;

        public Vector2 MousePosition(bool eventTrigger)
        {
            Vector2 mousePos = transform.position;
#if !UNITY_ANDROID
            mousePos = Input.mousePosition;
            if (eventTrigger && Input.GetMouseButtonDown(0))
                OnMovement?.Invoke(mousePos);
            return mousePos;
#endif
#if UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                mousePos = touch.position;

                if (eventTrigger&&touch.phase == TouchPhase.Ended)
                    OnMovement?.Invoke(mousePos);
                lastPos = mousePos;
            }
            return lastPos;
#endif
        }

        private void Update()
        {
            transform.position = Vector2.Lerp(transform.position, mainCam.ScreenToWorldPoint(MousePosition(true)),
                speed * Time.unscaledDeltaTime);
        }

        private void Start()
        {
#if UNITY_ANDROID
            Application.targetFrameRate = 120;

#endif
            cursorTransform = transform;
            mainCam = Camera.main;
        }
    }
}