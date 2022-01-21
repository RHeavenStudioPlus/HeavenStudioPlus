using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Util
{
    public class Sound : MonoBehaviour
    {
        public AudioClip clip;
        public float pitch = 1;

        private AudioSource audioSource;

        private int pauseTimes = 0;

        private float startTime;

        public float beat;

        bool playInstant = false;
        int playIndex = 0;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.pitch = pitch;

            if (beat == -1)
            {
                audioSource.PlayScheduled(Time.time);
                playInstant = true;
                playIndex++;
            }
            else
            {
                playInstant = false;
            }

            startTime = Conductor.instance.songPosition;

            StartCoroutine(NotRelyOnBeatSound());
        }

        private void Update()
        {
            if (!playInstant)
            {
                if (Conductor.instance.songPositionInBeats > beat && playIndex < 1)
                {
                    audioSource.PlayScheduled(Time.time);
                    playIndex++;
                }
            }
        }

        IEnumerator NotRelyOnBeatSound()
        {
            yield return new WaitForSeconds(clip.length);
            Destroy(this.gameObject);
        }
    }
}
