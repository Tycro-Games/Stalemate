using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class UnitRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ScriptableUnitSettings unitSettings;
    [SerializeField] private float alpha = 1.0f;
    private int hp;

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
}