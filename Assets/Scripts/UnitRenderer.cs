using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct UnitData
{
    public SpriteRenderer spriteRenderer;
    public UnitBoardInfo unitSettings;
    public float alpha;

    public int hp;
}

[Serializable]
public struct UnitBoardInfo
{
    public UnitBoardInfo(ScriptableUnitSettings settings, bool isRed)
    {
        unitSettings = settings;
        this.isRed = isRed;
    }

    public ScriptableUnitSettings unitSettings;
    public bool isRed;

    public void Reset()
    {
        isRed = true;
        unitSettings = null;
    }
}

[RequireComponent(typeof(SpriteRenderer))]
public class UnitRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private UnitBoardInfo unitSettings;
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


    public UnitBoardInfo GetUnitSettings()
    {
        return unitSettings;
    }

    public bool TryToKill(int i = 1)
    {
        hp -= i;
        if (hp <= 0) return true;

        return false;
    }

    public void SetUnitSettings(UnitBoardInfo settings)
    {
        unitSettings = settings;

        Draw();
    }

    private void Reset()
    {
        unitSettings.Reset();
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Draw()
    {
        var settings = unitSettings.unitSettings;
        if (settings == null)
        {
            spriteRenderer.sprite = null;
            return;
        }

        hp = settings.hitsToDiePerTurn;
        spriteRenderer.sprite = settings.sprite;
        //var color = unitSettings.color;
        //spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
        //spriteRenderer.flipY = unitSettings.flipY;
    }

    public void SetUnitData(UnitData unitData)
    {
        spriteRenderer = unitData.spriteRenderer;
        alpha = unitData.alpha;
        hp = unitData.hp;
        SetUnitSettings(unitData.unitSettings);
    }
}