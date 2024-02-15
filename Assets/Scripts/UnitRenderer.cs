using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct UnitData
{
    public SpriteRenderer spriteRenderer;
    public ScriptableUnitSettings unitSettings;
    public float alpha;

    public int hp;
}

[RequireComponent(typeof(SpriteRenderer))]
public class UnitRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ScriptableUnitSettings unitSettings;
    [SerializeField] private float alpha = 1.0f;

    private int hp;

    public UnitData Clone()
    {
        var clone = new UnitData
        {
            spriteRenderer = spriteRenderer,
            unitSettings = unitSettings,
            alpha = alpha,
            hp = hp
        };


        return clone;
    }


    public ScriptableUnitSettings GetUnitSettings()
    {
        return unitSettings;
    }

    public bool TryToKill(int i = 1)
    {
        hp -= i;
        if (hp <= 0) return true;

        return false;
    }

    public void SetUnitSettings(ScriptableUnitSettings settings)
    {
        unitSettings = settings;

        Draw();
    }

    private void Reset()
    {
        unitSettings = null;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Draw()
    {
        if (unitSettings == null)
        {
            spriteRenderer.sprite = null;
            return;
        }

        hp = unitSettings.hp;
        spriteRenderer.sprite = unitSettings.sprite;
        var color = unitSettings.color;
        spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
        spriteRenderer.flipY = unitSettings.flipY;
    }

    public void SetUnitData(UnitData unitData)
    {
        spriteRenderer = unitData.spriteRenderer;
        alpha = unitData.alpha;
        hp = unitData.hp;
        SetUnitSettings(unitData.unitSettings);
    }
}