using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
internal struct DotLineCoordinates {
  public int x;
  public int y;
  public float isUp;

  public DotLineCoordinates(int x, int y, float sign) {
    this.x = x;
    this.y = y;
    isUp = sign;
  }
}

public enum SquareType { RED, BLUE }
;

public class Board : MonoBehaviour

{
  // board
  private static int sizeX = 4;
  private static int sizeY = 5;
  [SerializeField]
  private int startingSizeX = 4;
  [SerializeField]
  private int startingSizeY = 5;

  [SerializeField]
  private Sprite[] spritesOrder;
  [SerializeField]
  private Sprite middleVerticalLines;
  [SerializeField]
  private List<DotLineCoordinates> dotLinesCoordinates = new();
  [SerializeField]
  private Sprite dotLine;
  [HideInInspector]
  public bool shouldUpdate;

  public List<GameObject> squares;

  public List<UnitRenderer> pieces;
  [SerializeField]
  private float pieceSize = 7.0f;

#if UNITY_EDITOR
  [CustomEditor(typeof(Board))]
  public class BoardEditor : Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      var board = (Board)target;

      // Create a button outside the inspector
      if (GUILayout.Button("Manual Update") || board.shouldUpdate) {
        // Call the CreateBoard method when the button is clicked
        board.CreateBoard();
        board.shouldUpdate = false;
      }
    }
  }
#endif
  private void OnValidate() {
    shouldUpdate = true;
  }

  private void CreateBoard() {
    sizeX = startingSizeX;
    sizeY = startingSizeY;
    if (sizeY == 0 || sizeX == 0)
      return;
    DeleteBoard();

    squares = new List<GameObject>(sizeX * sizeY);
    pieces = new List<UnitRenderer>(sizeX * sizeY);
    for (var y = 0; y < sizeY; y++)
      for (var x = 0; x < sizeX; x++) {
        // Create square
        var index = x + y * sizeX;

        squares.Add(new GameObject());
        var piece = new GameObject("Piece" + (x, y));
        piece.transform.localScale = new Vector3(pieceSize, pieceSize, 1.0f);

        piece.transform.parent = squares[index].transform;
        pieces.Add(piece.AddComponent<UnitRenderer>());

        squares[index].layer = LayerMask.NameToLayer("Grid");
        squares[index].AddComponent<BoxCollider>();

        var square = squares[index].transform;
        square.parent = transform;
        square.localRotation = Quaternion.identity;
        square.name = (x, y).ToString();
        square.localPosition = new Vector2(x, y);

        if (y == sizeY / 2) {
          var spriteObj = new GameObject();
          spriteObj.name = "Middle square";
          spriteObj.transform.parent = square;
          spriteObj.transform.localRotation = Quaternion.identity;

          for (var j = 0; j < 2; j++) {
            float sign = j == 0 ? 1 : -1;

            if (dotLinesCoordinates.Contains(new DotLineCoordinates(x, y, sign)))
              continue;
            var spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            spriteRenderer.transform.parent = spriteObj.transform;

            spriteRenderer.transform.localPosition = new Vector2(0.0f, 0.5f * sign);
            spriteRenderer.transform.localRotation = Quaternion.identity;

            spriteRenderer.sprite = spritesOrder[y];
          }

          var middle = spriteObj.AddComponent<SpriteRenderer>();
          middle.transform.parent = spriteObj.transform;

          middle.transform.localPosition = new Vector2(0.0f, 0.0f);
          middle.transform.localRotation = Quaternion.identity;

          middle.sprite = middleVerticalLines;
        } else {
          var spriteRenderer = new GameObject("Grid" + (x, y)).AddComponent<SpriteRenderer>();
          spriteRenderer.transform.parent = squares[index].transform;
          spriteRenderer.transform.localPosition = new Vector2(0.0f, 0.0f);
          spriteRenderer.transform.localRotation = Quaternion.identity;
          spriteRenderer.sprite = spritesOrder[y];
          switch (y) {
            // blue
            case 0:
              spriteRenderer.name = "Blue";
              break;
            // red
            case 4:
              spriteRenderer.name = "Red";

              break;
            default:
              break;
          }
        }
      }

    // add the text script for win conditions here
    for (var i = 0; i < dotLinesCoordinates.Count; i++) {
      var x = dotLinesCoordinates[i].x;
      var y = dotLinesCoordinates[i].y;
      var index = x + y * sizeX;
      var gameObjDotLine = new GameObject();
      gameObjDotLine.AddComponent<DetectUnitsVertical>();
      gameObjDotLine.name = "DotLine" + (x, y);

      var dotLineRender = gameObjDotLine.AddComponent<SpriteRenderer>();
      dotLineRender.transform.parent = squares[index].transform.GetChild(0);
      dotLineRender.sprite = dotLine;
      dotLineRender.transform.localPosition = new Vector2(0.0f, 0.5f * dotLinesCoordinates[i].isUp);
      dotLineRender.transform.localRotation = Quaternion.identity;
    }
  }

  public static UnitRenderer PieceInFront(UnitRenderer piece, Vector2Int inFront,
                                          ref List<UnitRenderer> pieces) {
    int index = pieces.IndexOf(piece);
    if (index == -1)
      return null;  // Piece not found
    // at the edges

    if (index % sizeX == 3 && inFront.x == 1)
      return null;
    if (index % sizeX == 0 && inFront.x == -1)
      return null;
    var newIndex = index + inFront.x + inFront.y * sizeX;

    // Check if the new index is within bounds
    if (newIndex >= 0 && newIndex < pieces.Count)
      return pieces[newIndex];

    return null;
  }

  public static UnitRenderer PieceInFrontWithPadding(UnitRenderer piece, Vector2Int inFront,
                                                     ref List<UnitRenderer> pieces) {
    int index = pieces.IndexOf(piece);
    if (index == -1)
      return null;  // Piece not found

    var newIndex = index + inFront.x + inFront.y * sizeX;

    // Check if the new index is within bounds
    if (newIndex >= sizeX && newIndex < pieces.Count - sizeX)
      return pieces[newIndex];

    return null;
  }

  public UnitRenderer[] GetEmptySquares(SquareType type) {
    if (type == SquareType.RED)
      return pieces.TakeLast(sizeX).Where(x => x.GetUnitSettings().unitSettings == null).ToArray();
    if (type == SquareType.BLUE)
      return pieces.Take(sizeX).Where(x => x.GetUnitSettings().unitSettings == null).ToArray();
    Debug.Assert(false, "Invalid square type");
    return null;
  }
  public UnitRenderer[] GetSquares(SquareType type) {
    if (type == SquareType.RED)
      return pieces.TakeLast(sizeX).ToArray();
    if (type == SquareType.BLUE)
      return pieces.Take(sizeX).ToArray();
    Debug.Assert(false, "Invalid square type");
    return null;
  }
  public static List<UnitRenderer> GetAllPieces(SquareType type, ref List<UnitRenderer> pieces) {
    if (type == SquareType.RED)
      return pieces.FindAll(x => x.GetUnitSettings().unitSettings != null &&
                                 x.GetUnitSettings().isRed);
    if (type == SquareType.BLUE)
      return pieces.FindAll(x => x.GetUnitSettings().unitSettings != null &&
                                 !x.GetUnitSettings().isRed);

    Debug.Assert(false, "Invalid square type");
    return null;
  }

  private void DeleteBoard() {
    if (squares.Count > 0)
      for (var x = 0; x < squares.Count; x++) DestroyImmediate(squares[x]);
  }
}
