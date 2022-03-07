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

        [System.Serializable]
        public class MiiFace
        {
            public List<Sprite> Sprites;
        }

    }
}