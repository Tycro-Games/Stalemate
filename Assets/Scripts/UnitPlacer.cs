using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using Bogadanul.Assets.Scripts.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CellFinder))]
public class UnitPlacer : MonoBehaviour
{
    [SerializeField] private ScriptableUnitSettings unitSettings;
    [SerializeField] private UnitSettingsEvent onUnitSettingsChanged;


    [SerializeField] private UnityEvent onHover;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onNoHover;


    private CellFinder cellFinder;
    private CursorController cursorController;

    private void Start()
    {
        cellFinder = GetComponent<CellFinder>();
        cursorController = FindObjectOfType<CursorController>();
        cursorController.OnMovement += Place;
    }

    private void OnDisable()
    {
        cursorController.OnMovement -= Place;
    }

    public void SetUnitSettings(ScriptableUnitSettings settings)
    {
        unitSettings = settings;
        onUnitSettingsChanged?.Invoke(settings);
    }

    private void Reset()
    {
        unitSettings = null;
    }

    public void Place(Vector3 input)
    {
        var place = cellFinder.NodeFromInput(input);
        if (place == null)
        {
            onNoHover?.Invoke();
            return;
        }

        onHover?.Invoke();

        Debug.Log("hover over:" + place.name);
        //decide which row is okay to place on red/ blue

        //check if the place is full or not
        //        if (currentTree != null)
        //        {

        //            hasPlaced = false;
        //#if UNITY_ANDROID
        //                if (first)
        //                {
        //                    first = false;
        //                }
        //#endif
        //            if (placeable)
        //            {
        //                Node n = raycaster.NodeFromInput(input);
        //                if (n == null)
        //                {
        //#if UNITY_ANDROID
        //                        //CancelPlacing();
        //#endif
        //                    return;
        //                }
        //                if (!Fruit)
        //                    CheckNode(n);
        //                else if (n.FruitPlaceable())
        //                {
        //                    CheckPlacerPath.ToSpawn(n, currentTree);
        //                    Instantiate(EffectOnPlace, n.worldPosition, Quaternion.identity);


        //                    hasPlaced = true;

        //                    Placing(n);
        //                }
        //            }
    }
}