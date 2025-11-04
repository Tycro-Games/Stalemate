using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class UnitUIRenderer : MonoBehaviour
{
    [SerializeField]
    private Image spriteRenderer;
    [SerializeField]
    private ScriptableUnitSettings unitSettings;
    [SerializeField]
    private UnitInfoEvent onUnitInfoChanged;
    [SerializeField]
    private bool isRed;

    public void TriggerOnUnitSettings()
    {
        var cost = unitSettings.cost;
        var remainingPoints = RedBlueTurn.currentPoints;
        if (cost > remainingPoints)
            return;
        onUnitInfoChanged?.Invoke(GetBoardInfo());
    }

    public void SetUnitSettings(ScriptableUnitSettings _unitSettings)
    {
        unitSettings = _unitSettings;

        spriteRenderer.sprite = unitSettings.sprite;
        Draw();
    }

    public void SetIsRed(bool _isRed)
    {
        isRed = _isRed;
    }

    public ScriptableUnitSettings GetUnitSettings()
    {
        return unitSettings;
    }

    public UnitBoardInfo GetBoardInfo()
    {
        return new UnitBoardInfo(unitSettings, isRed);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<Image>();
        Draw();
    }

    public void Draw()
    {
        spriteRenderer.material = new Material(Resources.Load<Material>("Materials/UnitMaterial"));
        spriteRenderer.material.SetFloat("_IsRed", isRed ? 1.0f : 0.0f);
        spriteRenderer.material.SetFloat("_IsFlipped", unitSettings.flip ? 1.0f : 0.0f);

        spriteRenderer.sprite = unitSettings.sprite;
    }
}
