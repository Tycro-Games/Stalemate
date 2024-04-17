using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhaseSystem : MonoBehaviour
{
    [SerializeField] private List<Phase> phases = new();
    [SerializeField] private List<Phase> unitsAct = new();
    private int phaseIndex = 0;
    private int actionIndex = 0;

    private void Start()
    {
        StartNextPhase();
    }

    public void StartNextPhase()
    {
        Debug.Log("Starting phase " + phases[phaseIndex].name);

        phases[phaseIndex].OnStart?.Invoke();
    }

    public void EndPhase()
    {
        Debug.Log("Ending phase " + phases[phaseIndex].name);
        var phase = phases[phaseIndex];

        phaseIndex = (phaseIndex + 1) % phases.Count;
        phase.OnEnd?.Invoke();
    }

    public void EndAction()
    {
        Debug.Log("Ending action " + unitsAct[actionIndex].name);
        var action = unitsAct[actionIndex];

        actionIndex = (actionIndex + 1) % unitsAct.Count;
        action.OnEnd?.Invoke();
    }

    public void NextUnitAct()
    {
        Debug.Log("Starting unit act " + unitsAct[actionIndex].name);

        unitsAct[actionIndex].OnStart?.Invoke();
    }
}