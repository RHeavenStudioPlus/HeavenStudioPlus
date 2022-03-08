using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Util
{
    public class Jukebox
    {
        public enum AudioType
        {
            OGG,
            WAV
        }

        /// <summary>
        /// This is me just idiot-proofing.
        /// </summary>
        public static void BasicCheck()
        {
            if (FindJukebox() == null)
            {
                GameObject Jukebox = new GameObject("Jukebox");
                Jukebox.AddComponent<AudioSource>();
                Jukebox.tag = "Jukebox";
            }
        }

        public static GameObject FindJukebox()
        {
            if (GameObject.FindGameObjectWithTag("Jukebox") != null)
                return GameObject.FindGameObjectWithTag("Jukebox");
            else
                return null;
        }

        public static void SetVolume(float volume)
        {
            BasicCheck();
            FindJukebox().GetComponent<AudioSource>().volume = volume;
        }

        public static AudioSource PlayOneShot(string name, float beat = -1)
        {
            GameObject oneShot = new GameObject("oneShot");

            AudioSource audioSource = oneShot.AddComponent<AudioSource>();
            //audioSource.outputAudioMixerGroup = Settings.GetSFXMixer();
            audioSource.playOnAwake = false;

            Sound snd = oneShot.AddComponent<Sound>();
            AudioClip clip = Resources.Load<AudioClip>($"Sfx/{name}");
            snd.clip = clip;
            snd.beat = beat;
            // snd.pitch = (clip.length / Conductor.instance.secPerBeat);

            GameManager.instance.SoundObjects.Add(oneShot);

            return audioSource;
        }

        public static AudioSource PlayOneShotScheduled(string name, double targetTime)
        {
            GameObject oneShot = new GameObject("oneShotScheduled");

            var audioSource = oneShot.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            var snd = oneShot.AddComponent<Sound>();

            var clip = Resources.Load<AudioClip>($"Sfx/{name}");
            audioSource.clip = clip;
            snd.clip = clip;
            
            snd.scheduled = true;
            snd.scheduledTime = targetTime;
            audioSource.PlayScheduled(targetTime);
            
            GameManager.instance.SoundObjects.Add(oneShot);

            return audioSource;
        }

        public static AudioSource PlayOneShotGame(string name, float beat = -1, bool forcePlay = false)
        {
            if (GameManager.instance.currentGame == name.Split('/')[0] || forcePlay)
            {
                return PlayOneShot($"games/{name}", beat);
            }

            return null;
        }

        public static AudioSource PlayOneShotScheduledGame(string name, double targetTime, bool forcePlay = false)
        {
            if (GameManager.instance.currentGame == name.Split('/')[0] || forcePlay)
            {
                return PlayOneShotScheduled($"games/{name}", targetTime);
            }

            return null;
        }
    }

}