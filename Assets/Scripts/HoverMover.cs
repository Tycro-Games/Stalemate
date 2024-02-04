using Bogadanul.Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class HoverMover : MonoBehaviour
{
    [SerializeField] private Vector2 minLimits;
    [SerializeField] private Vector2 maxLimits;


    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f), minLimits + maxLimits);
    }
}