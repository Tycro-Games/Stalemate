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

    public void TriggerOnUnitSettings()
    {
        onUnitSettingsChanged?.Invoke(unitSettings);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<Image>();
        spriteRenderer.sprite = unitSettings.sprite;
        spriteRenderer.color = unitSettings.color;
        var x = 1.0f;
        var z = 1.0f;
        transform.localScale = new Vector3(x, unitSettings.flipY ? -1.0f : 1.0f, z);
    }
}