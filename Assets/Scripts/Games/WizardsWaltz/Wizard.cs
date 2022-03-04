using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.WizardsWaltz
{
    public class Wizard : MonoBehaviour
    {
        public Animator animator;
        public GameObject shadow;

        private WizardsWaltz game;
        private float songPos;

        private void Awake()
        {
            game = WizardsWaltz.instance;
        }

        void Update()
        {
            songPos = Conductor.instance.songPositionInBeats;
            var am = game.beatInterval / 2f;
            var x = Mathf.Sin(Mathf.PI * songPos / am) * 6;
            var y = Mathf.Cos(Mathf.PI * songPos / am) * 1.5f;
            var scale = 1 - Mathf.Cos(Mathf.PI * songPos / am) * 0.25f;
            
            transform.position = new Vector3(x, 2 + y, -scale);
            shadow.transform.position = new Vector3(x, -2.5f + y, -scale + 0.1f);

            var xscale = scale;
            if (y > 0) xscale *= -1;
            transform.localScale = new Vector3(xscale, scale, 1);
            shadow.transform.localScale = new Vector3(scale, scale, 1);
        }

        private void LateUpdate()
        {
            if (PlayerInput.Pressed(true))
            {
                animator.Play("Magic", 0, 0);
            }
        }

        public void Magic(Plant plant, bool hit)
        {
            animator.Play("Magic", 0, 0);

            if(plant == null)
            {
                // TODO: Play empty A press sound
                return;
            }
            if (hit)
            {
                Jukebox.PlayOneShotGame("wizardsWaltz/grow");
                plant.Bloom();
            }
            else
            {
                Jukebox.PlayOneShot("miss");
                plant.Eat();
            }
        }

    }
}