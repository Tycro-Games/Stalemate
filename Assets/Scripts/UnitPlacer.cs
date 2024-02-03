using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    [SerializeField] private ScriptableUnitSettings unitSettings;
    [SerializeField] private UnitSettingsEvent onUnitSettingsChanged;

    public void SetUnitSettings(ScriptableUnitSettings settings)
    {
        unitSettings = settings;
        onUnitSettingsChanged?.Invoke(settings);
    }

    private void Reset()
    {
        unitSettings = null;
    }
}