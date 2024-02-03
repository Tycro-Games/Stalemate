using Bogadanul.Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
internal struct DotLineCoordinates
{
    public int x;
    public int y;
    public float isUp;

    public DotLineCoordinates(int x, int y, float sign)
    {
        this.x = x;
        this.y = y;
        isUp = sign;
    }
}


public class Board : MonoBehaviour
{
    //raycaster
    [SerializeField] private LayerMask layerToPlace = 0;
    private Camera cam;
    private CursorController cursorController;

    private void Start()
    {
        cam = Camera.main;
        cursorController = FindObjectOfType<CursorController>();
        cursorController.OnMovement += Place;
    }

    private void OnDisable()
    {
        cursorController.OnMovement -= Place;
    }


    //board
    [SerializeField] private int sizeX = 4;
    [SerializeField] private int sizeY = 5;

    [SerializeField] private Sprite[] spritesOrder;
    [SerializeField] private Sprite middleVerticalLines;
    [SerializeField] private List<DotLineCoordinates> dotLinesCoordinates = new();
    [SerializeField] private Sprite dotLine;
    [HideInInspector] public bool shouldUpdate;


    public GameObject[] squares;
#if UNITY_EDITOR
    [CustomEditor(typeof(Board))]
    public class BoardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var board = (Board)target;

            // Create a button outside the inspector
            if (GUILayout.Button("Manual Update") || board.shouldUpdate)
            {
                // Call the CreateBoard method when the button is clicked
                board.CreateBoard();
                board.shouldUpdate = false;
            }
        }
    }
#endif
    private void OnValidate()
    {
        shouldUpdate = true;
    }

    private void CreateBoard()
    {
        DeleteBoard();

        var squareShader = Shader.Find("Unlit/Color");
        var squareRenderers = new MeshRenderer[sizeX, sizeY];
        //var squarePieceRenderers = new SpriteRenderer[sizeX, sizeY];

        for (var y = 0; y < sizeY; y++)
        for (var x = 0; x < sizeX; x++)
        {
            // Create square
            var index = x + y * sizeX;
            squares[index] = new GameObject();
            squares[index].AddComponent<BoxCollider>();

            var square = squares[index].transform;
            square.parent = transform;
            square.name = (x, y).ToString();
            square.position = new Vector2(x, y);

            if (y == sizeY / 2)
            {
                for (var j = 0; j < 2; j++)
                {
                    float sign = j == 0 ? 1 : -1;

                    if (dotLinesCoordinates.Contains(new DotLineCoordinates(x, y, sign)))
                        continue;
                    var spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
                    spriteRenderer.transform.parent = squares[index].transform;

                    spriteRenderer.transform.localPosition = new Vector2(0.0f, 0.5f * sign);

                    spriteRenderer.sprite = spritesOrder[y];
                }

                var middle = new GameObject().AddComponent<SpriteRenderer>();
                middle.transform.parent = squares[index].transform;

                middle.transform.localPosition = new Vector2(0.0f, 0.0f);

                middle.sprite = middleVerticalLines;
            }
            else
            {
                var spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
                spriteRenderer.transform.parent = squares[index].transform;
                spriteRenderer.transform.localPosition = new Vector2(0.0f, 0.0f);
                spriteRenderer.sprite = spritesOrder[y];
            }
            //var squareMaterial = ResetSquareColours(squareShader, index);
            //squareRenderers[x, y] = square.gameObject.GetComponent<MeshRenderer>();
            //squareRenderers[x, y].material = squareMaterial;
            //// Create piece sprite renderer for current square
            //var pieceRenderer = new GameObject("Piece").AddComponent<SpriteRenderer>();
            //pieceRenderer.transform.parent = square;
            //pieceRenderer.transform.position = square.position;
            //squarePieceRenderers[x, y] = pieceRenderer;
        }

        for (var i = 0; i < dotLinesCoordinates.Count; i++)
        {
            var x = dotLinesCoordinates[i].x;
            var y = dotLinesCoordinates[i].y;
            var index = x + y * sizeX;

            var dotLineRender = new GameObject().AddComponent<SpriteRenderer>();
            dotLineRender.transform.parent = squares[index].transform;
            dotLineRender.sprite = dotLine;
            dotLineRender.transform.localPosition = new Vector2(0.0f, 0.5f * dotLinesCoordinates[i].isUp);
        }
    }

    public GameObject NodeFromInput(Vector2 position)
    {
        if (Physics.Raycast(cam.ScreenPointToRay(position), out var hit, 50, layerToPlace))
            return hit.collider.gameObject;
        return null;
    }

    private void DeleteBoard()
    {
        if (squares == null)
            squares = new GameObject[sizeX * sizeY];

        if (squares.Length > 0)
            for (var x = 0; x < squares.Length; x++)
                // Check if the element in the array is not null
                if (squares[x] != null)
                    DestroyImmediate(squares[x]);
    }

    public void Place(Vector3 input)
    {
        var place = NodeFromInput(input);
        if (place == null) return;

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