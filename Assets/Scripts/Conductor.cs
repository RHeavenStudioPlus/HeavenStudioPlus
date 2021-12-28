using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Starpelly;

// I CANNOT STRESS THIS ENOUGH, SET "Project Settings/Audio/DSP Buffer Size" to "Best latency" or else AudioSource.time WILL NOT update every frame.

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

        //Pause times
        // private int pauseTime = 0;

        public float beatThreshold;

        private float lastTime;
        private float lastMst_F;
        private int framesSinceLastSame;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //Load the AudioSource attached to the Conductor GameObject
            musicSource = GetComponent<AudioSource>();

            //Calculate the number of seconds in each beat
            secPerBeat = 60f / songBpm;

            //Record the time when the music starts
            dspSongTime = (float)musicSource.time;

            //Start the music
            // musicSource.Play();
        }

        public void Play(float startBeat)
        {
            musicSource.Play();
            
        }

        public void Update()
        {
            // Conductor.instance.musicSource.pitch = Time.timeScale;

            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                pauseTime++;
                if (pauseTime == 1)
                    musicSource.Pause();
                else if (pauseTime > 1) { musicSource.UnPause(); pauseTime = 0; }
            }*/

            float mst = musicSource.timeSamples / (float)musicSource.clip.frequency;
            float mst_f = mst + 0;

            if (mst == lastTime && musicSource.isPlaying)
            {
                framesSinceLastSame++;

                mst_f = mst_f + (Time.deltaTime * framesSinceLastSame) * musicSource.pitch;

                if (mst_f <= lastMst_F)
                {
                    // mst_f = lastMst_F;
                    float b = lastMst_F + (Time.deltaTime) * musicSource.pitch;

                    mst_f = b;
                    // print(b);
                    // print(mst_f + "  " + b + "  " + lastMst_F);
                }
                else if (mst_f < lastTime)
                {
                    Debug.LogError("What the fuck.");
                }

                // print($"{lastMst_F}, {mst_f}");
            }
            else
            {
                framesSinceLastSame = 0;
            }

            lastTime = mst;
            lastMst_F = mst_f;

            //determine how many seconds since the song started
            songPosition = (float)(mst_f - dspSongTime - firstBeatOffset);

            //determine how many beats since the song started
            songPositionInBeats = songPosition / secPerBeat;
            // print($"{mst_f}(AudioSource.time), {Time.frameCount}(Time.fasrameCount)");
            // print($"{musicSource.time}(0), {mst_f}");


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

        public void SetBpm(float bpm)
        {
            this.songBpm = bpm;
            secPerBeat = 60f / songBpm;
        }

        public bool InThreshold(float beat)
        {
            //Check if the beat sent falls within beatThreshold
            //Written to handle the looping
            if (beat <= beatThreshold + 1)
            {
                //Debug.Log("Case 1, beat is close to 1");
                if (loopPositionInBeats > beat + beatThreshold)
                {
                    if (loopPositionInBeats >= (beat + songPositionInBeats - 1) + beatThreshold)
                    {
                        //Debug.Log("LoopPos just below loop point");
                        return true;
                    }
                    else
                    {
                        //Debug.Log("LoopPos not within beat threshold");
                    }
                }
                else
                {
                    //Debug.Log("Case 1, loopPos between loop point and beat threshold");
                }
            }
            else if (beat < (songPositionInBeats + 1 - beatThreshold))
            {
                //Debug.Log("Case 2, beat is far from loop point.");
                if (loopPositionInBeats >= beat - beatThreshold && loopPositionInBeats <= beat + beatThreshold)
                {
                    //Debug.Log("LoopPos within threshold");
                    return true;
                }
            }
            else if (beat >= (songPositionInBeats + 1 - beatThreshold))
            {
                //Debug.Log("Case 3, beat is close to loop point");
                if (loopPositionInBeats < beat)
                {
                    if (loopPositionInBeats >= beat - beatThreshold)
                    {
                        //Debug.Log("LoopPos just below beat");
                        return true;
                    }
                    else if (loopPositionInBeats < (beat - songPositionInBeats + 1) - beatThreshold)
                    {
                        //Debug.Log("LoopPos just above loop point");
                        return true;
                    }
                }
                else
                {
                    //Debug.Log("LoopPos just above beat");
                    return true;
                }

            }
            else
            {
                Debug.LogError("Strange Case. Where is this beat? This should never happen");
            }
            return false;
        }
    }
}