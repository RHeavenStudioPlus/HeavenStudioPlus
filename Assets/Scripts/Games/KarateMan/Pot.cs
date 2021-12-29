using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class Pot : PlayerActionObject
    {
        public float startBeat;
        private Animator anim;

        [SerializeField]
        private GameObject Sprite;

        private void Start()
        {
            PlayerActionInit(this.gameObject);
            anim = GetComponent<Animator>();

            Sprite.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }

        private void Update()
        {
            float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.15f);
            anim.Play("PotThrow", 0, normalizedBeatAnim);
            anim.speed = 0;

            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 1);

            StateCheck(normalizedBeat, KarateMan.instance.EligibleHits);
        }
    }
}