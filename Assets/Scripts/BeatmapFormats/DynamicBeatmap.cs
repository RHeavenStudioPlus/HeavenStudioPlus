using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HeavenStudio.Util;

namespace HeavenStudio
{
    [Serializable]
    public class DynamicBeatmap
    {
        public static int CurrentRiqVersion = 0;
        public float bpm;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(100)] public int musicVolume; // In percent (1-100)
        
        public Dictionary<string, object> properties = 
            new Dictionary<string, object>() {
                // software version (MajorMinorPatch, revision)
                {"productversion", 000},
                {"productsubversion", 0},
                //file format version
                {"riqversion", CurrentRiqVersion},

                // general chart info
                {"remixtitle", "New Remix"},    // chart name
                {"remixauthor", "Your Name"},   // charter's name
                {"remixlevel", 1},              // chart difficulty
                {"remixtempo", 120f},           // avg. chart tempo
                {"remixtags", ""},              // chart tags
                {"icontype", 0},                // chart icon (presets, custom - future)
                {"iconurl", ""},                // custom icon location (future)

                // chart song info
                {"idolgenre", "Song Genre"},    // song genre
                {"idolsong", "Song Name"},      // song name
                {"idolcredit", "Artist"},       // song artist

                // chart prologue
                {"prologuetype", 0},            // prologue card animation (future)
                {"prologuecaption", "Remix"},   // prologue card sub-title (future)

                // chart results screen messages
                {"resultcaption", "Rhythm League Notes"},                       // result screen header
                {"resultcommon_hi", "Good rhythm."},                            // generic "Superb" message (one-liner, or second line for single-type)
                {"resultcommon_ok", "Eh. Passable."},                           // generic "OK" message (one-liner, or second line for single-type)
                {"resultcommon_ng", "Try harder next time."},                   // generic "Try Again" message (one-liner, or second line for single-type)

                    // the following are shown / hidden in-editor depending on the tags of the games used
                {"resultnormal_hi", "You show strong fundamentals."},           // "Superb" message for normal games (two-liner)
                {"resultnormal_ng", "Work on your fundamentals."},              // "Try Again" message for normal games (two-liner)

                {"resultkeep_hi", "You kept the beat well."},                   // "Superb" message for keep-the-beat games (two-liner)
                {"resultkeep_ng", "You had trouble keeping the beat."},         // "Try Again" message for keep-the-beat games (two-liner)

                {"resultaim_hi", "You had great aim."},                         // "Superb" message for aim games (two-liner)
                {"resultaim_ng", "Your aim was a little shaky."},               // "Try Again" message for aim games (two-liner)

                {"resultrepeat_hi", "You followed the example well."},          // "Superb" message for call-and-response games (two-liner)
                {"resultrepeat_ng", "Next time, follow the example better."},   // "Try Again" message for call-and-response games (two-liner)
        };

        public List<DynamicEntity> entities = new List<DynamicEntity>();
        public List<TempoChange> tempoChanges = new List<TempoChange>();
        public List<VolumeChange> volumeChanges = new List<VolumeChange>();
        public float firstBeatOffset;

        [Serializable]
        public class DynamicEntity : ICloneable
        {
            public float beat;
            public int track;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float length;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float swing;
            public Dictionary<string, dynamic> DynamicData = new Dictionary<string, dynamic>();

            public string datamodel;
            [JsonIgnore] public Editor.Track.TimelineEventObj eventObj;

            public object Clone()
            {
                return this.MemberwiseClone();
            }

            public DynamicEntity DeepCopy()
            {
                //lol the AI generated this
                return JsonConvert.DeserializeObject<DynamicEntity>(JsonConvert.SerializeObject(this));
            }

            public dynamic this[string propertyName]
            {
                get
                {
                    switch (propertyName)
                    {
                        case "beat":
                            return beat;
                        case "track":
                            return track;
                        case "length":
                            return length;
                        case "swing":
                            return swing;
                        case "datamodel":
                            return datamodel;
                        default:
                            //TODO: do this checking and conversion on load instead of at runtime
                            Minigames.Minigame game = EventCaller.instance.GetMinigame(datamodel.Split(0));
                            Minigames.GameAction action = EventCaller.instance.GetGameAction(game, datamodel.Split(1));
                            Minigames.Param param = EventCaller.instance.GetGameParam(game, datamodel.Split(1), propertyName);
                            var type = param.parameter.GetType();
                            if (DynamicData.ContainsKey(propertyName))
                            {
                                var pType = DynamicData[propertyName].GetType();
                                if (pType == type)
                                {
                                    return DynamicData[propertyName];
                                }
                                else
                                {
                                    if (type == typeof(EntityTypes.Integer))
                                        return (int) DynamicData[propertyName];
                                    else if (type == typeof(EntityTypes.Float))
                                        return (float) DynamicData[propertyName];
                                    else if (type.IsEnum)
                                        return (int) DynamicData[propertyName];
                                    else if (pType == typeof(Newtonsoft.Json.Linq.JObject))
                                    {
                                        DynamicData[propertyName] = DynamicData[propertyName].ToObject(type);
                                        return DynamicData[propertyName];
                                    }
                                    else
                                        return Convert.ChangeType(DynamicData[propertyName], type);
                                }
                            }
                            else
                            {
                                return param.parameter;
                            }
                    }
                }
                set
                {
                    if (DynamicData.ContainsKey(propertyName))
                    {
                        DynamicData[propertyName] = value;
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"This entity does not have a property named {propertyName}! Attempted to insert value of type {value.GetType()}");
                    }
                }
            }

