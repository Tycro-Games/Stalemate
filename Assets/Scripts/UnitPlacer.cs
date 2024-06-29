using System.Collections.Generic;
using Assets.Scripts.Utility;
using Bogadanul.Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CellFinder))]
public class UnitPlacer : MonoBehaviour
{
    [SerializeField] private bool placeOnOppositeSide = false;
    [SerializeField] private UnitBoardInfo unitSettings;
    [SerializeField] private UnitInfoEvent onUnitInfoChanged;


    [SerializeField] private Vector3Event onHover;
    [SerializeField] private UnitInfoEvent onHoverUnitInfo;

    [Tooltip("Triggered where the unit was placed")] [SerializeField]
    private UnitRenderEvent onClick;

    [Tooltip("Triggered after the unit placement")] [SerializeField]
    private UnityEvent onPlace;

    [SerializeField] private UnityEvent onNoHover;
    [SerializeField] private UnityEvent onHoverNewTile;
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
        onUnitInfoChanged?.Invoke(settings);
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

    public void PickUpUnit(InputAction.CallbackContext context)
    {
        //there is something selected
        if (IsEditable == false)
            return;
        if (selectedRenderer.sprite != null)
        {
            if (context.control.IsPressed())
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

        PlaceUnit(context);
    }

    public void PlaceUnit(InputAction.CallbackContext context)
    {
        if (unitSettings.unitSettings == null) return;

        if (context.control.IsPressed())
        {
            var selectedPiece = place.transform.GetChild(0).GetComponent<UnitRenderer>();
            selectedPiece.SetUnitSettings(unitSettings);
            RedBlueTurn.currentPoints -= unitSettings.unitSettings.cost;
            CheckRemainingPoints();
            onPlace?.Invoke();
            placedSquares.Add(selectedPiece);
        }
    }

    private SpriteRenderer selectedRenderer;
    private GameObject place;
    private bool IsEditable = true;

    public void Place(Vector3 input)
    {
        selectedRenderer = null;
        IsEditable = true;
        place = cellFinder.NodeFromInput(input);
        if (place == null)
        {
            onNoHover?.Invoke();
            onHoverNewTile?.Invoke();
            IsEditable = false;
            return;
        }


        //Debug.Log("hover over:" + place.name);
        //decide which row is okay to place on red/ blue

        string nameTurn;
        if (placeOnOppositeSide)
            nameTurn = !RedBlueTurn.IsRedFirst() ? "Red" : "Blue";
        else
            nameTurn = !RedBlueTurn.IsRedFirst() ? "Blue" : "Red";

        selectedRenderer = place.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (selectedRenderer.sprite != null)
            onHoverUnitInfo?.Invoke(place.transform.GetChild(0).GetComponent<UnitRenderer>().GetUnitSettings());
        else
            onHoverNewTile?.Invoke();
        //this is the color of square that is being hovered over
        var nameSelected = place.transform.GetChild(1).name;
        //the right color of the turn and free space
        if (nameTurn != nameSelected)
        {
            IsEditable = false;
            onNoHover?.Invoke();
            return;
        }

        ////there is something selected
        //if (selectedRenderer.sprite != null)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        var selectedPiece = place.transform.GetChild(0).GetComponent<UnitRenderer>();
        //        var substraction = selectedPiece.GetUnitSettings().unitSettings.cost;
        //        if (unitSettings.unitSettings == null)
        //            SetUnitSettings(selectedPiece.GetUnitSettings());
        //        RedBlueTurn.currentPoints += substraction;
        //        placedSquares.Remove(selectedPiece);
        //        selectedPiece.SetUnitSettings(new UnitBoardInfo());
        //        onPlace?.Invoke();
        //        return;
        //    }


        //    onNoHover?.Invoke();
        //}


        onHover?.Invoke(place.transform.position);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    var selectedPiece = place.transform.GetChild(0).GetComponent<UnitRenderer>();
        //    selectedPiece.SetUnitSettings(unitSettings);
        //    RedBlueTurn.currentPoints -= unitSettings.unitSettings.cost;
        //    CheckRemainingPoints();
        //    onPlace?.Invoke();
        //    placedSquares.Add(selectedPiece);
        //}
    }
}