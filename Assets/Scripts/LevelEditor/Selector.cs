using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor
{
    public class Selector : MonoBehaviour
    {
        private bool clicked;

        public static Selector instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }
    }
}