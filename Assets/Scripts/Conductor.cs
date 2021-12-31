using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Starpelly;

namespace RhythmHeavenMania
{
    [RequireComponent(typeof(AudioSource))]
    public class Conductor : MonoBehaviour
    {
        //Song beats per minute
        //This is determined by the song you're trying to sync up to
        public float songBpm;

        //The number of seconds for each song beat
        public float secPerBeat;

        //Current song position, in seconds
        public float songPosition;

        //Current song position, in beats
        public float songPositionInBeats;

        //How many seconds have passed since the song started
        public float dspSongTime;

        //an AudioSource attached to this GameObject that will play the music.
        public AudioSource musicSource;

        //The offset to the first beat of the song in seconds
        public float firstBeatOffset;

        //the number of beats in each loop
        public float beatsPerLoop;

        //the total number of loops completed since the looping clip first started
        public int completedLoops = 0;

        //The current position of the song within the loop in beats.
        public float loopPositionInBeats;

        //The current relative position of the song within the loop measured between 0 and 1.
        public float loopPositionInAnalog;

        //Conductor instance
        public static Conductor instance;

        private AudioDspTimeKeeper timeKeeper;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //Load the AudioSource attached to the Conductor GameObject
            musicSource = GetComponent<AudioSource>();

            timeKeeper = GetComponent<AudioDspTimeKeeper>();

            //Calculate the number of seconds in each beat
            secPerBeat = 60f / songBpm;

            //Record the time when the music starts
            // dspSongTime = (float)musicSource.time;

            //Start the music
            // musicSource.Play();
        }

        public void Play(float startBeat)
        {
            timeKeeper.Play();
            SetTime(startBeat);
        }

        public void SetTime(float startBeat)
        {
            musicSource.time = GetSongPosFromBeat(startBeat);
            songPositionInBeats = musicSource.time / secPerBeat;
            GameManager.instance.SetCurrentEventToClosest(songPositionInBeats);
        }

        public void Update()
        {
            if (!musicSource.isPlaying) return;

            //determine how many seconds since the song started
            // songPosition = (float)(timeKeeper.dspTime - dspSongTime - firstBeatOffset);
            songPosition = (float)timeKeeper.GetCurrentTimeInSong();

            //determine how many beats since the song started
            songPositionInBeats = songPosition / secPerBeat;


            //calculate the loop position
            if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
                completedLoops++;
            loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;

            loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;
        }

        public float GetLoopPositionFromBeat(float startBeat, float length)
        {
            float a = Mathp.Normalize(songPositionInBeats, startBeat, startBeat + length);
            return a;
        }

        private float GetSongPosFromBeat(float beat)
        {
            return secPerBeat * beat;
        }

        public void SetBpm(float bpm)
        {
            this.songBpm = bpm;
            secPerBeat = 60f / songBpm;
        }
    }
}