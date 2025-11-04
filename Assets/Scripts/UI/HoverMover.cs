using UnityEngine;
using UnityEngine.Events;

public class HoverMover : MonoBehaviour
{
    private Vector2 minLimits = Vector2.zero;
    private Vector2 maxLimits;
    [SerializeField]
    private UnityEvent onMove;
    [SerializeField]
    private UnityEvent onDisable;
    private bool needsSound = false;
    private float squareSize;

    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }

    void Start()
    {
        var board = FindObjectOfType<Board>();
        squareSize = board.GetSquareSize();
        maxLimits = new Vector2(squareSize, squareSize) * board.GetBoardSize();
        transform.localScale += new Vector3(squareSize / 2.0f, squareSize / 2.0f);
    }
    public void MoveClamped(Vector3 pos)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        float snappedX = Mathf.Round(pos.x / squareSize) * squareSize;
        float snappedY = Mathf.Round(pos.y / squareSize) * squareSize;

        Vector3 newPos = new Vector3(Mathf.Clamp(snappedX, minLimits.x, maxLimits.x),
                                     Mathf.Clamp(snappedY, minLimits.y, maxLimits.y), 0.0f);
        if (needsSound)
        {
            needsSound = false;

            onMove?.Invoke();
        }
        if (!transform.position.Equals(newPos))
            needsSound = true;

        transform.position = newPos;
    }

    void OnDisable()
    {
        needsSound = true;
        onDisable?.Invoke();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)((minLimits + maxLimits) / 2.0f), (maxLimits));
    }
}
