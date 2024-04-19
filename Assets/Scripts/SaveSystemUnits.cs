using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SerializableList<T>
{
    public List<T> list = new();
}

public class SaveSystemUnits : MonoBehaviour
{
    [SerializeField] private Transform redParent;
    [SerializeField] private Transform blueParent;
    private UnitUIRenderer[] unitUIRed;
    private UnitUIRenderer[] unitUIBlue;
    private List<ScriptableUnitSettings> allUnits = new();
    private SerializableList<int> redUnits = new();
    private SerializableList<int> blueUnits = new();
    [SerializeField] private ScriptableUnitSettings fogOfWar;
    [SerializeField] private readonly string filePathRed = "Assets/SaveData/UnitsRed.json";
    [SerializeField] private readonly string filePathBlue = "Assets/SaveData/UnitsBlue.json";
    [SerializeField] private readonly string filePathWon = "Assets/SaveData/DidRedWon.json";

    private void Start()
    {
        allUnits = Resources.LoadAll<ScriptableUnitSettings>("UnitSettings").ToList();
        allUnits.Remove(fogOfWar);
        //load stuff
        LoadData();
    }

    public void LoadData()
    {
        if (!File.Exists(filePathBlue))
        {
            Debug.Log("No data to be loaded");
            return;
        }

        var json = File.ReadAllText(filePathRed);

        // Convert JSON to list of integers
        redUnits = JsonUtility.FromJson<SerializableList<int>>(json);
        if (redUnits.list.Count == 0) return;
        var i = 0;
        foreach (var red in unitUIRed) red.SetUnitSettings(allUnits[redUnits.list[i++]]);

        json = File.ReadAllText(filePathBlue);

        // Convert JSON to list of integers
        blueUnits = JsonUtility.FromJson<SerializableList<int>>(json);
        if (blueUnits.list.Count == 0) return;
        i = 0;
        foreach (var blue in unitUIBlue) blue.SetUnitSettings(allUnits[blueUnits.list[i++]]);
    }

    private void Awake()
    {
        unitUIRed = redParent.GetComponentsInChildren<UnitUIRenderer>();
        unitUIBlue = blueParent.GetComponentsInChildren<UnitUIRenderer>();
    }

    private int GetID(ScriptableUnitSettings unitSettings)
    {
        return allUnits.FindIndex(u => u == unitSettings);
    }

    public void Serialize(bool redWon)
    {
        var serializeRed = new SerializableList<int>();
        var serializeBlue = new SerializableList<int>();

        // Populate redUnits and blueUnits lists with ScriptableUnitSettings
        foreach (var blue in unitUIBlue) serializeBlue.list.Add(GetID(blue.GetUnitSettings()));

        foreach (var red in unitUIRed) serializeRed.list.Add(GetID(red.GetUnitSettings()));

        // Now you can serialize redUnits and blueUnits using Unity's JSON utility
        var redUnitsJson = JsonUtility.ToJson(serializeRed);
        var blueUnitsJson = JsonUtility.ToJson(serializeBlue);

        // Optionally, you can save the JSON strings to files or send them over the network, etc.
        // For example, if you want to save them to files:
        File.WriteAllText(filePathRed, redUnitsJson);
        File.WriteAllText(filePathBlue, blueUnitsJson);
        File.WriteAllText(filePathWon, redWon ? "true" : "false");
    }
}