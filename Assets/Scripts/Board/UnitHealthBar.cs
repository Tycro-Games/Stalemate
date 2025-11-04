using UnityEngine;

[RequireComponent(typeof(UnitRenderer))]
public class UnitHealthBar : MonoBehaviour
{
    [SerializeField]
    private int maxHp;

    private int currentHp;
    private GameObject healthUnitPrefab;
    private GameObject healthBarContainer;
    UnitRenderer unitRenderer;
    private readonly float offsetHealth = 0.08f;
    private void Awake()
    {
        unitRenderer = GetComponent<UnitRenderer>();
        unitRenderer.onUnitSetHp += InitializeHealth;
        unitRenderer.onDamageTaken += ApplyDamage;

        CreateHealthBar();
    }

    private void CreateHealthBar()
    {
        healthUnitPrefab = Resources.Load<GameObject>("Prefabs/HealthUnitPrefab");
        if (healthUnitPrefab == null)
        {
            Debug.LogError("Health unit prefab not found in Resources!");
            return;
        }

        // Create a container for the health bar above the unit
        healthBarContainer = Instantiate(Resources.Load<GameObject>("Prefabs/HealthBarContainerPrefab"));
        healthBarContainer.transform.SetParent(transform);
        float sign = 1.0f;
        if (unitRenderer.GetUnitSettings().isRed)
        {
            sign = -1.0f;
        }
        healthBarContainer.transform.localPosition = new Vector3(0, offsetHealth * sign, 0);
    }

    void OnDisable()
    {
        unitRenderer.onUnitSetHp -= InitializeHealth;
        unitRenderer.onDamageTaken -= ApplyDamage;
    }
    public void InitializeHealth(int baseHp)
    {
        maxHp = baseHp;
        currentHp = maxHp;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {

        foreach (Transform child in healthBarContainer.transform)
        {
            Destroy(child.gameObject);
        }

        float spacing = 0.04f;

        float totalWidth = (currentHp - 1) * spacing;

        float startingX = -totalWidth / 2;

        float sign = 1.0f;
        if (unitRenderer.GetUnitSettings().isRed)
        {
            sign = -1.0f;
        }
        healthBarContainer.transform.localPosition = new Vector3(0, offsetHealth * sign, 0);
        for (int i = 0; i < currentHp; i++)
        {
            GameObject healthUnit = Instantiate(healthUnitPrefab, healthBarContainer.transform);
            healthUnit.transform.localPosition = new Vector3(startingX + i * spacing, 0, 0);
        }
    }

    public void ApplyDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        UpdateHealthBar();
    }
}
