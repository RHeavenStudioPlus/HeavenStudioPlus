#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Linq;

[InitializeOnLoad]
public class SwitchShortcutsProfileOnPlay
{
    private const string PlayingProfileId = "Playing";
    private static string _activeProfileId;
    private static bool _switched;

    static SwitchShortcutsProfileOnPlay()
    {
        EditorApplication.playModeStateChanged += DetectPlayModeState;
    }

    private static void SetActiveProfile(string profileId)
    {
        Debug.Log($"Activating Shortcut profile \"{profileId}\"");
        ShortcutManager.instance.activeProfileId = profileId;
    }

    private static void DetectPlayModeState(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                OnEnteredPlayMode();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                OnExitingPlayMode();
                break;
        }
    }

    private static void OnExitingPlayMode()
    {
        if (!_switched)
            return;

        _switched = false;
        SetActiveProfile("Default");
    }

    private static void OnEnteredPlayMode()
    {
        _activeProfileId = ShortcutManager.instance.activeProfileId;
        if (_activeProfileId.Equals(PlayingProfileId))
            return; // Same as active

        var allProfiles = ShortcutManager.instance.GetAvailableProfileIds().ToList();

        if (!allProfiles.Contains(PlayingProfileId))
            return; // Couldn't find PlayingProfileId

        _switched = true;
        SetActiveProfile("Playing");
    }
}
#endif