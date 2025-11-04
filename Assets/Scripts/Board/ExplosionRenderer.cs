using System;
using UnityEngine;

public class ExplosionRenderer : MonoBehaviour
{
    private ParticleSystemRenderer particleRenderer;
    private void Awake()
    {
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        particleRenderer.material = new Material(Resources.Load<Material>("Materials/ParticleExplosions"));
    }
    public void DrawColor(bool isRed)
    {
        particleRenderer.material.SetFloat("_IsRed", isRed ? 1.0f : 0.0f);
    }
}
