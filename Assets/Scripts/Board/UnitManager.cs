using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnitMover))]
[RequireComponent(typeof(UnitAttacker))]
public class UnitManager : MonoBehaviour {
  private List<UnitRenderer> redUnits = new();
  private List<UnitRenderer> blueUnits = new();
  [SerializeField]
  private float timeBeforeMove = 1.0f;
  [SerializeField]
  private float timeAfterMove = 1.0f;
  [SerializeField]
  private UnityEvent onMoveEnd;
  [SerializeField]
  private UnityEvent onAttackEnd;
  [SerializeField]
  private UnityEvent onBoostEnd;
  private Board board;
  private UnitMover mover;
  private UnitAttacker attacker;

  private bool isPlayerTurn = true;

  public static Action onUnitManipulation;
  public static Action<int, int> onWinConditionChange;

  [HideInInspector]
  public List<Transform> positions;
  private int redUnitsOnY = 0;
  private int blueUnitsOnY = 0;
  private bool hasMovedPriorityNation = false;

  // auxialiary lists for feedback systems
  private List<UnitRenderer> initialUnitSpace = new();
  private List<UnitRenderer> finalUnitSpace = new();

  private List<Tuple<Vector2, AttackTypes>> attackPositions;
  private List<UnitRenderer> piecesToBoost;

  public void SetCurrentSide(bool playerTurn) {
    if (hasMovedPriorityNation) {
      hasMovedPriorityNation = false;
      isPlayerTurn = !RedBlueTurn.IsRedFirst();

    } else {
      hasMovedPriorityNation = true;
      isPlayerTurn = RedBlueTurn.IsRedFirst();
    }
  }

  public void UpdateWinningCounts() {
    redUnitsOnY = 0;
    blueUnitsOnY = 0;
    var middleLineXs = new List<float>();
    var middleLineYs = new List<float>();
    foreach (var position in positions) {
      middleLineXs.Add(position.transform.position.x);
      middleLineYs.Add(position.transform.position.y);
    }
    var redUnitXs = new List<float>();
    var redUnitYs = new List<float>();
    foreach (var redUnit in redUnits) {
      redUnitXs.Add(redUnit.transform.position.x);
      redUnitYs.Add(redUnit.transform.position.y);
    }

    var blueUnitXs = new List<float>();
    var blueUnitYs = new List<float>();
    foreach (var blueUnit in blueUnits) {
      blueUnitXs.Add(blueUnit.transform.position.x);
      blueUnitYs.Add(blueUnit.transform.position.y);
    }
    for (int i = 0; i < middleLineXs.Count; i++) {
      float middleLineX = middleLineXs[i];
      float middleLineY = middleLineYs[i];

      // Count red units on Y axis
      for (int j = 0; j < redUnitXs.Count; j++) {
        if (Math.Abs(redUnitXs[j] - middleLineX) < 0.01f && redUnitYs[j] <= middleLineY) {
          redUnitsOnY++;
        }
      }

      // Count blue units on Y axis
      for (int j = 0; j < blueUnitXs.Count; j++) {
        if (Math.Abs(blueUnitXs[j] - middleLineX) < 0.01f && blueUnitYs[j] >= middleLineY) {
          blueUnitsOnY++;
        }
      }
    }

    onWinConditionChange?.Invoke(redUnitsOnY, blueUnitsOnY);
  }

  public List<UnitRenderer> GetRedUnits() {
    return redUnits;
  }

  public List<UnitRenderer> GetBlueUnits() {
    return blueUnits;
  }

  private void Start() {
    board = GetComponent<Board>();
    mover = GetComponent<UnitMover>();
    attacker = GetComponent<UnitAttacker>();
  }

  public void ResetRedBlueUnitLists() {
    redUnits = Board.GetAllPieces(SquareType.RED, ref board.pieces);
    blueUnits = Board.GetAllPieces(SquareType.BLUE, ref board.pieces);
    UpdateWinningCounts();
  }

  public void MoveCurrentSide() {
    StartCoroutine(Movement(isPlayerTurn, GlobalSettings.GetOneActionPerUnit()));
  }

  public void AttackCurrentSide() {
    StartCoroutine(Attack(isPlayerTurn, GlobalSettings.GetOneActionPerUnit()));
  }

  public void BoostCurrentSide() {
    StartCoroutine(Boost(isPlayerTurn));
  }

