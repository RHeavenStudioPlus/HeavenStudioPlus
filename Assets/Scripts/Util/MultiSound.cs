using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RhythmHeavenMania.Util
{
    public class MultiSound : MonoBehaviour
    {
        private float startBeat;
        private int index;
        public List<Sound> sounds = new List<Sound>();

        public class Sound
        {
            public string name { get; set; }
            public float beat { get; set; }

            public Sound(string name, float beat)
            {
                this.name = name;
                this.beat = beat;
            }
        }


        public static void Play(Sound[] snds)
        {
            List<Sound> sounds = snds.ToList();
            GameObject gameObj = new GameObject();
            MultiSound ms = gameObj.AddComponent<MultiSound>();
            ms.sounds = sounds;
            ms.startBeat = sounds[0].beat;
            gameObj.name = "MultiSound";

            GameManager.instance.SoundObjects.Add(gameObj);
        }

        private void Update()
        {
            float songPositionInBeats = Conductor.instance.songPositionInBeats;

            for (int i = 0; i < sounds.Count; i++)
            {
                if (songPositionInBeats >= sounds[i].beat && index == i)
                {
                    Jukebox.PlayOneShotGame(sounds[i].name);
                    index++;
                }
            }

            if (songPositionInBeats >= (sounds[sounds.Count - 1].beat))
            {
                Destroy(this.gameObject);
            }
        }
    }
}