            public void CreateProperty(string name, dynamic defaultValue)
            {
                if (!DynamicData.ContainsKey(name))
                    DynamicData.Add(name, defaultValue);
            }
        }

        [Serializable]
        public class TempoChange : ICloneable
        {
            public float beat;
            public float length;
            public float tempo;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        [Serializable]
        public class VolumeChange : ICloneable
        {
            public float beat;
            public float length;
            public float volume;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        public object this[string propertyName]
        {
            get
            {
                return properties[propertyName];
            }
            set
            {
                if (properties.ContainsKey(propertyName))
                {
                    properties[propertyName] = value;
                }
                else
                {
                    UnityEngine.Debug.LogError($"This beatmap does not have a property named {propertyName}! Attempted to insert value of type {value.GetType()}");
                }
            }
        }

        /// <summary>
        /// converts from the old "rhmania" / "tengoku" format to the new "riq" format
        /// </summary>
        /// <param name="beatmap">a deserialized .rhmania or .tengoku beatmap</param>
        /// <returns>a .riq beatmap</returns>
        public static DynamicBeatmap BeatmapConverter(Beatmap beatmap)
        {
            DynamicBeatmap dynamicBeatmap = new DynamicBeatmap();
            dynamicBeatmap.bpm = beatmap.bpm;
            dynamicBeatmap.musicVolume = beatmap.musicVolume;
            dynamicBeatmap.firstBeatOffset = beatmap.firstBeatOffset;

            foreach (var entity in beatmap.entities)
            {
                dynamicBeatmap.entities.Add(new DynamicEntity()
                {
                    beat = entity.beat,
                    track = entity.track,
                    length = entity.length,
                    swing = entity.swing,
                    datamodel = entity.datamodel,
                    //TODO: only convert properties that actually exist in each GameAction
                    DynamicData = new Dictionary<string, dynamic>()
                    {
                        { "valA", entity.valA },
                        { "valB", entity.valB },
                        { "valC", entity.valC },

                        { "toggle", entity.toggle },

                        { "type", entity.type },
                        { "type2", entity.type2 },
                        { "type3", entity.type3 },
                        { "type4", entity.type4 },
                        { "type5", entity.type5 },
                        { "type6", entity.type6 },

                        { "ease", (int) entity.ease },

                        { "colorA", (Color) entity.colorA },
                        { "colorB", (Color) entity.colorB },
                        { "colorC", (Color) entity.colorC },
                        { "colorD", (Color) entity.colorD },
                        { "colorE", (Color) entity.colorE },
                        { "colorF", (Color) entity.colorF },

                        { "text1", entity.text1 },
                        { "text2", entity.text2 },
                        { "text3", entity.text3 },
                    }
                });
            }
            foreach (var tempoChange in beatmap.tempoChanges)
            {
                dynamicBeatmap.tempoChanges.Add(new TempoChange()
                {
                    beat = tempoChange.beat,
                    length = tempoChange.length,
                    tempo = tempoChange.tempo
                });
            }
            foreach (var volumeChange in beatmap.volumeChanges)
            {
                dynamicBeatmap.volumeChanges.Add(new VolumeChange()
                {
                    beat = volumeChange.beat,
                    length = volumeChange.length,
                    volume = volumeChange.volume
                });
            }
            return dynamicBeatmap;
        }

        /// <summary>
        /// FUTURE: converts from a karateka mania chart ("bor") to the "riq" format
        /// </summary>
        /// <param name="bor">a rawtext .bor chart</param>
        /// <returns>a .riq beatmap</returns>
        /// <remarks>not implemented yet</remarks>
        public static DynamicBeatmap KManiaBorConverter(String bor)
        {
            return null;
        }

        /// <summary>
        /// updates an "riq" beatmap
        /// </summary>
        /// <param name="beatmap">old beatmap</param>
        /// <param name="version">version of old beatmap</param>
        /// <returns>updated beatmap</returns>
        /// <remarks>not implemented yet</remarks>
        public static DynamicBeatmap BeatmapUpdater(DynamicBeatmap beatmap, int version)
        {
            return beatmap;
        }

    }
}