  private IEnumerator Movement(bool isRed, bool canSetPerformAction)
  {
    yield return new WaitForSeconds(timeBeforeMove);
    if (isRed)
      MoveUnits(ref redUnits, canSetPerformAction);
    else
      MoveUnits(ref blueUnits, canSetPerformAction);

    yield return StartCoroutine(MovementFeedback());

    yield return new WaitForSeconds(timeAfterMove);

    onMoveEnd?.Invoke();
    onUnitManipulation?.Invoke();
    UpdateWinningCounts();
  }

  private IEnumerator MovementFeedback() {
    yield return StartCoroutine(mover.MoveUnits(initialUnitSpace, finalUnitSpace));
    initialUnitSpace.Clear();
    finalUnitSpace.Clear();
  }
  private IEnumerator AttackFeedback(bool isRed) {
    yield return StartCoroutine(attacker.AttackUnits(attackPositions, isRed));
    attackPositions.Clear();
  }

  private IEnumerator Boost(bool isRed) {
    yield return new WaitForSeconds(timeBeforeMove);
    if (isRed) {
      BoostUnits(ref redUnits);

    } else {
      BoostUnits(ref blueUnits);
    }

    yield return StartCoroutine(MovementFeedback());
    yield return StartCoroutine(AttackFeedback(isRed));

    if (piecesToBoost.Count > 0)
      AudioManager.instance.PlayOneShot(AudioManager.instance.boostSound);

    yield return new WaitForSeconds(timeAfterMove);
    onBoostEnd?.Invoke();
    onUnitManipulation?.Invoke();
    UpdateWinningCounts();
  }

  private IEnumerator Attack(bool isRed, bool canSetPerformAction)
  {
    yield return new WaitForSeconds(timeBeforeMove);
    if (isRed) {
      AttackUnits(ref redUnits, canSetPerformAction);
      CleanNullEnemies(ref blueUnits);
    } else {
      AttackUnits(ref blueUnits, canSetPerformAction);
      CleanNullEnemies(ref redUnits);
    }
    yield return StartCoroutine(AttackFeedback(isRed));

    yield return new WaitForSeconds(timeAfterMove);
    onAttackEnd?.Invoke();
    onUnitManipulation?.Invoke();
    UpdateWinningCounts();
  }

  public void CleanNullEnemies(ref List<UnitRenderer> units) {
    for (var i = units.Count - 1; i >= 0; i--)
      if (units[i].GetUnitSettings().unitSettings == null)
        units.RemoveAt(i);
  }

  public void GetAttackingSquares(out List<Tuple<Vector2, AttackTypes>> attackingPos) {
    attackingPos = attackPositions;
  }
  public void GetInitialMovementSquares(out List<UnitRenderer> movePos) {
    movePos = initialUnitSpace;
  }
  public void GetFinalMovementSquares(out List<UnitRenderer> movePos) {
    movePos = finalUnitSpace;
  }
  public void MoveUnits(ref List<UnitRenderer> units, bool setPerformAction = false)
  {
    if (units.Count == 0)
      return;

    initialUnitSpace = new List<UnitRenderer>(new UnitRenderer[units.Count]);
    finalUnitSpace = new List<UnitRenderer>(new UnitRenderer[units.Count]);

    SortUnits(ref units);
    for (var i = 0; i < units.Count; i++) {

      var settings = units[i].GetUnitSettings();
      var sign = settings.isRed ? 1 : -1;

      if (initialUnitSpace[i] == null)
        initialUnitSpace[i] = units[i];
      if (GlobalSettings.GetOneActionPerUnit() && units[i].hasPerformedAction)
        continue;
      // get settings
      for (var j = 0; j < settings.unitSettings.movePositions.Length; j++) {
        UnitRenderer newSquare = Board.PieceInFrontWithPadding(
            units[i], settings.unitSettings.movePositions[j] * sign, ref board.pieces);
        if (newSquare == null)
          continue;

        if (newSquare.GetUnitSettings().unitSettings != null)
          continue;

        newSquare.SetUnitSettingsAndHp(settings, units[i].GetHp());

        finalUnitSpace[i] = newSquare;

        units[i].SetUnitSettings(new UnitBoardInfo());

        units[i] = newSquare;

        if (setPerformAction)
        {
          units[i].hasPerformedAction = true;
        }
      }
    }
  }

