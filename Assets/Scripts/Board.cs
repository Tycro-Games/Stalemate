using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class Board : MonoBehaviour
{
    [SerializeField] private int sizeX = 4;
    [SerializeField] private int sizeY = 5;

    [SerializeField] private Sprite[] spritesOrder;


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
            if (GUILayout.Button("Create Board"))
                // Call the CreateBoard method when the button is clicked
                board.CreateBoard();
        }
    }
#endif
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
            squares[index].AddComponent<BoxCollider2D>();
            var spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            spriteRenderer.transform.parent = squares[index].transform;
            var square = squares[index].transform;
            square.parent = transform;
            square.name = (x, y).ToString();
            square.position = new Vector2(x, y);


            spriteRenderer.sprite = spritesOrder[y];
            //var squareMaterial = ResetSquareColours(squareShader, index);

            //squareRenderers[x, y] = square.gameObject.GetComponent<MeshRenderer>();

            //squareRenderers[x, y].material = squareMaterial;

            //// Create piece sprite renderer for current square
            //var pieceRenderer = new GameObject("Piece").AddComponent<SpriteRenderer>();
            //pieceRenderer.transform.parent = square;
            //pieceRenderer.transform.position = square.position;
            //squarePieceRenderers[x, y] = pieceRenderer;
        }
    }


    private void DeleteBoard()
    {
        // Check if the squares array is not null
        if (squares.Length > 0)
            for (var x = 0; x < squares.Length; x++)
                // Check if the element in the array is not null
                if (squares[x] != null)
                    // Destroy the game object
                    DestroyImmediate(squares[x]);

        squares = new GameObject[sizeX * sizeY];
    }
}