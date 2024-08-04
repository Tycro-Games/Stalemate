using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] private EventReference eventReference;

    public void PlaySound()
    {
        AudioManager.instance.PlayOneShot(eventReference);
    }
}
