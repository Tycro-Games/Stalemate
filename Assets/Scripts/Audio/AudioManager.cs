using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }


    //event refs

    [field: Header("Movement Sounds")]
    [field:SerializeField] public EventReference moveSound { get; private set; }
    [field: SerializeField] public EventReference boostSound { get; private set; }
    [field: SerializeField] public EventReference shootSound { get; private set; }

    [field: Header("UI Sounds")]
    [field: SerializeField] public EventReference peakSound { get; private set; }
    [field: SerializeField] public EventReference endTurnSound { get; private set; }
    [field: SerializeField] public EventReference hoverChangeSound { get; private set; }
    [field: SerializeField] public EventReference selectUnit { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("too many audio managers");
        }

        instance = this;
    }

    public void PlayOneShot(EventReference eventReference)
    {
        RuntimeManager.PlayOneShot(eventReference);
    }
}
