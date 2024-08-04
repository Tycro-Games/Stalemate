using System;
using UnityEngine;




public class ExplosionRenderer : MonoBehaviour
{
    private ParticleSystemRenderer  particleRenderer;
    private void Awake()
    {
         particleRenderer = GetComponent<ParticleSystemRenderer>();

         particleRenderer.material = new Material(Resources.Load<Material>("Materials/ParticleExplosions"));
    }
    //only changes the material instance color
    public void DrawColor(bool isRed)
    {
         particleRenderer.material.SetFloat("_IsRed", isRed ? 1.0f : 0.0f);
        // particleRenderer.material.SetFloat("_IsFlipped", unitSettings.unitSettings.flip ? 1.0f : 0.0f);
    }
   

 
}