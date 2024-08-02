using System;
using FMODUnity;
using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    [Serializable]
    public class TransformEvent : UnityEvent<Transform>
    {
    }

    [Serializable]
    public class Vector3Event : UnityEvent<Vector3>
    {
    }


    [Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }
    [Serializable]
    public class AudioEvent : UnityEvent<EventReference>
    {
    }

    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }
    [Serializable]
    public class UnitInfoEvent : UnityEvent<UnitBoardInfo>
    {
    }

    [Serializable]
    public class UnitRenderEvent : UnityEvent<UnitRenderer>
    {
    }
}