  private static void SortUnits(ref List<UnitRenderer> units) {
    var decreasingSortOrder = !units[0].GetUnitSettings().isRed;
    if (decreasingSortOrder)
      units.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
    else
      units.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
  }

  public void BoostUnits(ref List<UnitRenderer> units) {
    if (units.Count == 0)
      return;
    SortUnits(ref units);
    piecesToBoost = new List<UnitRenderer>();
    for (var i = 0; i < units.Count; i++) {
      var settings = units[i].GetUnitSettings();
      if (!settings.unitSettings.boost)
        continue;
      var sign = settings.isRed ? 1 : -1;
      // this should only be one
      for (var j = 0; j < settings.unitSettings.boostPositions.Length; j++) {
        var newSquare = Board.PieceInFront(units[i], settings.unitSettings.boostPositions[j] * sign,
                                           ref board.pieces);
        if (newSquare == null)
          continue;

        var attackedSquareSettings = newSquare.GetUnitSettings();
        // move itself
        if (attackedSquareSettings.unitSettings == null ||
            attackedSquareSettings.isRed != settings.isRed) {
          // Debug.Log("unit " + units[i].name + "boosted itself");

          piecesToBoost.Add(units[i]);
        }
        // boost what is in the boost positions
        else {
          // Debug.Log("unit " + units[i].name + "boosted" + newSquare.name);

          piecesToBoost.Add(newSquare);
        }
      }
    }

    if (piecesToBoost.Count == 0)
      return;
    piecesToBoost = piecesToBoost.Distinct().ToList();
    var isRed = piecesToBoost[0].GetUnitSettings().isRed;
    foreach (var piece in piecesToBoost)
    {
      piece.hasPerformedAction = false;
    }
    if (GlobalSettings.GetOneActionPerUnit())
    {
      AttackUnits(ref piecesToBoost, GlobalSettings.GetOneActionPerUnit());
      MoveUnits(ref piecesToBoost, GlobalSettings.GetOneActionPerUnit());
    }
    else
    {
      MoveUnits(ref piecesToBoost);
      AttackUnits(ref piecesToBoost);
    }
    CleanNullEnemies(ref redUnits);
    CleanNullEnemies(ref blueUnits);
    if (isRed)
      redUnits.AddRange(piecesToBoost);
    else
      blueUnits.AddRange(piecesToBoost);

    ResetRedBlueUnitLists();
  }

  public void AttackUnits(ref List<UnitRenderer> units, bool setPerformAction = false)
  {
    if (units.Count == 0)
      return;

    attackPositions = new List<Tuple<Vector2, AttackTypes>>();
    SortUnits(ref units);
    for (var i = 0; i < units.Count; i++) {
      if (GlobalSettings.GetOneActionPerUnit() && units[i].hasPerformedAction)
        continue;
      var settings = units[i].GetUnitSettings();

      var sign = settings.isRed ? 1 : -1;

      for (var j = 0; j < settings.unitSettings.attackPositions.Length; j++) {
        var newSquare = Board.PieceInFront(
            units[i], settings.unitSettings.attackPositions[j] * sign, ref board.pieces);
        // outside bounds
        if (newSquare == null)
          continue;
        // Debug.Log("unit " + units[i].name + " attacked:" + newSquare.name);

        var attackedSquareSettings = newSquare.GetUnitSettings();

        if (attackedSquareSettings.unitSettings == null) {
          attackPositions.Add(
              Tuple.Create((Vector2)newSquare.transform.position, AttackTypes.EMPTY_SPACE));

          continue;
        }


        // if (attackedSquareSettings.isKillable == false) {
        //   attackPositions.Add(
        //       Tuple.Create((Vector2)newSquare.transform.position, AttackTypes.HIT_UNIT));
        //   continue;
        // }

        if (attackedSquareSettings.isRed == settings.isRed)
          continue;

        // check health of the unit

        // if health is 0, remove the unit
        if (newSquare.TryToKill()) {
          attackPositions.Add(
              Tuple.Create((Vector2)newSquare.transform.position, AttackTypes.DESTROY_UNIT));

          newSquare.SetUnitSettings(new UnitBoardInfo());
        } else {
          attackPositions.Add(
              Tuple.Create((Vector2)newSquare.transform.position, AttackTypes.HIT_UNIT));
        }
        if (setPerformAction)
        {
          units[i].hasPerformedAction = true;
        }
      }
    }
  }
}
