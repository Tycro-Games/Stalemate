using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhaseSystem : MonoBehaviour
{
    [SerializeField] private List<Phase> phases = new();
    private int index = 0;

    private void Start()
    {
        StartPhase();
    }

    public void StartPhase()
    {
        Debug.Log("Starting phase" + phases[index].name);

        phases[index].OnStart?.Invoke();
    }

    public void EndPhase()
    {
        Debug.Log("Ending phase" + phases[index].name);
        var phase = phases[index];

        index = (index + 1) % phases.Count;
        phase.OnEnd?.Invoke();
    }
}