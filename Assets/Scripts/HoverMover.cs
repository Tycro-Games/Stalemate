using Bogadanul.Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class HoverMover : MonoBehaviour
{
    private CursorController cursorController;

    private Camera cam;
    [SerializeField] private Vector2 minLimits;
    [SerializeField] private Vector2 maxLimits;

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
        if (pos.x < minLimits.x || pos.x > maxLimits.x || pos.y < minLimits.y || pos.y > maxLimits.y)
            return;
        transform.position = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(1.5f, 2f, 0), minLimits + maxLimits);
    }
}