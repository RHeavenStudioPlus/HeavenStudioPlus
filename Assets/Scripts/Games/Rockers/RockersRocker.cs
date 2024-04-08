using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Util;
using System;

namespace HeavenStudio.Games.Scripts_Rockers
{
    public class RockersRocker : MonoBehaviour
    {
        private Sound[] stringSounds = new Sound[6];
        private Sound chordSound;
        private Animator anim;
        public int[] lastPitches = new int[6];
        public int lastBendPitch;

        [SerializeField] private GameObject strumEffect;

        [SerializeField] private bool JJ;

        [NonSerialized] public bool muted;
        private bool strumming;
        private bool bending;
        [NonSerialized] public bool together;

        [SerializeField] List<Sprite> bluSprites = new List<Sprite>();
        [SerializeField] List<Sprite> yelSprites = new List<Sprite>();
        [SerializeField] List<Sprite> normalSprites = new List<Sprite>();

        [SerializeField] List<SpriteRenderer> lightningLefts = new List<SpriteRenderer>();
        [SerializeField] List<SpriteRenderer> lightningRights = new List<SpriteRenderer>();

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            StopSounds();
        }

        private void StopSounds()
        {
            foreach (var sound in stringSounds)
            {
                if (sound != null)
                {
                    sound.KillLoop();
                }
            }
            
            if (chordSound != null)
            {
                chordSound.KillLoop();
                chordSound = null;
            }
                
            stringSounds = new Sound[6];
        }

        public void PrepareTogether(bool forceMute = false)
        {
            together = true;
            if ((PlayerInput.GetIsAction(Rockers.InputAction_BasicPressing) && !JJ) || forceMute)
            {
                DoScaledAnimationAsync("ComeOnPrepare", 0.5f);
                if (forceMute) Mute(true, true);
            }
            else
            {
                DoScaledAnimationAsync("ComeOnPrepareNoMute", 0.5f);
                if (strumming) strumEffect.GetComponent<Animator>().Play("StrumRight", 0, 0);
            }
        }

        public void Miss()
        {
            if (strumming) return;
            DoScaledAnimationAsync(together ? "Miss" : "MissComeOn", 0.5f);
        }

        public void ReturnBack()
        {
            together = false;
            if (JJ)
            {
                muted = false;
                DoScaledAnimationAsync("Return", 0.5f);
            }
            else
            {
                if (strumming) strumEffect.GetComponent<Animator>().Play("StrumIdle", 0, 0);
                if (PlayerInput.GetIsAction(Rockers.InputAction_BasicPressing) || (GameManager.instance.autoplay && muted))
                {
                    DoScaledAnimationAsync("Crouch", 0.5f);
                }
                else
                {
                    muted = false;
                    DoScaledAnimationAsync("Return", 0.5f);
                }
            }
        }

        private bool lastGleeClub = false;
        private NoteSample lastSample;
        private int lastSampleTones;

        public void StrumStringsLast(bool disableStrumEffect = false, bool jump = false, bool barely = false)
        {
            StrumStrings(lastGleeClub, lastPitches, lastSample, lastSampleTones, disableStrumEffect, jump, barely);
        }

