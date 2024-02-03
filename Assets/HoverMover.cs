using Bogadanul.Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class HoverMover : MonoBehaviour
{
    private CursorController cursorController;

    private Camera cam;

    private void Start()
    {
        cursorController = FindObjectOfType<CursorController>();

        cam = Camera.main;
    }

    private void OnEnable()
    {
        cursorController.OnMovement += Move;
    }

    private void OnDisable()
    {
        cursorController.OnMovement -= Move;
    }

    private void Move(Vector3 pos)
    {
        pos = cam.ScreenToWorldPoint(pos);
        transform.position = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
}