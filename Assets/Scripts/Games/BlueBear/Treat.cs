using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyBezierCurves;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_BlueBear
{
    public class Treat : PlayerActionObject
    {
        const float rotSpeed = 360f;

        public bool isCake;
        public float startBeat;

        bool flying = true;
        float flyBeats;

        [NonSerialized] public BezierCurve3D curve;

        private BlueBear game;
        
        private void Awake()
        {
            game = BlueBear.instance;

            flyBeats = isCake ? 3f : 2f;
        }

        private void Update()
        {
            if (flying)
            {
                var cond = Conductor.instance;

                float flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
                flyPos *= isCake ? 0.75f : 0.6f;
                transform.position = curve.GetPoint(flyPos);

                if (flyPos > 1f)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }

                float rot = isCake ? rotSpeed : -rotSpeed;
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (rot * Time.deltaTime));

                float normalizedBeat = cond.GetPositionFromMargin(startBeat + flyBeats, 1f);
                StateCheck(normalizedBeat);

                if (PlayerInput.Pressed())
                {
                    if (!isCake)
                    {
                        if (state.perfect)
                        {
                            flying = false;

                            Jukebox.PlayOneShotGame("blueBear/chompDonut");

                            SpawnCrumbs();

                            GameObject.Destroy(gameObject);
                        }
                    }
                }
                else if (PlayerInput.GetAnyDirection())
                {
                    if (isCake)
                    {
                        if (state.perfect)
                        {
                            flying = false;

                            Jukebox.PlayOneShotGame("blueBear/chompCake");

                            SpawnCrumbs();
                            
                            GameObject.Destroy(gameObject);
                        }
                    }
                }
            }
        }

        public override void OnAce()
        {
            flying = false;

            if (isCake)
            {
                game.headAndBodyAnim.Play("BiteL", 0, 0);
                Jukebox.PlayOneShotGame("blueBear/chompCake");
            }
            else
            {
                game.headAndBodyAnim.Play("BiteR", 0, 0);
                Jukebox.PlayOneShotGame("blueBear/chompDonut");
            }

            SpawnCrumbs();

            GameObject.Destroy(gameObject);
        }

        void SpawnCrumbs()
        {
            var crumbsGO = GameObject.Instantiate(game.crumbsBase, game.crumbsHolder);
            crumbsGO.SetActive(true);
            crumbsGO.transform.position = transform.position;

            var ps = crumbsGO.GetComponent<ParticleSystem>();
            var main = ps.main;
            var newGradient = new ParticleSystem.MinMaxGradient(isCake ? game.cakeGradient : game.donutGradient);
            newGradient.mode = ParticleSystemGradientMode.RandomColor;
            main.startColor = newGradient;
            ps.Play();
        }
    }
}
