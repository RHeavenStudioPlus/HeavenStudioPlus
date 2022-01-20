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

        public bool relyOnBeat = true;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.pitch = pitch;
            audioSource.PlayScheduled(Time.time);

            startTime = Conductor.instance.songPosition;

            if (!relyOnBeat)
            {
                StartCoroutine(NotRelyOnBeatSound());
            }
        }

        private void Update()
        {
            if (relyOnBeat)
            {
                if (Conductor.instance.isPaused && !Conductor.instance.isPlaying && pauseTimes == 0)
                {
                    audioSource.Pause();
                    pauseTimes = 1;
                }
                else if (Conductor.instance.isPlaying && !Conductor.instance.isPaused && pauseTimes == 1)
                {
                    audioSource.Play();
                    pauseTimes = 0;
                }

                else if (!Conductor.instance.isPlaying && !Conductor.instance.isPaused)
                {
                    Destroy(this.gameObject);
                }
                if (Conductor.instance.songPosition > startTime + clip.length)
                {
                    Destroy(this.gameObject);
                }

                if (Conductor.instance.songPosition < startTime)
                {
                    Destroy(this.gameObject);
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