        public void StrumStrings(bool gleeClub, int[] pitches, NoteSample sample, int sampleTones, bool disableStrumEffect = false, bool jump = false, bool barely = false)
        {
            if (strumming) return;
            lastGleeClub = gleeClub;
            lastSample = sample;
            lastSampleTones = sampleTones;
            muted = false;
            strumming = true;
            StopSounds();
            if (sample.sample == null)
            {
                lastPitches = pitches;
                for (int i = 0; i < pitches.Length; i++)
                {
                    if (pitches[i] == -1) continue;
                    float pitch = SoundByte.GetPitchFromSemiTones(pitches[i], true);
                    float volume = GetVolumeBasedOnAmountOfStrings(pitches.Length);
                    string soundName = "rockers/strings/" + (gleeClub ? "gleeClub/gleeClub" : "normal/normal" + (i + 1));
                    // Debug.Log("Pitch: " + pitch + " Volume: " + volume + " Name: " + soundName);
                    stringSounds[i] = SoundByte.PlayOneShotGame(soundName, -1, pitch, volume, true);
                }
            }
            else
            {
                float pitch = SoundByte.GetPitchFromSemiTones(sampleTones, true);
                chordSound = SoundByte.PlayOneShotGame(sample.sample, -1, pitch, 1, true);
            }

            if (together)
            {
                DoScaledAnimationAsync(jump ? "Jump" : "ComeOnStrum", 0.5f);
                if (disableStrumEffect) return;
                strumEffect.SetActive(true);
                bool strumLeft = JJ && jump;
                strumEffect.GetComponent<Animator>().Play(strumLeft ? "StrumStartLeft" : "StrumStartRIght", 0, 0);
            }
            else
            {
                DoScaledAnimationAsync("Strum", 0.5f);
                if (disableStrumEffect) return;
                strumEffect.SetActive(true);
                strumEffect.GetComponent<Animator>().Play("StrumStart", 0, 0);
            }

            if (!JJ)
            {
                if (barely)
                {
                    bool useYel = UnityEngine.Random.Range(1, 3) == 1;
                    for (int i = 0; i < 3; i++)
                    {
                        if (lightningRights[i].gameObject.activeSelf) lightningRights[i].sprite = useYel ? yelSprites[i] : bluSprites[i];
                        if (lightningLefts[i].gameObject.activeSelf) lightningLefts[i].sprite = useYel ? yelSprites[i] : bluSprites[i];
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (lightningRights[i].gameObject.activeSelf) lightningRights[i].sprite = normalSprites[i];
                        if (lightningLefts[i].gameObject.activeSelf) lightningLefts[i].sprite = normalSprites[i];
                    }
                }
            }
        }

        public void BendUp(int pitch)
        {
            if (bending || !strumming) return;
            bending = true;
            lastBendPitch = pitch;
            if (chordSound != null) {
                chordSound.BendUp(0.05f, GetBentPitch(chordSound.pitch, pitch));
            }
            else
            {
                for (int i = 0; i < stringSounds.Length; i++)
                {
                    if (stringSounds[i] != null)
                    {
                        stringSounds[i].BendUp(0.05f, GetBentPitch(stringSounds[i].pitch, pitch));
                    }
                }
            }

            SoundByte.PlayOneShotGame("rockers/bendUp");
            DoScaledAnimationAsync("Bend", 0.5f);
        }


        public void BendDown()
        {
            if (!bending) return;
            bending = false;
            foreach (var sound in stringSounds)
            {
                if (sound != null)
                {
                    sound.BendDown(0.05f);
                }
            }
            if (chordSound != null)
            {
                chordSound.BendDown(0.05f);
            }
            SoundByte.PlayOneShotGame("rockers/bendDown");
            DoScaledAnimationAsync("Unbend", 0.5f);
        }

        private float GetVolumeBasedOnAmountOfStrings(int stringAmount)
        {

            switch (stringAmount)
            {
                default:
                    return 1;
                case 3:
                    return 0.893f;
                case 4:
                    return 0.75f;
                case 5:
                    return 0.686f;
                case 6:
                    return 0.62f;
            }
        }

        public void Mute(bool soundExists = true, bool noAnim = false)
        {
            strumming = false;
            strumEffect.SetActive(false);
            bending = false;
            StopSounds();
            if (soundExists) SoundByte.PlayOneShotGame("rockers/mute");
            if (!noAnim) DoScaledAnimationAsync(together ? "ComeOnMute" : "Crouch", 0.5f);
            muted = true;
        }

        public void UnHold(bool overrideMute = false)
        {
            if (!muted && !overrideMute) return;
            muted = false;
            if (!together) DoScaledAnimationAsync("UnCrouch", 0.5f);
        }

        private void DoScaledAnimationAsync(string name, float time)
        {
            anim.DoScaledAnimationAsync((JJ ? "JJ" : "") + name, time);
        }
        
        private float GetBentPitch(float pitch, int bend)
        {
            float unscaledPitch = pitch / Conductor.instance.musicSource.pitch;
            float bendPitch = SoundByte.GetPitchFromSemiTones(bend, false);
            
            return (unscaledPitch * bendPitch) * Conductor.instance.musicSource.pitch;
        }
    }
}

