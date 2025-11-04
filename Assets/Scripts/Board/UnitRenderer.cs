using System;
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
        isKillable = true;
    }

    public ScriptableUnitSettings unitSettings;
    public bool isRed;
    public bool isKillable;
    public void Reset()
    {
        isRed = true;
        unitSettings = null;
        isKillable = true;
    }
}

[RequireComponent(typeof(SpriteRenderer))]
public class UnitRenderer : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private UnitBoardInfo unitSettings;
    [SerializeField]
    private float alpha = 1.0f;

    private int hp = 0;
    public bool hasPerformedAction = false;

    public int GetHp()
    {
        return hp;
    }
    public event Action<int> onDamageTaken;
    public event Action<int> onUnitSetHp;
    public UnitData Clone()
    {
        var clone =
            new UnitData { spriteRenderer = spriteRenderer, unitSettings = unitSettings, alpha = alpha, hp = hp };

        return clone;
    }
    public bool GetHasPerformedAction()
    {
        return hasPerformedAction;
    }
    public void MakeKillable()
    {
        unitSettings.isKillable = true;
    }
    public void MakeUnkillable()
    {
        unitSettings.isKillable = false;
    }

    public UnitBoardInfo GetUnitSettings()
    {
        return unitSettings;
    }

    public bool TryToKill(int i = 1)
    {
        hp -= i;
        onDamageTaken?.Invoke(i);
        if (hp <= 0)
            return true;

        return false;
    }
    public void SetUnitSettings(UnitBoardInfo settings)
    {
        unitSettings = settings;
        Draw();
    }
    public void SetUnitSettingsAndHp(UnitBoardInfo settings, int previousHp = 0)
    {
        unitSettings = settings;
        InitializeHealth(previousHp);
        Draw();
    }

    private void Reset()
    {
        unitSettings.Reset();
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.material = new Material(Resources.Load<Material>("Materials/UnitMaterial"));
        spriteRenderer.material.SetFloat("_Opacity", alpha);
    }

    public void Draw()
    {
        var settings = unitSettings.unitSettings;
        if (settings == null)
        {
            spriteRenderer.sprite = null;
            hp = 0;
            onUnitSetHp?.Invoke(hp);

            return;
        }
        spriteRenderer.sprite = settings.sprite;
        spriteRenderer.material.SetFloat("_IsRed", unitSettings.isRed ? 1.0f : 0.0f);
        spriteRenderer.material.SetFloat("_IsFlipped", unitSettings.unitSettings.flip ? 1.0f : 0.0f);
    }

    public void InitializeHealth(int Hp)
    {
        if (unitSettings.unitSettings && unitSettings.unitSettings.hitsToDiePerTurn != 0)
        {
            hp = Hp == 0 ? unitSettings.unitSettings.hitsToDiePerTurn + GlobalSettings.GetHpModifier() : Hp;
            onUnitSetHp?.Invoke(hp);
        }
    }

    public void SetUnitData(UnitData unitData)
    {
        spriteRenderer = unitData.spriteRenderer;
        alpha = unitData.alpha;

        SetUnitSettingsAndHp(unitData.unitSettings, unitData.hp);
    }
}
