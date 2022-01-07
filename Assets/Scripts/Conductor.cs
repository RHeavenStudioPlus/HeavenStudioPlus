using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Starpelly;

namespace RhythmHeavenMania
{
    // [RequireComponent(typeof(AudioSource))]
    public class Conductor : MonoBehaviour
    {
        // Song beats per minute
        // This is determined by the song you're trying to sync up to
        public float songBpm;

        // The number of seconds for each song beat
        public float secPerBeat;

        // Current song position, in seconds
        public float songPosition;

        // Current song position, in beats
        public float songPositionInBeats;

        // Time that the song paused
        private float pauseTime;

        // Current time of the song
        private float time;

        // an AudioSource attached to this GameObject that will play the music.
        public AudioSource musicSource;

        // The offset to the first beat of the song in seconds
        public float firstBeatOffset;

        // Conductor instance
        public static Conductor instance;

        public bool isPlaying;
        public bool isPaused;

        public float currentTime;

        // private AudioDspTimeKeeper timeKeeper;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            musicSource = GetComponent<AudioSource>();

            secPerBeat = 60f / songBpm;
        }

        public void SetBeat(float t)
        {
            float secFromBeat = secPerBeat * t;

            currentTime = secFromBeat;
            pauseTime = 0;

            if (secFromBeat < musicSource.clip.length)
                musicSource.time = secFromBeat;

            GameManager.instance.SetCurrentEventToClosest(t);
        }

        public void Play()
        {
            time = currentTime + pauseTime;

            isPlaying = true;
            isPaused = false;

            musicSource.PlayScheduled(Time.time);
        }

        public void Pause()
        {
            pauseTime = time;
            currentTime = 0;

            isPlaying = false;
            isPaused = true;

            musicSource.Pause();
        }

        public void Stop()
        {
            time = 0;
            isPlaying = false;
            isPaused = false;

            GameManager.instance.SetCurrentEventToClosest(songPositionInBeats);

            musicSource.Stop();
        }

        /*public void SetTime(float startBeat)
        {
            musicSource.time = GetSongPosFromBeat(startBeat);
            songPositionInBeats = musicSource.time / secPerBeat;
            GameManager.instance.SetCurrentEventToClosest(songPositionInBeats);
        }*/

        public void Update()
        {
            if (isPlaying)
            {
                time += Time.deltaTime * musicSource.pitch;

                songPosition = time - firstBeatOffset;

                songPositionInBeats = songPosition / secPerBeat;
            }
        }

        public float GetLoopPositionFromBeat(float startBeat, float length)
        {
            float a = Mathp.Normalize(songPositionInBeats, startBeat, startBeat + length);
            return a;
        }

        public float GetSongPosFromBeat(float beat)
        {
            return secPerBeat * beat;
        }

        public void SetBpm(float bpm)
        {
            this.songBpm = bpm;
            secPerBeat = 60f / songBpm;
        }

        public float SongLengthInBeats()
        {
            if (!musicSource.clip) return 0;
            return musicSource.clip.length / secPerBeat;
        }
    }
}