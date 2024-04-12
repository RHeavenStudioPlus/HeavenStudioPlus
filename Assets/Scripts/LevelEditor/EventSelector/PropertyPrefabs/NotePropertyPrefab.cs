using System;
using System.Collections;
using System.Collections.Generic;
using HeavenStudio;
using HeavenStudio.Common;
using HeavenStudio.Editor;
using HeavenStudio.Util;
using TMPro;
using UnityEngine;

public class NotePropertyPrefab : NumberPropertyPrefab
{
    public TMP_Text noteLabel, flatLabel;

    private Sound previewAudioSource;
    private int offsetFromC;
    public EntityTypes.Note note;

    public override void SetProperties(string propertyName, object type, string caption)
    {
        base.SetProperties(propertyName, type, caption);

        SetNote((EntityTypes.Note)type);

        slider.onValueChanged.AddListener(
            _ =>
            {
                int trueSemitones = (int)slider.value + offsetFromC;
                inputField.text = slider.value.ToString();
                parameterManager.entity[propertyName] = trueSemitones;
                if (slider.value != _defaultValue)
                {
                    this.caption.text = _captionText + "*";
                }
                else
                {
                    this.caption.text = _captionText;
                }

                UpdateNoteText(trueSemitones);
                
                PlayPreview(note, trueSemitones);
            }
        );

        inputField.onSelect.AddListener(
            _ =>
                Editor.instance.editingInputField = true
        );

        inputField.onEndEdit.AddListener(
            _ =>
            {
                slider.value = Convert.ToSingle(inputField.text);
                
                int trueSemitones = (int)slider.value + offsetFromC;
                
                parameterManager.entity[propertyName] = trueSemitones;
                Editor.instance.editingInputField = false;
                if (slider.value != _defaultValue)
                {
                    this.caption.text = _captionText + "*";
                }
                else
                {
                    this.caption.text = _captionText;
                }

                UpdateNoteText(trueSemitones);
                
                PlayPreview(note, trueSemitones);
            }
        );
    }

    public void SetNote(EntityTypes.Note note, bool playPreview = false)
    {
        this.note = note;

        slider.minValue = note.min;
        slider.maxValue = note.max;

        slider.wholeNumbers = true;

        offsetFromC = 0;
        if (note.offsetToC) {
            int sameC = 3 - note.sampleNote;
            int upperC = 15 - note.sampleNote;
            offsetFromC = Mathf.Abs(sameC) < Mathf.Abs(upperC) ? sameC : upperC;
        }

        int lastValue = (int)slider.value;
        slider.value = Convert.ToSingle(parameterManager.entity[propertyName]) - offsetFromC;
        _defaultValue = slider.value;
        
        inputField.text = slider.value.ToString();
        UpdateNoteText((int)slider.value + offsetFromC);
        
        if((int)slider.value == lastValue && playPreview)
            PlayPreview(note, (int)slider.value + offsetFromC);
    }
    
    private void UpdateNoteText(int semiTones)
    {
        GetNoteText(note, semiTones, out var sharp, out var flat);
        noteLabel.text = sharp;
        flatLabel.text = flat;
    }
    
    public void OnSelectSliderHandle()
    {
        PlayPreview(note, (int)slider.value + offsetFromC);
    }

    private void PlayPreview(EntityTypes.Note note, int currentSemitones)
    {
        if (note.sampleName == null || !PersistentDataManager.gameSettings.previewNoteSounds) return;

        if (previewAudioSource != null)
        {
            previewAudioSource.KillLoop();
            previewAudioSource = null;
        }
        
        float pitch = SoundByte.GetPitchFromSemiTones(currentSemitones, true);
        if(pitch == 1f) pitch = 1.0001f; // man writes worst workaround ever, banned from Heaven Studio source code
        previewAudioSource = SoundByte.PlayOneShotGame(note.sampleName, pitch: pitch, volume: 0.75f, forcePlay: true, ignoreConductorPause: true);
        previewAudioSource.KillLoop(.5f);
    }

    private static readonly string[] notes = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
    private static readonly string[] notesFlat = { "A", "Bb", "B", "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab" };

    private static string GetNoteText(EntityTypes.Note note, int currentSemitones, out string sharp, out string flat)   
    {
        int noteIndex = (note.sampleNote + currentSemitones) % 12;
        if (noteIndex < 0)
        {
            noteIndex += 12;
        }

        int octaveOffset = (note.sampleNote + currentSemitones) / 12;
        int octave = note.sampleOctave + octaveOffset;

        if ((note.sampleNote + currentSemitones) % 12 < 0)
        {
            octave--;
        }
        
        sharp = notes[noteIndex] + octave;
        flat = notesFlat[noteIndex] + octave;
        return sharp;
    }
}
