using UnityEngine;

namespace HeavenStudio.Util
{
    public static class ParticleSystemHelpers
    {
        public static void PlayScaledAsync(this ParticleSystem particleSystem, float timeScale, bool respectDistance = false)
        {
            SetAsyncScaling(particleSystem, timeScale, respectDistance);
            particleSystem.Play();
        }

        public static void SetAsyncScaling(this ParticleSystem particleSystem, float timeScale, bool respectDistance = false)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            main.simulationSpeed = main.simulationSpeed / Conductor.instance.pitchedSecPerBeat * timeScale;
            // addition by Yin
            if (respectDistance)
            {
                ParticleSystem.EmissionModule emission = particleSystem.emission;
                emission.rateOverDistanceMultiplier = Conductor.instance.pitchedSecPerBeat * timeScale * 4; // i don't know why 4 is the magic number
            }
        }

        public static void PlayScaledAsyncAllChildren(this ParticleSystem particleSystem, float timeScale)
        {
            particleSystem.PlayScaledAsync(timeScale);
            
            foreach (var p in particleSystem.GetComponentsInChildren<ParticleSystem>())
            {
                if (p == particleSystem) continue;
                p.PlayScaledAsyncAllChildren(timeScale);
            }
        }
    }
}

