using UnityEngine;

[RequireComponent(typeof(UnitRenderer))]
public class UnitHealthBar : MonoBehaviour {
  [SerializeField]
  private int maxHp;

  private int currentHp;
  private GameObject healthUnitPrefab;
  private Transform healthBarContainer;
  UnitRenderer unitRenderer;
  private void Awake() {
    // Load the health unit prefab from Resources
    unitRenderer = GetComponent<UnitRenderer>();
    unitRenderer.onUnitSetHp += InitializeHealth;
    unitRenderer.onDamageTaken += ApplyDamage;
    healthUnitPrefab = Resources.Load<GameObject>("Prefabs/HealthUnitPrefab");
    if (healthUnitPrefab == null) {
      Debug.LogError("Health unit prefab not found in Resources!");
      return;
    }

    // Create a container for the health bar above the unit
    healthBarContainer = new GameObject("HealthBarContainer").transform;
    healthBarContainer.SetParent(transform);
    healthBarContainer.localPosition =
        new Vector3(0, 0.0f, 0) * healthBarContainer.localScale.x;  // Position above the unit
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
    foreach (Transform child in healthBarContainer) {
      Destroy(child.gameObject);
    }

    // Create a health unit for each point of current HP
    for (int i = 0; i < currentHp; i++) {
      GameObject healthUnit = Instantiate(healthUnitPrefab, healthBarContainer);
      healthUnit.transform.localPosition =
          new Vector3(i * 0.2f, 0, 0);  // Adjust spacing between units
    }
  }

  public void ApplyDamage(int damage) {
    currentHp -= damage;
    currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    UpdateHealthBar();
  }
}
