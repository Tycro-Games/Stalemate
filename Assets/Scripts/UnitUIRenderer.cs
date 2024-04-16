using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class UnitUIRenderer : MonoBehaviour
{
    [SerializeField] private Image spriteRenderer;
    [SerializeField] private ScriptableUnitSettings unitSettings;
    [SerializeField] private UnitSettingsEvent onUnitSettingsChanged;
    [SerializeField] private bool isRed;

    public void TriggerOnUnitSettings()
    {
        var cost = unitSettings.cost;
        var remainingPoints = RedBlueTurn.currentPoints;
        if (cost > remainingPoints) return;
        onUnitSettingsChanged?.Invoke(GetBoardInfo());
    }

    public UnitBoardInfo GetBoardInfo()
    {
        return new UnitBoardInfo(unitSettings, isRed);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
        spriteRenderer.sprite = unitSettings.sprite;
    }
}