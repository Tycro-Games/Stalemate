using UnityEngine;
using UnityEngine.Events;

public class SideDisplay : MonoBehaviour
{
    [SerializeField] private const string red = "Red";
    [SerializeField] private const string blue = "Blue";
    [SerializeField] private UnityEvent OnRed;
    [SerializeField] private UnityEvent OnBlue;

    public void OnSideDisplay(string side)
    {
        if (side == red)
        {
            OnRed?.Invoke();
        }
        else if (side == blue)
        {
            OnBlue?.Invoke();
        }
        
    }
}
