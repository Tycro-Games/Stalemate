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
    [SerializeField] private UnitBoardInfo unitSettings;
    [SerializeField] private UnitSettingsEvent onUnitSettingsChanged;


    [SerializeField] private Vector3Event onHover;

    [Tooltip("Triggered where the unit was placed")] [SerializeField]
    private UnitRenderEvent onClick;

    [Tooltip("Triggered after the unit placement")] [SerializeField]
    private UnityEvent onPlace;

    [SerializeField] private UnityEvent onNoHover;
    [SerializeField] private UnityEvent onValidTurn;
    [SerializeField] private UnityEvent onNotValidTurn;


    private CellFinder cellFinder;
    private CursorController cursorController;
    [SerializeField] private int maxPlacements = 4;
    private List<UnitRenderer> placedSquares = new();

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

    public void SetUnitSettings(UnitBoardInfo settings)
    {
        unitSettings = settings;
        onUnitSettingsChanged?.Invoke(settings);
    }

    private void Reset()
    {
        SetUnitSettings(new UnitBoardInfo());
    }

    private void CheckRemainingPoints()
    {
        if (RedBlueTurn.currentPoints < unitSettings.unitSettings.cost) Reset();
    }

    public void ValidTurn()
    {
        if (placedSquares.Count == maxPlacements || RedBlueTurn.currentPoints == 0)
            onValidTurn?.Invoke();
        else
            onNotValidTurn?.Invoke();
    }

    public void SetNewUnitsToBoard()
    {
        foreach (var square in placedSquares)
            onClick?.Invoke(square);
        placedSquares.Clear();
    }

    public void Place(Vector3 input)
    {
        var place = cellFinder.NodeFromInput(input);
        if (place == null)
        {
            onNoHover?.Invoke();
            return;
        }


        Debug.Log("hover over:" + place.name);
        //decide which row is okay to place on red/ blue
        var nameTurn = RedBlueTurn.IsRedFirst() ? "Red" : "Blue";
        var selectedRenderer = place.transform.GetChild(0).GetComponent<SpriteRenderer>();

        //this is the color of square that is being hovered over
        var nameSelected = place.transform.GetChild(1).name;
        //the right color of the turn and free space
        if (nameTurn != nameSelected)
        {
            onNoHover?.Invoke();
            return;
        }

        //there is something selected
        if (selectedRenderer.sprite != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectedPiece = place.transform.GetChild(0).GetComponent<UnitRenderer>();
                var substraction = selectedPiece.GetUnitSettings().unitSettings.cost;
                if (unitSettings.unitSettings == null)
                    SetUnitSettings(selectedPiece.GetUnitSettings());
                RedBlueTurn.currentPoints += substraction;
                placedSquares.Remove(selectedPiece);
                selectedPiece.SetUnitSettings(new UnitBoardInfo());
                onPlace?.Invoke();
                return;
            }

            onNoHover?.Invoke();
        }


        onHover?.Invoke(place.transform.position);
        if (unitSettings.unitSettings == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            var selectedPiece = place.transform.GetChild(0).GetComponent<UnitRenderer>();
            selectedPiece.SetUnitSettings(unitSettings);
            RedBlueTurn.currentPoints -= unitSettings.unitSettings.cost;
            CheckRemainingPoints();
            onPlace?.Invoke();
            placedSquares.Add(selectedPiece);
        }
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