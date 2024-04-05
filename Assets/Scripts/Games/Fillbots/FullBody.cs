using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Scripts_Fillbots
{
    public class FullBody : MonoBehaviour
    {
        [SerializeField] private SpriteMask mask;

        [SerializeField] private Sprite[] sprites;

        [SerializeField] private SpriteRenderer fullBody;
        [System.NonSerialized] public Color lampColorOff = new Color(0.635f, 0.635f, 0.185f);
        [System.NonSerialized] public Color lampColorOn = new Color(1f, 1f, 0.42f);

        public enum LampState
        {
            Off,
            On,
        }

        public void SetMask(int i)
        {
            mask.sprite = sprites[i];
        }

        public void SetLamp(LampState state)
        {
            if (state == LampState.On) fullBody.material.SetColor("_ColorAlpha", lampColorOn);
            else fullBody.material.SetColor("_ColorAlpha", lampColorOff);
        }
    }
}
