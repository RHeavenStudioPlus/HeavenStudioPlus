using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.WizardsWaltz
{
    public class Wizard : MonoBehaviour
    {

        private float songPos;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            songPos = Conductor.instance.songPositionInBeats;
            var x = Mathf.Sin(Mathf.PI * songPos / 2) * 6;
            var y = 2 + Mathf.Cos(Mathf.PI * songPos / 2) * 1.5f;
            var scale = 1 - Mathf.Cos(Mathf.PI * songPos / 2) * 0.25f;
            transform.position = new Vector3(x, y, 0);
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}