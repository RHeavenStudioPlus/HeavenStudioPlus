using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DrummingPractice
{
    public class Drummer : MonoBehaviour
    {

        [Header("References")]
        public Animator animator;
        public List<MiiFace> miiFaces;
        public SpriteRenderer face;

        public bool player = false;

        public int mii = 0;
        public int count = 0;

        private bool hitting = false;

        [System.Serializable]
        public class MiiFace
        {
            public List<Sprite> Sprites;
        }

        private void Update()
        {
            if (player && PlayerInput.Pressed())
            {
                Hit(false);
            }
        }

        public void SetFace(int type)
        {
            face.sprite = miiFaces[mii].Sprites[type];
        }

        public void Bop()
        {
            if (animator.IsAnimationNotPlaying())
                animator.Play("Bop", 0, 0);
        }

        public void Prepare(int type)
        {
            count = type;
            if (count % 2 == 0)
                animator.Play("PrepareLeft", 0, 0);
            else
                animator.Play("PrepareRight", 0, 0);
        }

        public void Hit(bool hit)
        {
            if (!hitting)
            {
                if (count % 2 == 0)
                    animator.Play("HitLeft", 0, 0);
                else
                    animator.Play("HitRight", 0, 0);
                count++;

                if (player)
                {
                    if (hit)
                        Jukebox.PlayOneShotGame("drummingPractice/hit");
                    else
                        Jukebox.PlayOneShotGame("drummingPractice/miss");
                }

                hitting = true;
            }
        }

        public void EndHit()
        {
            hitting = false;
        }

    }
}