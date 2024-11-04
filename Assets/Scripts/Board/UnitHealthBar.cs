using UnityEngine;

[RequireComponent(typeof(UnitRenderer))]
public class UnitHealthBar : MonoBehaviour {
  [SerializeField]
  private int maxHp;

  private int currentHp;
  private GameObject healthUnitPrefab;
  private GameObject healthBarContainer;
  UnitRenderer unitRenderer;
  private void Awake() {
    // Load the health unit prefab from Resources
    unitRenderer = GetComponent<UnitRenderer>();
    unitRenderer.onUnitSetHp += InitializeHealth;
    unitRenderer.onDamageTaken += ApplyDamage;

    CreateHealthBar();
  }

  private void CreateHealthBar() {
    healthUnitPrefab = Resources.Load<GameObject>("Prefabs/HealthUnitPrefab");
    if (healthUnitPrefab == null) {
      Debug.LogError("Health unit prefab not found in Resources!");
      return;
    }

    // Create a container for the health bar above the unit
    healthBarContainer =
        Instantiate(Resources.Load<GameObject>("Prefabs/HealthBarContainerPrefab"));
    healthBarContainer.transform.SetParent(transform);
    float sign = 1.0f;
    if (unitRenderer.GetUnitSettings().isRed) {
      sign = -1.0f;
    }
    healthBarContainer.transform.localPosition =
        new Vector3(0, 0.08f * sign, 0);  // Position above the unit
  }

  void OnDisable() {
    unitRenderer.onUnitSetHp -= InitializeHealth;
    unitRenderer.onDamageTaken -= ApplyDamage;
  }
  public void InitializeHealth(int baseHp) {
    maxHp = baseHp;
    currentHp = maxHp;
    UpdateHealthBar();
  }

  private void UpdateHealthBar() {
    // Clear existing health units
    foreach (Transform child in healthBarContainer.transform) {
      Destroy(child.gameObject);
    }

    // Define spacing between health units
    float spacing = 0.2f;

    // Calculate the total width of all units
    float totalWidth = (currentHp - 1) * spacing;

    // Calculate the starting offset to center the units
    float startingX = -totalWidth / 2;

    float sign = 1.0f;
    if (unitRenderer.GetUnitSettings().isRed) {
      sign = -1.0f;
    }
    healthBarContainer.transform.localPosition =
        new Vector3(0, 0.08f * sign, 0);  // Position above the unit
    // Create a health unit for each point of current HP and position them
    for (int i = 0; i < currentHp; i++) {
      GameObject healthUnit = Instantiate(healthUnitPrefab, healthBarContainer.transform);
      healthUnit.transform.localPosition = new Vector3(startingX + i * spacing, 0, 0);
    }
  }

  public void ApplyDamage(int damage) {
    currentHp -= damage;
    currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    UpdateHealthBar();
  }
}
