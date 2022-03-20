using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeavenStudio.Util
{
    public class MultiSound : MonoBehaviour
    {
        private float startBeat;
        private int index;
        private bool game;
        private bool forcePlay;
        public List<Sound> sounds = new List<Sound>();

        public class Sound
        {
            public string name { get; set; }
            public float beat { get; set; }
            public float pitch { get; set; }
            public float volume { get; set; }
            public bool looping { get; set; }
            public float offset { get; set; }

            public Sound(string name, float beat, float pitch = 1f, float volume = 1f, bool looping = false, float offset = 0f)
            {
                this.name = name;
                this.beat = beat;
                this.pitch = pitch;
                this.volume = volume;
                this.looping = looping;
                this.offset = offset;
            }
        }


        public static MultiSound Play(Sound[] snds, bool game = true, bool forcePlay = false)
        {
            List<Sound> sounds = snds.ToList();
            GameObject gameObj = new GameObject();
            MultiSound ms = gameObj.AddComponent<MultiSound>();
            ms.sounds = sounds;
            ms.startBeat = sounds[0].beat;
            ms.game = game;
            ms.forcePlay = forcePlay;
            gameObj.name = "MultiSound";

            GameManager.instance.SoundObjects.Add(gameObj);
            return ms;
        }

        private void Update()
        {
            float songPositionInBeats = Conductor.instance.songPositionInBeats;

            for (int i = 0; i < sounds.Count; i++)
            {
                if (songPositionInBeats >= sounds[i].beat - Conductor.instance.GetRestFromRealTime(sounds[i].offset) && index == i)
                {
                    if (game)
                        Jukebox.PlayOneShotGame(sounds[i].name, sounds[i].beat - Conductor.instance.GetRestFromRealTime(sounds[i].offset), sounds[i].pitch, sounds[i].volume, sounds[i].looping, forcePlay);
                    else
                        Jukebox.PlayOneShot(sounds[i].name, sounds[i].beat - Conductor.instance.GetRestFromRealTime(sounds[i].offset), sounds[i].pitch, sounds[i].volume, sounds[i].looping);

                    index++;
                }
            }

            if (songPositionInBeats >= (sounds[sounds.Count - 1].beat - Conductor.instance.GetRestFromRealTime(sounds[sounds.Count - 1].offset)))
            {
                Delete();
            }
        }

        public void Delete()
        {
            GameManager.instance.SoundObjects.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}