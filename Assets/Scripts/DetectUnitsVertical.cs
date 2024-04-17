using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectUnitsVertical : MonoBehaviour
{
    private UnitManager manager;

    private int y;

    //[SerializeField] private int defaultYRed = -1;
    private static int RedUnitsOnY = 0;
    private static int BlueUnitsOnY = 0;
    public static Action<int, int> onWinConditionChange;

    private void Start()
    {
        y = (int)transform.position.y;
        manager = GetComponentInParent<UnitManager>();
    }

    private void OnEnable()
    {
        UnitManager.onUnitManipulation += CheckY;
    }

    private void OnDisable()
    {
        UnitManager.onUnitManipulation -= CheckY;
    }

    public void CheckY()
    {
        RedUnitsOnY = 0;
        BlueUnitsOnY = 0;
        foreach (var redUnit in manager.GetRedUnits())
            if ((int)redUnit.transform.position.y <= y)
                RedUnitsOnY++;
        foreach (var blueUnit in manager.GetBlueUnits())
            if ((int)blueUnit.transform.position.y >= y)
                BlueUnitsOnY++;

        onWinConditionChange?.Invoke(RedUnitsOnY, BlueUnitsOnY);
    }

    private int ParseYFromName(string name)
    {
        // Split the name string to extract the part between the parentheses
        var startIndex = name.IndexOf('(') + 1;
        var endIndex = name.IndexOf(')');
        var coordinates = name.Substring(startIndex, endIndex - startIndex);

        // Split the coordinates string to extract the y value
        var parts = coordinates.Split(',');
        if (parts.Length == 2)
        {
            int yValue;
            if (int.TryParse(parts[1].Trim(), out yValue)) return yValue;
        }

        // Return a default value if parsing fails
        return -1;
    }
}