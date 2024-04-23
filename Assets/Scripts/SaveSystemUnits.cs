using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SerializableList<T>
{
    public List<T> list = new();
}

public class SaveSystemUnits : MonoBehaviour
{
    [SerializeField] private Transform redParent;

    [SerializeField] private Transform blueParent;

    //or the winner
    private UnitUIRenderer[] unitUIRed;

    private UnitUIRenderer[] unitUIBlue;
    private List<ScriptableUnitSettings> allUnits = new();
    private SerializableList<int> redWinUnits = new();
    private SerializableList<int> blueUnits = new();
    [SerializeField] private ScriptableUnitSettings fogOfWar;
    [SerializeField] private readonly string filePathRed = "UnitsRed.json";
    [SerializeField] private readonly string filePathBlue = "UnitsBlue.json";
    [SerializeField] private readonly string filePathWon = "DidRedWon.json";
    [SerializeField] private string persistentPath;
    [SerializeField] private UnityEvent onStart;

    private void Start()
    {
        persistentPath = Application.persistentDataPath;
        allUnits = GetAllUnitSettings();
        allUnits.Remove(fogOfWar);
        onStart?.Invoke();
        //load stuff
        //load winner
    }

    public void SerializeWinner()
    {
        var didRedWin = bool.Parse(File.ReadAllText(persistentPath + filePathWon));

        var serializeRed = new SerializableList<int>();
        var serializeBlue = new SerializableList<int>();

        if (didRedWin)
        {
            // Populate redUnits and blueUnits lists with ScriptableUnitSettings
            foreach (var blue in unitUIBlue) serializeBlue.list.Add(GetID(blue.GetUnitSettings()));

            foreach (var red in unitUIRed) serializeRed.list.Add(GetID(red.GetUnitSettings()));

            // Now you can serialize redUnits and blueUnits using Unity's JSON utility
            var redUnitsJson = JsonUtility.ToJson(serializeRed);
            var blueUnitsJson = JsonUtility.ToJson(serializeBlue);

            // Optionally, you can save the JSON strings to files or send them over the network, etc.
            // For example, if you want to save them to files:
            File.WriteAllText(persistentPath + filePathBlue, redUnitsJson);
            File.WriteAllText(persistentPath + filePathRed, blueUnitsJson);
        }
        else
        {
            // Populate redUnits and blueUnits lists with ScriptableUnitSettings
            foreach (var blue in unitUIBlue) serializeBlue.list.Add(GetID(blue.GetUnitSettings()));

            foreach (var red in unitUIRed) serializeRed.list.Add(GetID(red.GetUnitSettings()));

            // Now you can serialize redUnits and blueUnits using Unity's JSON utility
            var redUnitsJson = JsonUtility.ToJson(serializeRed);
            var blueUnitsJson = JsonUtility.ToJson(serializeBlue);

            // Optionally, you can save the JSON strings to files or send them over the network, etc.
            // For example, if you want to save them to files:
            File.WriteAllText(persistentPath + filePathRed, redUnitsJson);
            File.WriteAllText(persistentPath + filePathBlue, blueUnitsJson);
        }
    }

    public static List<ScriptableUnitSettings> GetAllUnitSettings()
    {
        return Resources.LoadAll<ScriptableUnitSettings>("UnitSettings").ToList();
    }

    private bool LoadOneSide(string fileToRead, ref UnitUIRenderer[] unitUI, ref SerializableList<int> units)
    {
        var json = File.ReadAllText(persistentPath + fileToRead);

        // Convert JSON to list of integers
        units = JsonUtility.FromJson<SerializableList<int>>(json);
        if (units.list.Count == 0) return false;
        var i = 0;
        foreach (var unit in unitUI) unit.SetUnitSettings(allUnits[units.list[i++]]);
        return true;
    }

    public void LoadData()
    {
        if (!File.Exists(persistentPath + filePathBlue))
        {
            Debug.Log("No data to be loaded");
            return;
        }

        if (!LoadOneSide(filePathRed, ref unitUIRed, ref redWinUnits)) return;

        if (!LoadOneSide(filePathBlue, ref unitUIBlue, ref blueUnits)) return;
    }

    public void LoadWinnerData()
    {
        if (!File.Exists(persistentPath + filePathBlue))
        {
            Debug.Log("No data to be loaded");
            return;
        }

        var didRedWin = bool.Parse(File.ReadAllText(persistentPath + filePathWon));
        if (didRedWin)
        {
            //red is the losers by default
            if (!LoadOneSide(filePathRed, ref unitUIBlue, ref blueUnits)) return;
            if (!LoadOneSide(filePathBlue, ref unitUIRed, ref redWinUnits)) return;
            SetTheColors(false);
        }
        else
        {
            if (!LoadOneSide(filePathRed, ref unitUIRed, ref redWinUnits)) return;
            if (!LoadOneSide(filePathBlue, ref unitUIBlue, ref blueUnits)) return;
            SetTheColors(true);
        }
    }

    private void SetTheColors(bool redWon)
    {
        foreach (var red in unitUIRed)
        {
            red.SetIsRed(redWon);
            red.Draw();
        }

        foreach (var blue in unitUIBlue)
        {
            blue.SetIsRed(!redWon);
            blue.Draw();
        }
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
        File.WriteAllText(persistentPath + filePathRed, redUnitsJson);
        File.WriteAllText(persistentPath + filePathBlue, blueUnitsJson);
        File.WriteAllText(persistentPath + filePathWon, redWon ? "true" : "false");
    }
}