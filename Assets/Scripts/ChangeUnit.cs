using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChangeUnit : MonoBehaviour
{
    private UnitUIRenderer[] unitUIRed;
    [SerializeField] private Transform redParent;
    private int index = -1;

    [SerializeField] private RectTransform highlight;
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
        //only of the cost we have
        allUnitSettings = SaveSystemUnits.GetAllUnitSettings().FindAll(x => x.cost == cost).ToList();
        allUnitSettings.Remove(toChangeUnit);
        //hope for the love of god that we have at least one unit
        var newUnit = allUnitSettings[Random.Range(0, allUnitSettings.Count)];
        //finally set the new unit
        unitUIRed[index].SetUnitSettings(newUnit);
    }

    public void ChooseRandomCostUnit()
    {
        index = Random.Range(0, unitUIRed.Length);
        //display the unit selected
        Debug.Log("index of selected:" + index);
        //var transforms = new List<RectTransform>();
        //transforms.AddRange(unitUIRed.Select(q => q.GetComponent<RectTransform>()));
        highlight.anchoredPosition = new Vector2(unitUIRed[index].GetComponent<RectTransform>().anchoredPosition.x,
            highlight.anchoredPosition.y);
    }
}