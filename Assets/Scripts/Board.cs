using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class Board : MonoBehaviour
{
    [SerializeField] private static int sizeX = 4;
    [SerializeField] private static int sizeY = 5;

    [SerializeField] private Color top = Color.red;
    [SerializeField] private Color middle = Color.yellow;
    [SerializeField] private Color buttom = Color.blue;

    public GameObject[] squares = new GameObject[sizeX * sizeY];
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
            squares[index] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var square = squares[index].transform;
            square.parent = transform;
            square.name = (x, y).ToString();
            square.position = new Vector2(x, y);
            var squareMaterial = ResetSquareColours(squareShader, index);

            squareRenderers[x, y] = square.gameObject.GetComponent<MeshRenderer>();

            squareRenderers[x, y].material = squareMaterial;

            //// Create piece sprite renderer for current square
            //var pieceRenderer = new GameObject("Piece").AddComponent<SpriteRenderer>();
            //pieceRenderer.transform.parent = square;
            //pieceRenderer.transform.position = square.position;
            //squarePieceRenderers[x, y] = pieceRenderer;
        }
    }

    private Material ResetSquareColours(Shader shader, int index)
    {
        var material = new Material(shader);
        //replace with switch


        if (index < sizeX) material.color = buttom;
        if (index >= sizeX * (sizeY - 1)) material.color = top;
        if (index >= sizeX * 2 && index < sizeX * 3) material.color = middle;

        return material;
    }

    private void DeleteBoard()
    {
        // Check if the squares array is not null
        if (squares != null)
            for (var y = 0; y < sizeY; y++)
            for (var x = 0; x < sizeX; x++)
                // Check if the element in the array is not null
                if (squares[x + y * sizeX] != null)
                    // Destroy the game object
                    DestroyImmediate(squares[x + y * sizeX]);
    }
}