using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class UnitRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ScriptableUnitSettings unitSettings;

    public void SetUnitSettings(ScriptableUnitSettings settings)
    {
        unitSettings = settings;
        Draw();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Draw()
    {
        spriteRenderer.sprite = unitSettings.sprite;
        spriteRenderer.color = unitSettings.color;
        spriteRenderer.flipY = unitSettings.flipY;
    }
}