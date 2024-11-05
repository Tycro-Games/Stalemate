using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;

public class RedBlueTurn : MonoBehaviour {
  [SerializeField]
  private bool redStarts = false;
  [SerializeField]
  private bool playerStartsRound = false;
  [SerializeField]
  private int startingTurn = 0;

  public static int currentTurn;

  [SerializeField]
  private int startingPoints = 0;
  [SerializeField]
  private int incrementPoints = 1;

  public static int maxPoints;
  public static int currentPoints;

  [SerializeField]
  private int endTurn = 7;
  private static bool isPlayerFirst;
  [SerializeField]
  private UnityEvent onWin;

  public static bool IsPlayerFirst() {
    return isPlayerFirst;
  }
  public static bool IsRedFirst() {
    return isRedFirst;
  }

  [SerializeField]
  private UnityEvent onNextTurn;
  [SerializeField]
  private UnityEvent onFinishPlacement;
  [SerializeField]
  private StringEvent onScoreChange;
  [SerializeField]
  private StringEvent onTurnChange;
  [SerializeField]
  private StringEvent onSideChange;
  [SerializeField]
  private UnityEvent onRedTurn;
  [SerializeField]
  private UnityEvent onBlueTurn;

  private static bool isRedFirst = false;
  [SerializeField]
  private UnityEvent onPlayerDeploy;
  [SerializeField]
  private UnityEvent onPlayerAct;
  [SerializeField]
  private UnityEvent onAIDeploy;
  [SerializeField]
  private UnityEvent onAIAct;

  private bool isPriorityNationDone = false;
  public void TriggerNationActions() {
    if (isPriorityNationDone) {
      // do the other
      if (!isRedFirst) {
        if (isPlayerFirst) {
          // do red
          CallPlayerActions();
        } else {
          CallAIActions();
        }
      } else {
        if (!isPlayerFirst) {
          // do red
          CallPlayerActions();
        } else {
          CallAIActions();
        }
      }
      isPriorityNationDone = false;
    } else {
      if (isRedFirst) {
        if (isPlayerFirst) {
          // do red
          CallPlayerActions();
        } else {
          CallAIActions();
        }
      } else {
        if (!isPlayerFirst) {
          // do red
          CallPlayerActions();
        } else {
          CallAIActions();
        }
        // do blue
      }
      isPriorityNationDone = true;
    }
  }

  private void CallAIActions() {
    onAIDeploy?.Invoke();

    onAIAct?.Invoke();
  }

  private void CallPlayerActions() {
    onPlayerDeploy?.Invoke();
    onPlayerAct?.Invoke();
  }

  private void OnEnable() {
    currentTurn = startingTurn;

    isRedFirst = redStarts;
    isPlayerFirst = playerStartsRound;
    onSideChange?.Invoke(isPlayerFirst ? "Red" : "Blue");

    maxPoints = startingPoints;
  }
  public void SetPoints(string points) {
    maxPoints = int.Parse(points);
    currentPoints = maxPoints;
    UpdateText();
  }

  // 1 increases points, 0 does not
  private bool increasePoints = true;
  public void SetValues() {
    currentTurn++;
    if (increasePoints) {
      maxPoints += incrementPoints;
      increasePoints = false;
    } else {
      increasePoints = true;
    }

    currentPoints = maxPoints;
    UpdateText();
  }

  [SerializeField]
  private UnityEvent onSwitchPriorityToRed;
  [SerializeField]
  private UnityEvent onSwitchPriorityToBlue;

  public void SwitchSides()
  {
    isPlayerFirst = !isPlayerFirst;
    if (isPlayerFirst) {
      onSideChange?.Invoke("Red");

    } else {
      onSideChange?.Invoke("Blue");
    }
  }
  public void SwitchPriorityNation() {
    isRedFirst = !isRedFirst;
    isPriorityNationDone = false;
    if (isRedFirst) {
      onSwitchPriorityToRed?.Invoke();
    } else {
      onSwitchPriorityToBlue?.Invoke();
    }
  }

  public void NextTurn() {
    if (currentTurn == endTurn)
      onWin?.Invoke();

    if (isPlayerFirst)
      onRedTurn?.Invoke();
    else
      onBlueTurn?.Invoke();

    onNextTurn?.Invoke();
  }

  public void UpdateText() {
    onScoreChange?.Invoke(currentPoints.ToString());
    onTurnChange?.Invoke(currentTurn.ToString());
  }

  public void FinishPlacement() {
    onFinishPlacement?.Invoke();
  }
}
