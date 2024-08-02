using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    

    //event refs



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
