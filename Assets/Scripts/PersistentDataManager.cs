using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Newtonsoft.Json;

namespace HeavenStudio.Common
{
    public static class PersistentDataManager
    {
        public enum PerfectChallengeType
        {
            Off,        // no perfect challenge
            Arcade,     // "arcade rule"
            Legacy,     // "legacy rule"
            On          // "megamix rule"
        }

        [NonSerialized] public static GameSettings gameSettings;

        public static void CreateDefaultSettings()
        {
            gameSettings = new GameSettings(
                true,
                false,
                1,
                GlobalGameManager.DEFAULT_SCREEN_SIZES[1].width,
                GlobalGameManager.DEFAULT_SCREEN_SIZES[1].height,
                0.8f,
                340,
                48000,
                true,
                true,
                PerfectChallengeType.Off,
                true,
                false,
                true,
                true,
                true
            );

            // disable if platform is mac
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                gameSettings.discordRPCEnable = false;
            else
                gameSettings.discordRPCEnable = true;
            
            gameSettings.timingDisplayComponents = new List<OverlaysManager.TimingDisplayComponent>()
            {
                OverlaysManager.TimingDisplayComponent.CreateDefaultSingle()
            };
            gameSettings.skillStarComponents = new List<OverlaysManager.SkillStarComponent>()
            {
                OverlaysManager.SkillStarComponent.CreateDefault()
            };
            gameSettings.sectionComponents = new List<OverlaysManager.SectionComponent>()
            {
                OverlaysManager.SectionComponent.CreateDefault()
            };
            
            SaveSettings();
        }

        public static void LoadSettings()
        {
            if (File.Exists(Application.persistentDataPath + "/settings.json"))
            {
                string json = File.ReadAllText(Application.persistentDataPath + "/settings.json");
                if (json == "")
                {
                    GlobalGameManager.IsFirstBoot = true;
                    CreateDefaultSettings();
                    return;
                }
                try
                {
                    gameSettings = JsonConvert.DeserializeObject<GameSettings>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while loading settings, creating default settings;\n{e.Message}");
                    GlobalGameManager.IsFirstBoot = true;
                    CreateDefaultSettings();
                }
            }
            else
            {
                GlobalGameManager.IsFirstBoot = true;
                CreateDefaultSettings();
            }
        }

        public static void SaveSettings()
        {
            string json = JsonUtility.ToJson(gameSettings);
            // save json to persistentDataPath
            FileStream file = File.Create(Application.persistentDataPath + "/settings.json");
            file.Write(System.Text.Encoding.ASCII.GetBytes(json), 0, json.Length);
            file.Close();
        }

        public static void SaveTheme(string json)
        {
            // save json to persistentDataPath
            FileStream file = File.Create(Application.persistentDataPath + "/editorTheme.json");
            file.Write(System.Text.Encoding.ASCII.GetBytes(json), 0, json.Length);
            file.Close();
        }

        [Serializable]
        public struct GameSettings
        {
            // default settings constructor
            public GameSettings(
                bool showSplash = false,
                bool isFullscreen = false,
                int resolutionIndex = 0,
                int resolutionWidth = 1280,
                int resolutionHeight = 720,
                float masterVolume = 0.8f,
                int dspSize = 340,
                int sampleRate = 44100,
                bool editorCursorEnable = true,
                bool discordRPCEnable = true,
                PerfectChallengeType perfectChallengeType = PerfectChallengeType.On,
                bool isMedalOn = true,
                bool timingDisplayMinMode = false,
                bool overlaysInEditor = true,
                bool letterboxBgEnable = true,
                bool letterboxFxEnable = true,
                int editorScale = 0,
                bool scaleWScreenSize = false,
                int showParamTooltips = 1,
                bool previewNoteSounds = true
                )
            {
                this.showSplash = showSplash;
                this.isFullscreen = isFullscreen;

                this.resolutionIndex = resolutionIndex;
                this.resolutionWidth = resolutionWidth;
                this.resolutionHeight = resolutionHeight;

                this.masterVolume = masterVolume;
                this.dspSize = dspSize;
                this.sampleRate = sampleRate;

                this.editorCursorEnable = editorCursorEnable;
                if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
                    this.discordRPCEnable = false;
                else
                    this.discordRPCEnable = true;
                this.editorScale = editorScale;
                this.scaleWScreenSize = scaleWScreenSize;
                this.showParamTooltips = showParamTooltips;
                this.previewNoteSounds = previewNoteSounds;

                this.perfectChallengeType = perfectChallengeType;
                this.isMedalOn = isMedalOn;
                this.timingDisplayMinMode = timingDisplayMinMode;
                this.overlaysInEditor = overlaysInEditor;
                this.letterboxBgEnable = true;
                this.letterboxFxEnable = true;

                this.timingDisplayComponents = new List<OverlaysManager.TimingDisplayComponent>()
                {
                    OverlaysManager.TimingDisplayComponent.CreateDefaultDual()
                };
                this.skillStarComponents = new List<OverlaysManager.SkillStarComponent>()
                {
                    OverlaysManager.SkillStarComponent.CreateDefault()
                };
                this.sectionComponents = new List<OverlaysManager.SectionComponent>()
                {
                    OverlaysManager.SectionComponent.CreateDefault()
                };
            }

            // Display / Audio Settings
            public bool showSplash;
            public bool isFullscreen;

            public int resolutionIndex;
            public int resolutionWidth;
            public int resolutionHeight;

            public float masterVolume;
            public int dspSize;
            public int sampleRate;

            // Editor Settings
            public bool editorCursorEnable;
            public bool discordRPCEnable;
            public int editorScale;
            public bool scaleWScreenSize;
            public int showParamTooltips;
            public bool previewNoteSounds;
            // public bool showCornerTooltips;

            // Gameplay Settings
            public PerfectChallengeType perfectChallengeType;
            public bool isMedalOn;
            public bool timingDisplayMinMode;
            public bool overlaysInEditor;
            public bool letterboxBgEnable;
            public bool letterboxFxEnable;
            public List<OverlaysManager.TimingDisplayComponent> timingDisplayComponents;
            public List<OverlaysManager.SkillStarComponent> skillStarComponents;
            public List<OverlaysManager.SectionComponent> sectionComponents;
            // public List<OverlaysManager.DurationComponent> durationComponents;
            // public List<OverlaysManager.WordJudgementComponent> wordJudgementComponents;
        }
    }
}