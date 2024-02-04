using System;
using UnityEngine.Events;

[Serializable]
public struct Phase
{
    public string name;
    public UnityEvent OnStart;
    public UnityEvent OnEnd;
}