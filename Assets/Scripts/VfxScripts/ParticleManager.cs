using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem[] allEffects;


    void Start()
    {
        allEffects = GetComponentsInChildren<ParticleSystem>();
    }

    public void PlayEffects()
    {
        foreach (ParticleSystem p in allEffects)
        {
            p.Stop();
            p.Play();
        }
    }
}
