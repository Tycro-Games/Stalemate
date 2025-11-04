using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeUnit : MonoBehaviour
{
    private UnitUIRenderer[] unitUIRed;
    [SerializeField]
    private Transform redParent;
    private int index = -1;

    [SerializeField]
    private RectTransform highlight;
    private List<ScriptableUnitSettings> allUnitSettings = new();

    private void Start()
    {
        unitUIRed = redParent.GetComponentsInChildren<UnitUIRenderer>();
        ChooseRandomCostUnit();
    }

    public void SwitchUnitWithRandomOne()
    {
        var toChangeUnit = unitUIRed[index].GetUnitSettings();
        var cost = toChangeUnit.cost;
        allUnitSettings = SaveSystemUnits.GetAllUnitSettings().FindAll(x => x.cost == cost).ToList();
        allUnitSettings.Remove(toChangeUnit);
        var newUnit = allUnitSettings[Random.Range(0, allUnitSettings.Count)];
        unitUIRed[index].SetUnitSettings(newUnit);
    }

    public void ChooseRandomCostUnit()
    {
        var randIndex = Random.Range(0, unitUIRed.Length);

        while (randIndex == index)
            randIndex = Random.Range(0, unitUIRed.Length);
        index = randIndex;
        Debug.Log("index of selected:" + index);

        highlight.anchoredPosition = new Vector2(unitUIRed[index].GetComponent<RectTransform>().anchoredPosition.x,
                                                 highlight.anchoredPosition.y);
    }
}
