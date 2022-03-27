using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NtrIdolAri : MonoBehaviour
{
    [Header("Objects")]
    public ParticleSystem idolClapEffect;
    public ParticleSystem idolWinkEffect;

    public void ClapParticle()
    {
        idolClapEffect.Play();
    }

    public void WinkParticle()
    {
        idolWinkEffect.Play();
    }
}
