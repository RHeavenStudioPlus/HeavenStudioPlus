using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

using HeavenStudio.Util;
using HeavenStudio.Editor.Track;
using HeavenStudio.Games;
using Jukebox;

using SatorImaging.UnitySourceGenerator;

using System;
using System.IO;
using System.Linq;
using BurstLinq;
using UnityEngine.Assertions.Must;
using Newtonsoft.Json.Linq;

namespace HeavenStudio
{
    [UnitySourceGenerator(typeof(MinigameLoaderGenerator), OverwriteIfFileExists = true)]
    public partial class Minigames
    {
        public enum RecommendedControlStyle
        {
            Any,
            Pad,
            Touch,
            Baton,
        }

        public static void InitPreprocessor()
        {
            RiqBeatmap.OnUpdateBeatmap += PreProcessBeatmap;
            RiqFileHandler.AudioConverter = JukeboxAudioConverter;
        }

        readonly public static Dictionary<string, object> propertiesModel = new()
        {
            // mapper set properties? (future: use this to flash the button)
            {"propertiesmodified", false},

            ////// CATEGORY 1: SONG INFO
            // general chart info
            {"remixtitle", "New Remix"},                                                                                        // chart name
            {"remixauthor", "Your Name"},                                                                                       // charter's name
            {"remixdesc", "Remix Description"},                                                                                 // chart description
            {"remixlevel", 1},                                                                                                  // chart difficulty (maybe offer a suggestion but still have the mapper determine it)
            {"remixtempo", 120f},                                                                                               // avg. chart tempo
            {"remixtags", ""},                                                                                                  // chart tags
            {"icontype", 0},                                                                                                    // chart icon (presets, custom - future)
            {"iconres", new EntityTypes.Resource(EntityTypes.Resource.ResourceType.Image, "Images/Select/", "Icon")},           // custom icon location (future)
            {"challengetype", 0},                                                                                               // perfect challenge type
            {"accessiblewarning", false},                                                                                       // epilepsy warning
            {"playstyle", RecommendedControlStyle.Any},                                                                         // recommended control style

            // chart song info
            {"idolgenre", "Song Genre"},                                                                                        // song genre
            {"idolsong", "Song Name"},                                                                                          // song name
            {"idolcredit", "Artist"},                                                                                           // song artist

            ////// CATEGORY 2: PROLOGUE AND EPILOGUE
            // chart prologue
            {"prologuetype", 0},                                                                                                // prologue card animation (future)
            {"prologuecaption", "Remix"},                                                                                       // prologue card sub-title (future)

            // chart results screen messages
            {"resultcaption", "Rhythm League Notes"},                                                                           // result screen header
            {"resultcommon_hi", "Good rhythm."},                                                                                // generic "Superb" message (one-liner)
            {"resultcommon_ok", "Eh. Passable."},                                                                               // generic "OK" message (one-liner)
            {"resultcommon_ng", "Try harder next time."},                                                                       // generic "Try Again" message (one-liner)

            {"resultcat0_hi", "You show strong fundamentals."},                                                                 // "Superb" message for input category 0 "normal" (two-liner)
            {"resultcat0_ng", "Work on your fundamentals."},                                                                    // "Try Again" message for input category 0 "normal" (two-liner)

            {"resultcat1_hi", "You kept the beat well."},                                                                       // "Superb" message for input category 1 "keep" (two-liner)
            {"resultcat1_ng", "You had trouble keeping the beat."},                                                             // "Try Again" message for input category 1 "keep" (two-liner)

            {"resultcat2_hi", "You had great aim."},                                                                            // "Superb" message for input category 2 "aim" (two-liner)
            {"resultcat2_ng", "Your aim was a little shaky."},                                                                  // "Try Again" message for input category 2 "aim" (two-liner)
                                                
            {"resultcat3_hi", "You followed the example well."},                                                                // "Superb" message for input category 3 "repeat" (two-liner)
            {"resultcat3_ng", "Next time, follow the example better."},                                                         // "Try Again" message for input category 3 "repeat" (two-liner)

            {"epilogue_hi", "Superb"},                                                                                          // epilogue "Superb" message
            {"epilogue_ok", "OK"},                                                                                              // epilogue "OK" message
            {"epilogue_ng", "Try Again"},                                                                                       // epilogue "Try Again" message

            {"epilogue_hi_res", new EntityTypes.Resource(EntityTypes.Resource.ResourceType.Image, "Images/Epilogue/", "Hi")},   // epilogue "Superb" image resource path
            {"epilogue_ok_res", new EntityTypes.Resource(EntityTypes.Resource.ResourceType.Image, "Images/Epilogue/", "Ok")},   // epilogue "OK" image resource path
            {"epilogue_ng_res", new EntityTypes.Resource(EntityTypes.Resource.ResourceType.Image, "Images/Epilogue/", "Ng")},   // epilogue "Try Again" image resource path
        };

        readonly static Dictionary<string, object> tempoChangeModel = new()
        {
            {"tempo", 120f},
            {"swing", 0f},
            {"swingDivision", 1f},
            {"timeSignature", new Vector2(4, 4)},
        };

        readonly static Dictionary<string, object> volumeChangeModel = new()
        {
            {"volume", 1f},
            {"fade", Util.EasingFunction.Ease.Instant},
        };

        readonly static Dictionary<string, object> sectionMarkModel = new()
        {
            {"sectionName", ""},
            {"startPerfect", false},
            {"weight", 1f},
            {"category", 0},
        };

        static void PreProcessSpecialEntity(RiqEntity e, Dictionary<string, object> model)
        {
            foreach (var t in model)
            {
                string propertyName = t.Key;
                Type type = t.Value.GetType();
                if (!e.dynamicData.ContainsKey(propertyName))
                {
                    e.CreateProperty(propertyName, t.Value);
                }
                Type pType = e[propertyName].GetType();
                if (pType != type)
                {
                    try
                    {
                        if (type.IsEnum)
                        {
                            if (pType == typeof(string))
                                e.dynamicData[propertyName] = (int)Enum.Parse(type, (string)e[propertyName]);
                            else
                                e.dynamicData[propertyName] = (int)e[propertyName];
                        }
                        else if (pType == typeof(Newtonsoft.Json.Linq.JObject))
                            e[propertyName] = e[propertyName].ToObject(type);
                        else
                            e[propertyName] = Convert.ChangeType(e[propertyName], type);
                    }
                    catch
                    {
                        Debug.LogWarning($"Could not convert {propertyName} to {type}! Using default value...");
                        // use default value
                        e.CreateProperty(propertyName, t.Value);
                    }
                }
            }
        }

        public static string JukeboxAudioConverter(string filePath, AudioType audioType, string specificType)
        {
            string wavCachePath = Path.Combine(Application.temporaryCachePath, "savewav");
            if (Directory.Exists(wavCachePath))
            {
                Directory.Delete(wavCachePath, true);
            }
            if (!Directory.Exists(wavCachePath))
            {
                Directory.CreateDirectory(wavCachePath);
            }
            if (audioType == AudioType.MPEG)
            {
                Debug.Log($"mp3 loaded, Converting {filePath} to wav...");
                // convert mp3 to wav
                // import the mp3 as an audioclip
                string url = "file://" + filePath;
                using (var www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
                {
                    www.SendWebRequest();
                    while (!www.isDone) { }
                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.LogError($"Could not load audio file {filePath}! Error: {www.error}");
                        return filePath;
                    }
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    SavWav.Save(fileName, clip, true);
                    filePath = Path.Combine(wavCachePath, $"{fileName}.wav");

                    clip = null;
                }
            }
            return filePath;
        }

        /// <summary>
        /// processes an riq beatmap after it is loaded
        /// </summary>
        public static RiqBeatmapData? PreProcessBeatmap(string version, RiqBeatmapData data)
        {
            Debug.Log("Preprocessing beatmap...");
            Minigame game;
            GameAction action;
            Type type, pType;
            if (EventCaller.instance != null)
            {
                string[] split;
                foreach (var e in data.entities)
                {
                    split = e.datamodel.Split('/');
                    var gameName = split[0];
                    var actionName = split[1];
                    game = EventCaller.instance.GetMinigame(gameName);
                    if (game == null)
                    {
                        Debug.LogWarning($"Unknown game {gameName} found in remix.json! Adding game...");
                        game = new Minigame(gameName, gameName.DisplayName() + " \n<color=#eb5454>[inferred from remix.json]</color>", "", false, false, new List<GameAction>(), inferred: true);
                        EventCaller.instance.minigames.Add(gameName, game);
                        if (Editor.Editor.instance != null)
                            Editor.Editor.instance.AddIcon(game);
                    }
                    action = EventCaller.instance.GetGameAction(game, actionName);
                    if (action == null)
                    {
                        Debug.LogWarning($"Unknown action {gameName}/{actionName} found in remix.json! Adding action...");
                        var parameters = new List<Param>();
                        foreach (var item in e.dynamicData)
                        {
                            Debug.Log($"k: {item.Key}, v: {item.Value}");
                            if (item.Key == "track")
                                continue;
                            if (item.Value == null)
                                continue;
                            var value = item.Value;
                            if (value.GetType() == typeof(long))
                                value = new EntityTypes.Integer(int.MinValue, int.MaxValue, (int)value);
                            else if (value.GetType() == typeof(double))
                                value = new EntityTypes.Float(float.NegativeInfinity, float.PositiveInfinity, (float)value);
                            parameters.Add(new Param(item.Key, value, item.Key.DisplayName(), "[inferred from remix.json]"));
                        }
                        action = new GameAction(actionName, actionName.DisplayName(), "Events", e.length, true, parameters);
                        game.actions.Add(action);
                    }

                    //check each param of the action
                    if (action.parameters != null)
                    {
                        foreach (var param in action.parameters)
                        {
                            type = param.parameter.GetType();
                            //add property if it doesn't exist
                            if (!e.dynamicData.ContainsKey(param.propertyName))
                            {
                                Debug.LogWarning($"Property {param.propertyName} does not exist in the entity's dynamic data! Adding...");
                                if (param.parameter is EntityTypes.Integer intParam)
                                    e.dynamicData.Add(param.propertyName, intParam.val);
                                else if (param.parameter is EntityTypes.Float floatParam)
                                    e.dynamicData.Add(param.propertyName, floatParam.val);
                                else if (param.parameter is EntityTypes.Dropdown ddParam)
                                    e.dynamicData.Add(param.propertyName, new EntityTypes.DropdownObj(ddParam));
                                else if (param.parameter is EntityTypes.Note noteParam)
                                    e.dynamicData.Add(param.propertyName, noteParam.val);
                                else if (type.IsEnum)
                                    e.dynamicData.Add(param.propertyName, (int)param.parameter);
                                else
                                    e.dynamicData.Add(param.propertyName, Convert.ChangeType(param.parameter, type));
                                continue;
                            }
                            pType = e[param.propertyName].GetType();
                            if (pType != type)
                            {
                                try
                                {
                                    if (type == typeof(EntityTypes.Integer) || type == typeof(EntityTypes.Note))
                                        e.dynamicData[param.propertyName] = (int)e[param.propertyName];
                                    else if (type == typeof(EntityTypes.Float))
                                        e.dynamicData[param.propertyName] = (float)e[param.propertyName];
                                    else if (type == typeof(EntityTypes.Button))
                                        e.dynamicData[param.propertyName] = (string)e[param.propertyName];
                                    else if (type == typeof(EntityTypes.Dropdown)) {
                                        JValue value = e[param.propertyName].value;
                                        JArray values = e[param.propertyName].Values;
                                        e.dynamicData[param.propertyName] = new EntityTypes.DropdownObj((int)value, values.Select(x => (string)x).ToList());
                                    }
                                    else if (type == typeof(EntityTypes.NoteSampleDropdown))
                                    {
                                        e.dynamicData[param.propertyName] = (int)e[param.propertyName];
                                    }
                                    else if (type == typeof(EntityTypes.Resource))
                                        e.dynamicData[param.propertyName] = (EntityTypes.Resource)e[param.propertyName];
                                    else if (type.IsEnum)
                                    {
                                        if (pType == typeof(string))
                                            e.dynamicData[param.propertyName] = (int)Enum.Parse(type, (string)e[param.propertyName]);
                                        else
                                            e.dynamicData[param.propertyName] = (int)e[param.propertyName];
                                    }
                                    else if (pType == typeof(Newtonsoft.Json.Linq.JObject))
                                        e.dynamicData[param.propertyName] = e[param.propertyName].ToObject(type);
                                    else
                                        e.dynamicData[param.propertyName] = Convert.ChangeType(e[param.propertyName], type);
                                }
                                catch
                                {
                                    Debug.LogWarning($"Could not convert {param.propertyName} to {type}! Using default value...");
                                    // GlobalGameManager.ShowErrorMessage("Warning", $"Could not convert {e.datamodel}/{param.propertyName} to {type}! This will be loaded using the default value, so chart may be unstable.");
                                    // use default value
                                    if (type == typeof(EntityTypes.Integer))
                                        e.dynamicData[param.propertyName] = ((EntityTypes.Integer)param.parameter).val;
                                    else if (type == typeof(EntityTypes.Float))
                                        e.dynamicData[param.propertyName] = ((EntityTypes.Float)param.parameter).val;
                                    // else if (type == typeof(EntityTypes.Dropdown))
                                    //     e.dynamicData[param.propertyName] = new EntityTypes.DropdownObj();
                                    else if (type == typeof(EntityTypes.Note))
                                        e.dynamicData[param.propertyName] = ((EntityTypes.Note)param.parameter).val;
                                    else if (type.IsEnum && param.propertyName != "ease")
                                        e.dynamicData[param.propertyName] = (int)param.parameter;
                                    else
                                        e.dynamicData[param.propertyName] = Convert.ChangeType(param.parameter, type);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var tempo in data.tempoChanges)
            {
                PreProcessSpecialEntity(tempo, tempoChangeModel);
            }
            if (data.tempoChanges[0]["tempo"] <= 0)
            {
                data.tempoChanges[0]["tempo"] = 120;
            }

            foreach (var vol in data.volumeChanges)
            {
                PreProcessSpecialEntity(vol, volumeChangeModel);
            }

            foreach (var section in data.beatmapSections)
            {
                PreProcessSpecialEntity(section, sectionMarkModel);
            }

            //go thru each property of the model beatmap and add any missing keyvalue pair
            foreach (var prop in propertiesModel)
            {
                var mType = propertiesModel[prop.Key].GetType();
                if (!data.properties.ContainsKey(prop.Key))
                {
                    data.properties.Add(prop.Key, prop.Value);
                }
                else
                {
                    // convert enums to the intended enum type
                    if (mType.IsEnum)
                    {
                        if (data.properties[prop.Key].GetType() == typeof(string))
                            data.properties[prop.Key] = Enum.Parse(mType, (string)data.properties[prop.Key]);
                        else
                            data.properties[prop.Key] = Enum.ToObject(mType, data.properties[prop.Key]);
                    }
                    // convert all JObjects to their respective types
                    else if (data.properties[prop.Key].GetType() == typeof(Newtonsoft.Json.Linq.JObject))
                    {
                        data.properties[prop.Key] = (data.properties[prop.Key] as Newtonsoft.Json.Linq.JObject).ToObject(mType);
                    }
                }
            }

            return data;
        }

        public class Minigame
        {

            public string name;
            public string displayName;
            public string color;
            public string splitColorL;
            public string splitColorR;
            public bool hidden;
            public bool fxOnly;
            public List<GameAction> actions = new List<GameAction>();
            

            public uint? chronologicalSortKey;
            // Yin: basically i figure this should just be whatever index number the minigame is
            // in its game of origin. So, basically, like, if we're talking Rhythm Heaven DS, Built to
            // Scale would be 1, then Fillbots would be 2, and so on. If it's an endless game, add 100.
            // If it's a rhythm toy, add 200. If it's a 2-Player endless game, add 300. If it's a credits
            // game... IDK, I guess just set it to 99. It works. If the game isn't a RH original then just
            // put the date in YYYYMMDD format. Oh, and if it's a practice game set it to 0.

            public List<string> tags;
            public string defaultLocale = "en";
            public string wantAssetBundle = null;
            public List<string> supportedLocales;
            public bool inferred;

            public bool UsesAssetBundle => (wantAssetBundle is not null or "") && (!badBundle);
            public bool HasLocales => supportedLocales.Count > 0;
            public bool AssetsLoaded => (!badBundle) && ((HasLocales && audioLoaded && currentLoadedLocale == defaultLocale) || (!HasLocales)) && (LoadedPrefab != null) && resourcesLoaded && loadComplete;
            public bool AlreadyLoading => alreadyLoading;

            public bool SequencesPreloaded => soundSequences != null;
            public string LoadableName => inferred ? "noGame" : name;
            public GameObject LoadedPrefab => loadedPrefab;

            private string currentLoadedLocale = "";
            private AssetBundle bundleResources = null;
            private bool resourcesLoaded = false;
            private bool resourcesPreloaded = false;
            private AssetBundle bundleAudio = null;
            private bool audioLoaded = false;
            private bool audioPreloaded = false;
            private GameObject loadedPrefab = null;
            private bool badBundle = false;
            // eventually implement secondary assetbundles for localization instead of one "common" and one "locale"

            bool loadComplete = false;

            private SoundSequence.SequenceKeyValue[] soundSequences = null;

            public SoundSequence.SequenceKeyValue[] LoadedSoundSequences
            {
                get => soundSequences;
                set => soundSequences = value;
            }

            public Minigame(string name, string displayName, string color, bool hidden, bool fxOnly, List<GameAction> actions, List<string> tags = null, string wantAssetBundle = null, string defaultLocale = "en", List<string> supportedLocales = null, bool inferred = false, uint? chronologicalSortKey = null)
            {
                this.name = name;
                this.displayName = displayName;
                this.color = color;
                this.actions = actions;
                this.hidden = hidden;
                this.fxOnly = fxOnly;

                this.tags = tags ?? new List<string>();
                this.wantAssetBundle = wantAssetBundle;
                this.defaultLocale = defaultLocale;
                this.supportedLocales = supportedLocales ?? new List<string>();
                this.inferred = inferred;

                this.splitColorL = null;
                this.splitColorR = null;

                this.chronologicalSortKey = chronologicalSortKey;
            }

            public Minigame(string name, string displayName, string color, string splitColorL, string splitColorR, bool hidden, bool fxOnly, List<GameAction> actions, List<string> tags = null, string wantAssetBundle = null, string defaultLocale = "en", List<string> supportedLocales = null, bool inferred = false, uint? chronologicalSortKey = null)
            {
                this.name = name;
                this.displayName = displayName;
                this.color = color;
                this.actions = actions;
                this.hidden = hidden;
                this.fxOnly = fxOnly;

                this.tags = tags ?? new List<string>();
                this.wantAssetBundle = wantAssetBundle;
                this.defaultLocale = defaultLocale;
                this.supportedLocales = supportedLocales ?? new List<string>();
                this.inferred = inferred;

                this.splitColorL = splitColorL;
                this.splitColorR = splitColorR;

                this.chronologicalSortKey = chronologicalSortKey;
            }

            bool alreadyLoading = false;
            public async UniTaskVoid LoadAssetsAsync()
            {
                if (alreadyLoading || AssetsLoaded || !UsesAssetBundle) return;
                loadComplete = false;
                alreadyLoading = true;
                await UniTask.WhenAll(LoadResourcesAssetBundleAsync(), LoadAudioAssetBundleAsync());
                if (badBundle)
                {
                    Debug.LogWarning($"Bad bundle for {name}");
                    alreadyLoading = false;
                    loadComplete = true;
                    return;
                }
                await UniTask.WhenAll(LoadGamePrefabAsync(), PrepareResources(), PrepareAudio());
                SoundByte.PreloadGameAudioClips(this);
                alreadyLoading = false;
                loadComplete = true;
            }

            public AssetBundle GetAudioAssetBundle()
            {
                if (bundleAudio != null && !audioLoaded)
                {
                    bundleAudio.Unload(true);
                    bundleAudio = null;
                    audioLoaded = false;
                    audioPreloaded = false;
                }
                if (!HasLocales) return null;
                if (!UsesAssetBundle) return null;
                if (bundleAudio == null || currentLoadedLocale != defaultLocale) //TEMPORARY: use the game's default locale until we add localization support
                {
                    if (audioLoaded) return bundleAudio;
                    // TODO: try/catch for missing assetbundles
                    currentLoadedLocale = defaultLocale;
                    bundleAudio = AssetBundle.LoadFromFile(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, wantAssetBundle, "locale." + defaultLocale)));
                    audioLoaded = true;
                }
                return bundleAudio;
            }

            public AssetBundle GetResourcesAssetBundle()
            {
                if (badBundle) return null;
                string path = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, wantAssetBundle, "common"));
                if (bundleResources != null && !resourcesLoaded)
                {
                    bundleResources.Unload(true);
                    bundleResources = null;
                    resourcesLoaded = false;
                    resourcesPreloaded = false;
                }
                if (!File.Exists(path))
                {
                    badBundle = true;
                    return null;
                }
                if (resourcesLoaded) return bundleResources;
                if (!UsesAssetBundle) return null;
                if (bundleResources == null)
                {
                    bundleResources = AssetBundle.LoadFromFile(path);
                    resourcesLoaded = true;
                }
                return bundleResources;
            }

            public async UniTask LoadResourcesAssetBundleAsync()
            {
                if (badBundle) return;
                string path = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, wantAssetBundle, "common"));
                if (bundleResources != null && !resourcesLoaded)
                {
                    await bundleResources.UnloadAsync(true);
                    bundleResources = null;
                    resourcesLoaded = false;
                    resourcesPreloaded = false;
                }
                // ignore all AB checks if path doesn't exist
                if (!File.Exists(path))
                {
                    badBundle = true;
                    return;
                }
                if (resourcesPreloaded || resourcesLoaded) return;
                resourcesPreloaded = true;
                if (!UsesAssetBundle) return;
                if (bundleResources != null) return;
                AssetBundle bundle = await AssetBundle.LoadFromFileAsync(path).ToUniTask(timing: PlayerLoopTiming.PreLateUpdate);
                bundleResources = bundle;
                resourcesLoaded = true;
            }

            public async UniTask LoadAudioAssetBundleAsync()
            {
                if (bundleAudio != null && !audioLoaded)
                {
                    await bundleAudio.UnloadAsync(true);
                    bundleAudio = null;
                    audioLoaded = false;
                    audioPreloaded = false;
                }

                if (!HasLocales) return;
                if (audioPreloaded) return;
                audioPreloaded = true;
                if (!UsesAssetBundle) return;
                if (audioLoaded && bundleAudio != null && currentLoadedLocale == defaultLocale) return;

                AssetBundle bundle = await AssetBundle.LoadFromFileAsync(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, wantAssetBundle, "locale." + defaultLocale))).ToUniTask(timing: PlayerLoopTiming.PreLateUpdate);
                if (audioLoaded && bundleAudio != null && currentLoadedLocale == defaultLocale) return;

                bundleAudio = bundle;
                currentLoadedLocale = defaultLocale;
                audioLoaded = true;
            }

            public async UniTask LoadGamePrefabAsync()
            {
                if (!UsesAssetBundle) return;
                if (!resourcesLoaded) return;
                if (bundleResources == null) return;
                if (badBundle) return;

                AssetBundleRequest request = bundleResources.LoadAssetAsync<GameObject>(name);
                request.completed += (op) => OnPrefabLoaded(op as AssetBundleRequest);
                await request;
                loadedPrefab = request.asset as GameObject;
            }

            void OnPrefabLoaded(AssetBundleRequest request)
            {
                GameObject prefab = request.asset as GameObject;
                // // load sound sequences here for now
                // // this is taxing and is still done synchronously
                // // move sequences to their own assets so that we don't have to look up a component
                if (prefab.TryGetComponent<Games.Minigame>(out Games.Minigame minigame))
                {
                    soundSequences = minigame.SoundSequences;
                }
                loadedPrefab = prefab;
            }

            public async UniTask PrepareResources()
            {
                if (!resourcesLoaded) return;
                if (bundleResources == null) return;

                var assets = bundleResources.LoadAllAssetsAsync();
                await assets;
            }

            public async UniTask PrepareAudio()
            {
                if (!audioLoaded) return;
                if (bundleAudio == null) return;

                var assets = bundleAudio.LoadAllAssetsAsync();
                await assets;
            }

            public async UniTask UnloadAllAssets()
            {
                if (!UsesAssetBundle) return;
                if (loadedPrefab != null)
                {
                    loadedPrefab = null;
                }
                if (bundleResources != null)
                {
                    await bundleResources.UnloadAsync(true);
                    bundleResources = null;
                    resourcesLoaded = false;
                    resourcesPreloaded = false;
                }
                if (bundleAudio != null)
                {
                    await bundleAudio.UnloadAsync(true);
                    bundleAudio = null;
                    audioLoaded = false;
                    audioPreloaded = false;
                }
                SoundByte.UnloadAudioClips(name);
                loadComplete = false;
            }
        }

        public class GameAction
        {
            public string actionName;
            public string displayName;
            public string tabName;
            public EventCallback function = delegate { };
            public float defaultLength = 1;
            public int defaultVersion = 0;
            public bool resizable = false;
            public List<Param> parameters = null;
            public bool hidden = false;
            public int priority = 0;
            public EventCallback inactiveFunction = delegate { };
            public EventCallback preFunction = delegate { };
            public float preFunctionLength = 2.0f;

            /// <summary>
            /// <para>Creates a block that can be used in the editor. The block's function and attributes are defined in the parentheses.</para>
            /// <para>Note: Every parameter after the second one is an optional parameter. You can change optional parameters by adding (name): (value) after the second parameter.</para>
            /// </summary>
            /// <param name="actionName">Entity model name</param>
            /// <param name="displayName">Name of the block used in the UI</param>
            /// <param name="defaultLength">How long the block appears in the editor</param>
            /// <param name="resizable">Allows the user to resize the block</param>
            /// <param name="parameters">Extra parameters for this block that change how it functions.</param>
            /// <param name="function"><para>What the block does when read during playback</para>
            /// <para>Only does this if the game that it is associated with is loaded.</para></param>
            /// <param name="inactiveFunction">What the block does when read while the game it's associated with isn't loaded.</param>
            /// <param name="preFunction">What the block does when the GameManager seeks to this cue for pre-scheduling.</param>
            /// <param name="hidden">Prevents the block from being shown in the game list. Block will still function normally if it is in the timeline.</param>
            /// <param name="priority">Priority of this event. Higher priority events will be run first.</param>
            public GameAction(string actionName, string displayName, string tabName, float defaultLength = 1, bool resizable = false, List<Param> parameters = null, EventCallback function = null, EventCallback inactiveFunction = null, EventCallback preFunction = null, bool hidden = false, int priority = 0, float preFunctionLength = 2.0f, int defaultVersion = 0)
            {
                this.actionName = actionName;
                this.displayName = string.IsNullOrEmpty(displayName) ? actionName : displayName;
                this.tabName = string.IsNullOrEmpty(tabName) ? "Events" : tabName;

                this.defaultLength = defaultLength;
                this.resizable = resizable;
                this.parameters = parameters;
                this.hidden = hidden;

                this.function = function ?? delegate { };
                this.inactiveFunction = inactiveFunction ?? delegate { };
                this.preFunction = preFunction ?? delegate { };
                this.priority = priority;
                this.preFunctionLength = preFunctionLength;
                this.defaultVersion = defaultVersion;
            }

            /// <summary>
            /// <para>Shorthand constructor for a GameAction with only required data</para>
            /// </summary>
            /// <param name="actionName">Entity model name</param>
            /// <param name="displayName">Name of the block used in the UI</param>
            /// <param name="tabName">Name of the tab to be under when spawning events</param>
            public GameAction(string actionName, string displayName, string tabName = "")
            {
                this.actionName = actionName;
                this.displayName = string.IsNullOrEmpty(displayName) ? actionName : displayName;
                this.tabName = tabName == string.Empty ? "Events" : tabName; // keep it null if it's null
            }
        }

        [System.Serializable]
        public class Param
        {
            public string propertyName;
            public object parameter;
            public string caption;
            public string tooltip;
            public List<CollapseParam> collapseParams;

            /// <summary>
            /// A parameter that changes the function of a GameAction.
            /// </summary>
            /// <param name="propertyName">The name of the variable that's being changed.</param>
            /// <param name="parameter">The value of the parameter</param>
            /// <param name="caption">The name shown in the editor. Can be anything you want.</param>
            public Param(string propertyName, object parameter, string caption, string tooltip = "", List<CollapseParam> collapseParams = null)
            {
                this.propertyName = propertyName;
                this.parameter = parameter;
                this.caption = caption;
                this.tooltip = tooltip;
                this.collapseParams = collapseParams;
            }

            public class CollapseParam
            {
                public Func<object, RiqEntity, bool> CollapseOn;
                public string[] collapseables;
                /// <summary>
                /// Class that decides how other parameters will be collapsed
                /// </summary>
                /// <param name="collapseOn">What values should make it collapse/uncollapse?</param>
                /// <param name="collapseables">IDs of the parameters to collapse</param>
                public CollapseParam(Func<object, RiqEntity, bool> collapseOn, params string[] collapseables)
                {
                    CollapseOn = collapseOn;
                    this.collapseables = collapseables;
                }
            }
        }

        public delegate void EventCallback();
        public delegate void ParamChangeCallback(string paramName, object paramValue, RiqEntity entity);

        public static void Init(EventCaller eventCaller)
        {
            List<Minigame> defaultGames = new()
            {
                new Minigame("gameManager", "Game Manager", "", false, true, new List<GameAction>()
                {
                    new GameAction("switchGame", "Switch Game", null, 0.5f, false,
                        function: delegate { var e = eventCaller.currentEntity; GameManager.instance.SwitchGame(eventCaller.currentSwitchGame, eventCaller.currentEntity.beat, e["toggle"]); },
                        parameters: new List<Param>()
                            {
                                new Param("toggle", true, "Black Flash", "Toggle if there should be a black flash before the game is switched. You should only disable this if you know what you're doing.")
                            },
                        inactiveFunction: delegate { var e = eventCaller.currentEntity; GameManager.instance.SwitchGame(eventCaller.currentSwitchGame, eventCaller.currentEntity.beat, e["toggle"]); }
                    ),
                    new GameAction("end", "End Remix", "",
                        function: delegate {
                            Debug.Log("end");
                            if (Timeline.instance != null)
                                Timeline.instance?.Stop(Timeline.instance.PlaybackBeat);
                            else
                                GameManager.instance.Stop(eventCaller.currentEntity.beat);
                        }
                    ),
                    new GameAction("skill star", "Skill Star", "", 1f, true)
                    {
                        //temp for testing
                        function = delegate {
                            var e = eventCaller.currentEntity;
                            Common.SkillStarManager.instance.DoStarIn(e.beat, e.length);
                        }
                    },
                    new GameAction("toggle inputs", "Toggle Inputs", "", 0.5f, true,
                        new List<Param>()
                        {
                            new Param("toggle", true, "Allow Inputs", "Toggle if the player is able to input. Any missed cues while this is disabled will not be counted as a miss and will not break a perfect.")
                        },
                        delegate
                        {
                            GameManager.instance.ToggleInputs(eventCaller.currentEntity["toggle"]);
                        }
                    ),
                }),

                new Minigame("countIn", "Count-Ins", "", false, true, new List<GameAction>()
                {
                    new GameAction("count-in", "Count-In", "Built", 4f, true,
                        new List<Param>()
                        {
                            new Param("alt", false, "Alt", "Set the type of sounds to use for the count-in."),
                            new Param("go", false, "Go!", "Toggle to end the count-in with \"Go!\""),
                        },
                        preFunction : delegate {
                            var e = eventCaller.currentEntity;
                            SoundEffects.CountIn(e.beat, e.length, e["alt"], e["go"]);
                        }
                    ),
                    new GameAction("4 beat count-in", "4 Beat Count-In", "Built", 4f, true,
                        new List<Param>()
                        {
                            new Param("type", SoundEffects.CountInType.Normal, "Type", "Set the type of sounds to use for the count-in.")
                        },
                        delegate {
                            var e = eventCaller.currentEntity;
                            SoundEffects.FourBeatCountIn(e.beat, e.length / 4f, e["type"]);
                        }
                    ),
                    new GameAction("8 beat count-in", "8 Beat Count-In", "Built", 8f, true,
                        new List<Param>()
                        {
                            new Param("type", SoundEffects.CountInType.Normal, "Type", "Set the type of sounds to use for the count-in.")
                        },
                        delegate {
                            var e = eventCaller.currentEntity;
                            SoundEffects.EightBeatCountIn(e.beat, e.length / 8f, e["type"]);
                        }
                    ),
                    new GameAction("count", "Count", "Single", 1f, false,
                        new List<Param>()
                        {
                            new Param("type", SoundEffects.CountNumbers.One, "Type", "Set the number to say."),
                            new Param("toggle", false, "Alt", "Toggle if the alternate version of this voice line should be used.")
                        },
                        delegate {
                            var e = eventCaller.currentEntity;
                            SoundEffects.Count(e["type"], e["toggle"]);
                        }
                    ),
                    new GameAction("cowbell", "Cowbell", "Single",
                        function: delegate { SoundEffects.Cowbell(); }
                    ),
                    new GameAction("ready!", "Ready!", "Single", 2f, true,
                        function: delegate { var e = eventCaller.currentEntity; SoundEffects.Ready(e.beat, (e.length / 2f)); }
                    ),
                    new GameAction("and", "And", "Single", 0.5f,
                        function: delegate { SoundEffects.And(); }
                    ),
                    new GameAction("go!", "Go!", "Single", 1f, false,
                        new List<Param>()
                        {
                            new Param("toggle", false, "Alt", "Toggle if the alternate version of this voice line should be used.")
                        },
                        function: delegate { SoundEffects.Go(eventCaller.currentEntity["toggle"]); }
                    ),

                    // // These are still here for backwards-compatibility but are hidden in the editor
                    // new GameAction("4 beat count-in (alt)", "", 4f, function: delegate { var e = eventCaller.currentEntity; SoundEffects.FourBeatCountIn(e.beat, e.length, 1); }, hidden: true),
                    // new GameAction("4 beat count-in (cowbell)", "", 4f, function: delegate { var e = eventCaller.currentEntity; SoundEffects.FourBeatCountIn(e.beat, e.length, 2); }, hidden: true),
                    // new GameAction("8 beat count-in (alt)", "", 8f, function: delegate { var e = eventCaller.currentEntity; SoundEffects.EightBeatCountIn(e.beat, e.length, 1); }, hidden: true),
                    // new GameAction("8 beat count-in (cowbell)", "", 8f, function: delegate { var e = eventCaller.currentEntity; SoundEffects.EightBeatCountIn(e.beat, e.length, 2); }, hidden: true),

                    // new GameAction("one", "", function: delegate { SoundEffects.Count(0, false); }, hidden: true),
                    // new GameAction("two", "", function: delegate { SoundEffects.Count(1, false); }, hidden: true),
                    // new GameAction("three", "", function: delegate { SoundEffects.Count(2, false); }, hidden: true),
                    // new GameAction("four", "", function: delegate { SoundEffects.Count(3, false); }, hidden: true),
                    // new GameAction("one (alt)", "", function: delegate { SoundEffects.Count(0, true); }, hidden: true),
                    // new GameAction("two (alt)", "", function: delegate { SoundEffects.Count(1, true); }, hidden: true),
                    // new GameAction("three (alt)", "", function: delegate { SoundEffects.Count(2, true); }, hidden: true),
                    // new GameAction("four (alt)", "", function: delegate { SoundEffects.Count(3, true); }, hidden: true),
                    // new GameAction("go! (alt)", "", function: delegate { SoundEffects.Go(true); }, hidden: true),
                }),

                new Minigame("vfx", "Visual Effects", "", false, true, new List<GameAction>()
                {
                    new GameAction("flash", "Flash/Fade", "VFX", 1f, true,
                        new List<Param>()
                        {
                            new Param("colorA", Color.white, "Start Color", "Set the color at the start of the event."),
                            new Param("colorB", Color.white, "End Color", "Set the color at the end of the event."),
                            new Param("valA", new EntityTypes.Float(0, 1, 1), "Start Opacity", "Set the opacity at the start of the event."),
                            new Param("valB", new EntityTypes.Float(0, 1, 0), "End Opacity", "Set the opacity at the end of the event."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "colorA", "valA" })
                            })
                        }
                    ),
                    new GameAction("filter", "Filter", "VFX", 1f, true,
                        new List<Param>()
                        {
                            new Param("filter", Games.Global.Filter.FilterType.grayscale, "Type", "Set the type of filter to use."),
                            // old

                            /*new Param("inten", new EntityTypes.Float(0, 100, 100), "Intensity"),
                            new Param("fadein", new EntityTypes.Float(0, 100, 0), "Fade In"),
                            new Param("fadeout", new EntityTypes.Float(0, 100, 0), "Fade Out")*/

                            // new
                            new Param("slot", new EntityTypes.Integer(1, 10, 1), "Slot", "Set the slot for the filter. Higher slots are applied first. There can only be one filter per slot."),
                            new Param("start", new EntityTypes.Float(0, 1, 1), "Start Intensity", "Set the intensity at the start of the event."),
                            new Param("end", new EntityTypes.Float(0, 1, 1), "End Intensity", "Set the intensity at the end of the event."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "start" })
                            }),
                        }
                    ),
                    new GameAction("move camera", "Move Camera", "Camera", 1f, true, new List<Param>()
                        {
                            new Param("valA", new EntityTypes.Float(-50, 50, 0), "Right / Left", "Set the position on the X axis."),
                            new Param("valB", new EntityTypes.Float(-50, 50, 0), "Up / Down", "Set the position on the Y axis."),
                            new Param("valC", new EntityTypes.Float(-250, 250, 10), "In / Out", "Set the position on the Z axis."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                            new Param("axis", GameCamera.CameraAxis.All, "Axis", "Set if only a specific axis should be modified." )
                        }
                    ),
                    new GameAction("rotate camera", "Rotate Camera", "Camera", 1f, true, new List<Param>()
                        {
                            new Param("valA", new EntityTypes.Float(-360, 360, 0), "Pitch", "Set the up/down rotation."),
                            new Param("valB", new EntityTypes.Float(-360, 360, 0), "Yaw", "Set the left/right rotation."),
                            new Param("valC", new EntityTypes.Float(-360, 360, 0), "Roll", "Set the clockwise/counterclockwise rotation."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                            new Param("axis", GameCamera.CameraAxis.All, "Axis", "Set if only a specific axis should be modified." )
                        }
                    ),
                     new GameAction("pan view", "Pan Viewport", "Camera", 1f, true, new List<Param>()
                        {
                            new Param("valA", new EntityTypes.Float(-50, 50, 0), "Right / Left", "Set the position on the X axis."),
                            new Param("valB", new EntityTypes.Float(-50, 50, 0), "Up / Down", "Set the position on the Y axis."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                            new Param("axis", StaticCamera.ViewAxis.All, "Axis", "Set if only a specific axis should be modified." )
                        }
                    ),
                    new GameAction("rotate view", "Rotate Viewport", "Camera", 1f, true, new List<Param>()
                        {
                            new Param("valA", new EntityTypes.Float(-360, 360, 0), "Rotation", "Set the clockwise/counterclockwise rotation."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                        }
                    ),
                    new GameAction("scale view", "Scale Viewport", "Camera", 1f, true, new List<Param>()
                        {
                            new Param("valA", new EntityTypes.Float(-50f, 50, 1), "Width", "Set the width of the viewport."),
                            new Param("valB", new EntityTypes.Float(-50f, 50, 1), "Height", "Set the height of the viewport."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action."),
                            new Param("axis", StaticCamera.ViewAxis.All, "Axis", "Set if only a specific axis should be modified." )
                        }
                    ),
                    new("stretch camera", "Stretch Camera", "Camera")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("x1", new EntityTypes.Float(-50f, 50f, 1f), "Start Width", "Set the width at the start of the event."),
                            new("x2", new EntityTypes.Float(-50f, 50f, 1f), "End Width", "Set the width at the end of the event."),
                            new("y1", new EntityTypes.Float(-50f, 50f, 1f), "Start Height", "Set the height at the start of the event."),
                            new("y2", new EntityTypes.Float(-50f, 50f, 1f), "End Height", "Set the height at the end of the event."),
                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "x1", "y1" })
                            }),
                            new Param("axis", GameCamera.CameraAxis.All, "Axis", "Set if only a specific axis should be modified.")
                        }
                    },
                    new GameAction("fitScreen", "Force Game Stretching To Window", "Camera")
                    {
                        defaultLength = 0.5f,
                        parameters = new()
                        {
                            new("enable", true, "Enabled", "Toggle if the game should be forced to stretch to the window size, removing the letterbox.")
                        }
                    },
                    new GameAction("screen shake", "Screen Shake", "Camera", 1f, true,
                        new List<Param>()
                        {
                            new Param("easedA", new EntityTypes.Float(0, 10, 0), "Start Horizontal Intensity", "Set the horizontal intensity of the screen shake at the start of the event."),
                            new Param("valA", new EntityTypes.Float(0, 10, 0), "End Horizontal Intensity", "Set the horizontal intensity of the screen shake at the end of the event."),
                            new Param("easedB", new EntityTypes.Float(0, 10, 0.5f), "Start Vertical Intensity", "Set the vertical intensity of the screen shake at the start of the event."),
                            new Param("valB", new EntityTypes.Float(0, 10, 0.5f), "End Vertical Intensity", "Set the vertical intensity of the screen shake at the end of the event."),
                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "easedA", "easedB" })
                            }),
                        }
                    ),
                    new GameAction("display textbox", "Display Textbox", "Text", 1f, true, new List<Param>()
                        {
                            new Param("text1", "", "Text", "Set the text to display in the textbox. Rich text is supported."),
                            new Param("type", Games.Global.Textbox.TextboxAnchor.TopMiddle, "Anchor", "Set where to anchor the textbox."),
                            new Param("valA", new EntityTypes.Float(0.25f, 4, 1), "Width", "Set the width of the textbox."),
                            new Param("valB", new EntityTypes.Float(0.5f, 8, 1), "Height", "Set the height of the textbox.")
                        }
                    ),
                    new GameAction("display open captions", "Display Open Captions", "Text", 1f, true,
                        new List<Param>()
                        {
                            new Param("text1", "", "Text", "Set the text to display in the captions. Rich text is supported."),
                            new Param("type", Games.Global.Textbox.TextboxAnchor.BottomMiddle, "Anchor", "Set where to anchor the captions."),
                            new Param("valA", new EntityTypes.Float(0.25f, 4, 1), "Width", "Set the width of the captions."),
                            new Param("valB", new EntityTypes.Float(0.5f, 8, 1), "Height", "Set the height of the captions.")
                        }
                    ),
                    new GameAction("display closed captions", "Display Closed Captions", "Text", 1f, true,
                        new List<Param>()
                        {
                            new Param("text1", "", "Text", "Set the text to display in the captions. Rich text is supported."),
                            new Param("type", Games.Global.Textbox.ClosedCaptionsAnchor.Top, "Anchor", "Set where to anchor the captions."),
                            new Param("valA", new EntityTypes.Float(0.5f, 4, 1), "Height", "Set the height of the captions.")
                        }
                    ),
                    new GameAction("display song artist", "Display Song Info", "Text", 1f, true,
                        new List<Param>()
                        {
                            new Param("text1", "", "Title", "Set the text to display in the upper label. Rich text is supported."),
                            new Param("text2", "", "Artist", "Set the text to display in the lower label. Rich text is supported."),
                            new Param("instantOn", false, "Instant Show", "Toggle if the slide-in animation should be skipped."),
                            new Param("instantOff", false, "Instant Hide", "Toggle if the slide-out animation should be skipped."),
                        }
                    ),
                    new GameAction("camera background color", "Camera Background Color", "Camera", 1, true, new List<Param>()
                        {
                            new Param("color", Color.black, "Start Color", "Set the color at the start of the event."),
                            new Param("color2", Color.black, "End Color", "Set the color at the end of the event."),
                            new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                        }
                    ),

                    // Post Processing VFX
                    new GameAction("vignette", "Vignette", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 20f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 20f, 1f), "End Intensity", "Set the intensity at the end of the event."),

                            new("colorStart", Color.black, "Start Color", "Set the color at the start of the event."),
                            new("colorEnd", Color.black, "End Color", "Set the color at the end of the event."),

                            new("xLocStart", new EntityTypes.Float(0.0f, 1f, 0.5f), "Start X Location", "Set the X location at the start of the event."),
                            new("xLocEnd", new EntityTypes.Float(0.0f, 1f, 0.5f), "End X Location", "Set the X location at the end of the event."),

                             new("yLocStart", new EntityTypes.Float(0.0f, 1f, 0.5f), "Start Y Location", "Set the Y location at the start of the event."),
                            new("yLocEnd", new EntityTypes.Float(0.0f, 1f, 0.5f), "End Y Location", "Set the Y location at the end of the event."),

                            new("smoothStart", new EntityTypes.Float(0.01f, 1f, 0.2f), "Start Smoothness", "Set the smoothness at the start of the event."),
                            new("smoothEnd", new EntityTypes.Float(0.01f, 1f, 0.2f), "End Smoothness", "Set the smoothness at the end of the event."),

                            new("roundStart", new EntityTypes.Float(0f, 1f, 1f), "Start Roundness", "Set the roundness at the start of the event."),
                            new("roundEnd", new EntityTypes.Float(0f, 1f, 1f), "End Roundness", "Set the roundness at the end of the event."),
                            new("rounded", false, "Rounded", "Toggle if the vignette should be equal on all sides."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "colorStart", "smoothStart", "roundStart", "xLocStart", "yLocStart" })
                            }),
                        }
                    },
                    new GameAction("cabb", "Chromatic Aberration", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 1f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 1f, 1f), "End Intensity", "Set the intensity at the end of the event."),
                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart" })
                            }),
                        }
                    },
                    new GameAction("bloom", "Bloom", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 100f, 0f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 100f, 1f), "End Intensity", "Set the intensity at the end of the event."),

                            new("colorStart", Color.white, "Start Tint", "Set the tint at the start of the event."),
                            new("colorEnd", Color.white, "End Tint", "Set the tint at the end of the event."),

                            new("thresholdStart", new EntityTypes.Float(0f, 100f, 1f), "Start Threshold", "Set the threshold at the start of the event."),
                            new("thresholdEnd", new EntityTypes.Float(0f, 100f, 1f), "End Threshold", "Set the threshold at the end of the event."),

                            new("softKneeStart", new EntityTypes.Float(0f, 1f, 0.5f), "Start Soft Knee", "Set the soft knee at the start of the event."),
                            new("softKneeEnd", new EntityTypes.Float(0f, 1f, 0.5f), "End Soft Knee", "Set the soft knee at the end of the event."),

                            new("anaStart", new EntityTypes.Float(-1f, 1f, 0f), "Start Anamorphic Ratio", "Set the anamorphic ratio at the start of the event."),
                            new("anaEnd", new EntityTypes.Float(-1f, 1f, 0f), "End Anamorphic Ratio", "Set the anamorphic ratio at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "colorStart", "thresholdStart", "softKneeStart", "anaStart" })
                            }),
                        }
                    },
                    new GameAction("lensD", "Lens Distortion", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(-100f, 100f, 0f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(-100f, 100f, 1f), "End Intensity", "Set the intensity at the end of the event."),

                            new("xStart", new EntityTypes.Float(0f, 1f, 1f), "Start X Multiplier", "Set the X multiplier at the start of the event."),
                            new("xEnd", new EntityTypes.Float(0f, 1f, 1f), "End X Multiplier", "Set the X multiplier at the end of the event."),
                            new("yStart", new EntityTypes.Float(0f, 1f, 1f), "Start Y Multiplier", "Set the Y multiplier at the start of the event."),
                            new("yEnd", new EntityTypes.Float(0f, 1f, 1f), "End Y Multiplier", "Set the Y multiplier at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "xStart", "yStart" })
                            }),
                        }
                    },
                    new GameAction("grain", "Grain", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 1f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 1f, 1f), "End Intensity", "Set the intensity at the end of the event."),

                            new("sizeStart", new EntityTypes.Float(0.3f, 3f, 1f), "Start Size", "Set the size at the start of the event."),
                            new("sizeEnd", new EntityTypes.Float(0.3f, 3f, 1f), "End Size", "Set the size at the end of the event."),

                            new("colored", true, "Colored", "Toggle if the grain will be colored."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "sizeStart" })
                            }),
                        }
                    },

                    new GameAction("colorGrading", "Color Grading", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("tempStart", new EntityTypes.Float(-100f, 100f), "Start Temperature", "Set the temperature at the start of the event."),
                            new("tempEnd", new EntityTypes.Float(-100f, 100f), "End Temperature", "Set the temperature at the end of the event."),

                            new("tintStart", new EntityTypes.Float(-100f, 100f), "Start Tint", "Set the tint at the start of the event."),
                            new("tintEnd", new EntityTypes.Float(-100f, 100f), "End Tint", "Set the tint at the end of the event."),

                            new("colorStart", Color.white, "Start Color Filter", "Set the color filter at the start of the event."),
                            new("colorEnd", Color.white, "End Color Filter", "Set the color filter at the end of the event."),

                            new("hueShiftStart", new EntityTypes.Float(-180f, 180f), "Start Hue Shift", "Set the hue shift at the start of the event."),
                            new("hueShiftEnd", new EntityTypes.Float(-180f, 180f), "End Hue Shift", "Set the hue shift at the end of the event."),

                            new("satStart", new EntityTypes.Float(-100f, 100f), "Start Saturation", "Set the saturation at the start of the event."),
                            new("satEnd", new EntityTypes.Float(-100f, 100f), "End Saturation", "Set the saturation at the end of the event."),

                            new("brightStart", new EntityTypes.Float(-100f, 100f), "Start Brightness", "Set the brightness at the start of the event."),
                            new("brightEnd", new EntityTypes.Float(-100f, 100f), "End Brightness", "Set the brightness at the end of the event."),

                            new("conStart", new EntityTypes.Float(-100f, 100f), "Start Contrast", "Set the contrast at the start of the event."),
                            new("conEnd", new EntityTypes.Float(-100f, 100f), "End Contrast", "Set the contrast at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "tempStart", "tintStart", "colorStart", "hueShiftStart", "satStart", "brightStart", "conStart" })
                            }),
                        }
                    },

                    new GameAction("gaussBlur", "Gaussian Blur", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 5f, 0f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 5f, 1f), "End Intensity", "Set the intensity at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart" })
                            }),
                        }
                    },

                    new GameAction("pixelQuad", "Pixelize", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("pixelSizeStart", new EntityTypes.Float(0.00f, 1f, 0.00f), "Start Pixel Size", "Set the pixel size at the start of the event."),
                            new("pixelSizeEnd", new EntityTypes.Float(0.00f, 1f, 0.5f), "End Pixel Size", "Set the pixel size at the end of the event."),
                            new("ratioStart", new EntityTypes.Float(0.2f, 5f, 1f), "Start Pixel Ratio", "Set the pixel ratio at the start of the event."),
                            new("ratioEnd", new EntityTypes.Float(0.2f, 5f, 1f), "End Pixel Ratio", "Set the pixel ratio at the end of the event."),
                            new("xScaleStart", new EntityTypes.Float(0.2f, 5f, 1f), "Start X Scale", "Set the X scale of the pixels at the start of the event."),
                            new("xScaleEnd", new EntityTypes.Float(0.2f, 5f, 1f), "End X Scale", "Set the X scale of the pixels at the end of the event."),
                            new("yScaleStart", new EntityTypes.Float(0.2f, 5f, 1f), "Start Y Scale", "Set the Y scale of the pixels at the start of the event."),
                            new("yScaleEnd", new EntityTypes.Float(0.2f, 5f, 1f), "End Y Scale", "Set the Y scale of the pixels at the end of the event."),
                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "pixelSizeStart", "ratioStart", "xScaleStart", "yScaleStart" })
                            }),
                        }
                    },

                    new GameAction("retroTv", "Retro TV", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 1f, 0f), "Start Distortion", "Set the distortion at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 1f, 0.2f), "End Distortion", "Set the distortion at the end of the event."),

                            new("rgbStart", new EntityTypes.Float(0f, 1f, 1f), "Start RGB Blend", "Set the RGB blend at the start of the event."),
                            new("rgbEnd", new EntityTypes.Float(0f, 1f, 1f), "End RGB Blend", "Set the RGB blend at the end of the event."),

                            new("bottomStart", new EntityTypes.Float(0f, 1f, 0.02f), "Start Bottom Collapse", "Set the bottom collapse at the start of the event."),
                            new("bottomEnd", new EntityTypes.Float(0f, 1f, 0.02f), "End Bottom Collapse", "Set the bottom collapse at the end of the event."),

                            new("noiseStart", new EntityTypes.Float(0f, 1f, 0.3f), "Start Noise", "Set the noise at the start of the event."),
                            new("noiseEnd", new EntityTypes.Float(0f, 1f, 0.3f), "End Noise", "Set the noise at the end of the event."),

                            new("HSonVHS", false, "VHS Effects", "Toggle if VHS effects are enabled." , new()
                            {
                                new Param.CollapseParam((x, _) => (bool)x, new string[] { "bleedIntStart","bleedIntEnd","bleedIteration","vhsGrainStart","vhsGrainEnd",
                                "grainScaleStart","grainScaleEnd","stripeDenStart","stripeDenEnd","stripeOpacStart","stripeOpacEnd","edgeIntStart","edgeIntEnd","edgeDistStart","edgeDistEnd"
                                 })
                            }
                            ),

                            new("bleedIntStart", new EntityTypes.Float(0f, 1f, 0.5f), "Start Bleed", "Set the color bleeding at the start of the event."),
                            new("bleedIntEnd", new EntityTypes.Float(0f, 1f, 0.5f), "End Bleed", "Set the color bleeding at the end of the event."),

                            new("bleedIteration", new EntityTypes.Integer(2,8,2), "Bleed Iterations", "Sets the amount of iterations of color bleeding."),

                            new("vhsGrainStart", new EntityTypes.Float(0f,1f,0.1f), "Start Grain", "Sets the intensity of the grain at the start of the event. Grain looks different than the Grain block."),
                            new("vhsGrainEnd", new EntityTypes.Float(0f,1f,0.1f), "End Grain", "Sets the intensity of the grain at the end of the event. Grain looks different than the Grain block."),

                            new("grainScaleStart", new EntityTypes.Float(0f,1f,0.1f), "Start Grain Scale", "Sets the size of the grain at the start of the event.Grain looks different than the Grain block."),
                            new("grainScaleEnd", new EntityTypes.Float(0f,1f,0.1f), "End Grain Scale", "Sets the size of the grain at the end of the event.Grain looks different than the Grain block."),                            

                            new("stripeDenStart", new EntityTypes.Float(0f, 1f, 0.1f), "Start Stripe Density", "Set the stripe density at the start of the event."),
                            new("stripeDenEnd", new EntityTypes.Float(0f, 1f, 0.1f), "End Stripe Density", "Set the stripe density at the end of the event."),

                            new("stripeOpacStart", new EntityTypes.Float(0f, 1f, 1f), "Start Stripe Opacity", "Set the stripe opacity at the start of the event."),
                            new("stripeOpacEnd", new EntityTypes.Float(0f, 1f, 1f), "End Stripe Opacity", "Set the stripe opacity at the end of the event."),

                            new("edgeIntStart", new EntityTypes.Float(0f, 2f, 0.5f), "Start Edge Sharpness", "Set the edge sharpness at the start of the event."),
                            new("edgeIntEnd", new EntityTypes.Float(0f, 2f, 0.5f), "End Edge Sharpness", "Set the edge sharpness at the end of the event."),

                            new("edgeDistStart", new EntityTypes.Float(0f, 0.005f, 0.002f), "Start Edge Distance", "Set the edge sharpness at the start of the event."),
                            new("edgeDistEnd", new EntityTypes.Float(0f, 0.005f, 0.002f), "End Edge Distance", "Set the edge sharpness at the end of the event."),


                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "rgbStart", "bottomStart", "noiseStart",
                                "bleedIntStart","vhsGrainStart","grainScaleStart","stripeDenStart","stripeOpacStart","edgeIntStart","edgeDistStart"})
                            }),
                        }
                    },

                    new GameAction("scanJitter", "Scan Line Jitter", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 1f, 0f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 1f, 0.1f), "End Intensity", "Set the intensity at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart" })
                            }),
                        }
                    },

                    new GameAction("analogNoise", "Analog Noise", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 1f, 0f), "Start Speed", "Set the speed at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 1f, 0.5f), "End Speed", "Set the speed at the end of the event."),

                            new("fadingStart", new EntityTypes.Float(0f, 1f, 0f), "Start Fading", "Set the fading at the start of the event."),
                            new("fadingEnd", new EntityTypes.Float(0f, 1f, 0.1f), "End Fading", "Set the fading at the end of the event."),

                            new("thresholdStart", new EntityTypes.Float(0f, 1f, 0f), "Start Threshold", "Set the threshold at the start of the event."),
                            new("thresholdEnd", new EntityTypes.Float(0f, 1f, 0.8f), "End Threshold", "Set the threshold at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "fadingStart", "thresholdStart"})
                            }),
                        }
                    },

                    new GameAction("screenJump", "Screen Jump", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0f, 1f, 0f), "Start Intensity", "Set the intensity at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0f, 1f, 0.01f), "End Intensity", "Set the intensity at the end of the event."),

                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart" })
                            }),
                        }
                    },

                    new GameAction("sobelNeon", "Neon", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("intenStart", new EntityTypes.Float(0.0f, 1f, 0.0f), "Start Intensity", "Set the edge fade at the start of the event."),
                            new("intenEnd", new EntityTypes.Float(0.0f, 1f, 1f), "End Intensity", "Set the edge fade at the end of the event."),

                            new("edgeWidthStart", new EntityTypes.Float(0.00f, 5f, 0.0f), "Start Edge Width", "Set the edge width at the start of the event."),
                            new("edgeWidthEnd", new EntityTypes.Float(0.00f, 5f, 2f), "End Edge Width", "Set the edge width at the end of the event."),

                            new("bgFadeStart", new EntityTypes.Float(0f, 1f, 1f), "Start Background Presence", "Set the background presence at the start of the event."),
                            new("bgFadeEnd", new EntityTypes.Float(0f, 1f, 0f), "End Background Presence", "Set the background presence at the end of the event."),


                            new("brightnessStart", new EntityTypes.Float(0f, 2f, 1f), "Start Brightness", "Set the brightness at the start of the event."),
                            new("brightnessEnd", new EntityTypes.Float(0f, 2f, 1f), "End Brightness", "Set the brightness at the end of the event."),



                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "intenStart", "edgeWidthStart", "bgFadeStart", "brightnessStart" })
                            }),
                        }
                    },

                    
                    new GameAction("screenTiling", "Tile Screen", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("xStart", new EntityTypes.Float(1, 100, 1), "Start Horizontal Tiles", "Set the number of horizontal tiles at the start of the event."),
                            new("xEnd", new EntityTypes.Float(1, 100, 1), "End Horizontal Tiles", "Set the number of horizontal tiles at the end of the event."),
                            new("yStart", new EntityTypes.Float(1, 100, 1), "Start Vertical Tiles", "Set the number of vertical tiles at the start of the event."),
                            new("yEnd", new EntityTypes.Float(1, 100, 1), "End Vertical Tiles", "Set the number of vertical tiles at the end of the event."),
                            new("axis", StaticCamera.ViewAxis.All, "Axis", "Set if only a specific axis should be modified."),
                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "xStart", "yStart" })
                            }),
                        }
                    },
                    new GameAction("scrollTiles", "Scroll Tiles", "VFX")
                    {
                        resizable = true,
                        parameters = new()
                        {
                            new("xScrollStart", new EntityTypes.Float(-100, 100, 0), "Start Horizontal Scroll", "Set the horizontal scroll at the start of the event."),
                            new("xScrollEnd", new EntityTypes.Float(-100, 100, 0), "End Horizontal Scroll", "Set the horizontal scroll at the end of the event."),
                            new("yScrollStart", new EntityTypes.Float(-100, 100, 0), "Start Vertical Scroll", "Set the vertical scroll at the start of the event."),
                            new("yScrollEnd", new EntityTypes.Float(-100, 100, 0), "End Vertical Scroll", "Set the vertical scroll at the end of the event."),
                            new("axis", StaticCamera.ViewAxis.All, "Axis", "Set if only a specific axis should be modified."),
                            new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.", new()
                            {
                                new((x, y) => (Util.EasingFunction.Ease)x != Util.EasingFunction.Ease.Instant, new string[] { "xScrollStart", "yScrollStart" })
                            }),
                        }
                    },


                }),

                new Minigame("advanced", "Advanced", "", false, true, new List<GameAction>()
                {
                    new GameAction("play animation", "Play Animation", "Play", 0.5f, false,
                        new List<Param>()
                        {
                            new Param("getAnimators", new EntityTypes.Button("No Game", e => {
                                var gm = GameManager.instance;
                                Minigame game = gm.GetGameInfo(gm.currentGame);
                                if (game != null) {
                                    Animator[] animators = gm.minigameObj.transform.GetComponentsInChildren<Animator>();
                                    // not in an update loop so it's fine :3
                                    ((EntityTypes.DropdownObj)e["animator"]).SetValues(animators.Select(anim => {
                                        Transform obj = anim.transform;
                                        List<string> path = new() { obj.name };
                                        for (int i = 0; i < 10; i++) // not a while loop because i don't trust myself
                                        {
                                            if (obj.parent.name == game.name || obj.parent == null) break;
                                            obj = obj.parent;
                                            path.Insert(0, obj.name);
                                        }
                                        return string.Join('/', path);
                                    }).ToList());
                                }
                                return game?.displayName ?? "No Game";
                            }), "Get Animators", "Get all the animators in the current minigame scene. (Make sure to have the minigame you want loaded!)", new() {
                                new((x, _) => (string)x != "No Game", "animator", "getAnimations")
                            }),
                            new Param("animator", new EntityTypes.Dropdown(), "Animator", "Specify which animator in the scene to play an animation on."),
                            new Param("getAnimations", new EntityTypes.Button("", e => {
                                var gm = GameManager.instance;
                                Minigame game = gm.GetGameInfo(gm.currentGame);
                                string animPath = ((EntityTypes.DropdownObj)e["animator"]).CurrentValue;
                                Animator animator = null;
                                if (!string.IsNullOrEmpty(animPath)) {
                                    var animObj = gm.minigameObj.transform.Find(animPath);
                                    if (animObj != null && animObj.TryGetComponent(out animator) && animator != null) {
                                        List<string> animationClips = new();
                                        foreach (var clip in animator.runtimeAnimatorController.animationClips) {
                                            if (clip != null) {
                                                animationClips.Add(clip.name);
                                            }
                                        }
                                        ((EntityTypes.DropdownObj)e["animation"]).SetValues(animationClips);
                                    }
                                }
                                return animator != null ? animator.name : "";
                            }), "Get Animations", "Get all the animations in the selected animator.", new() {
                                new((x, _) => (string)x != "", "animation", "scale")
                            }),
                            new Param("animation", new EntityTypes.Dropdown(), "Animation", "Specify the name of the animation to play."),
                            new Param("scale", new EntityTypes.Float(0, 5, 0.5f), "Animation Scale", "The time scale of the animation. Higher values are faster."),
                        },
                        delegate {
                            var e = eventCaller.currentEntity;
                            GameManager.instance.PlayAnimationArbitrary(e["animator"].CurrentValue, e["animation"].CurrentValue, e["scale"]);
                        }
                    ),
                    new GameAction("play sfx", "Play SFX", "Play", 0.5f, true,
                        new List<Param>()
                        {
                            new Param("game", new EntityTypes.Dropdown(), "Which Game", "Specify the game's sfx to play. An empty input will play global sfx."),
                            new Param("getSfx", new EntityTypes.Button("", e => {
                                string gameName = ((EntityTypes.DropdownObj)e["game"]).CurrentValue;
                                List<string> clips;
                                if (eventCaller.minigames.TryGetValue(gameName, out Minigame game) && game != null) {
                                    IEnumerable<AudioClip> audioClips = game.GetResourcesAssetBundle().LoadAllAssets<AudioClip>();
                                    var localAssBun = game.GetAudioAssetBundle();
                                    if (localAssBun != null) {
                                        audioClips = audioClips.Concat(localAssBun.LoadAllAssets<AudioClip>());
                                    }
                                    clips = audioClips.Select(x => x.name).ToList();
                                } else {
                                    // this is probably the best way to do it?
                                    clips = new() { "applause", "metronome", "miss", "nearMiss", "perfectMiss", "skillStar" };
                                }
                                clips.Sort((s1, s2) => s1.CompareTo(s2));
                                EntityTypes.DropdownObj sfxDD = e["sfxName"];
                                sfxDD.SetValues(clips);
                                return clips.Count > 0 ? (game != null ? game.displayName : "Common") : "Empty!";
                            }), "Get SFX", "Get all the sfx in the selected minigame."),
                            new Param("sfxName", new EntityTypes.Dropdown(), "SFX Name", "The name of the sfx to play."),
                            new Param("useSemitones", false, "Use Semitones", "Toggle to use semitones instead of straight pitch.", new() {
                                new((x, e) => (bool)x, "semitones", "cents"),
                                new((x, e) => !(bool)x, "pitch"),
                            }),
                            new Param("semitones", new EntityTypes.Integer(-EntityTypes.Note.maxSemitones, EntityTypes.Note.maxSemitones, 0), "Semitones", "The semitones of the sfx."),
                            new Param("cents", new EntityTypes.Integer(-100, 100, 0), "Cents", "The cents of the sfx."),
                            new Param("pitch", new EntityTypes.Float(0, 5, 1), "Pitch", "The pitch of the sfx."),
                            new Param("volume", new EntityTypes.Float(0, 5, 1), "Volume", "The volume of the sfx."),
                            new Param("offset", new EntityTypes.Integer(-500, 500), "Offset (ms)", "The offset of the sfx in milliseconds."),
                            new Param("loop", false, "Loop", "Loop the sfx for the length of the block."),
                        },
                        preFunction : delegate {
                            var e = eventCaller.currentEntity;
                            float pitch = e["pitch"];
                            if (e["useSemitones"]) pitch = SoundByte.GetPitchFromCents((e["semitones"] * 100) + e["cents"], false);
                            GameManager.PlaySFXArbitrary(e.beat, e.length, e["game"].CurrentValue, e["sfxName"].CurrentValue, pitch, e["volume"], e["loop"], e["offset"]);
                        }
                    ),
                }),
            };

            foreach (var game in defaultGames)
            {
                eventCaller.minigames.Add(game.name, game);
            }

            LoadMinigames(eventCaller);

            // im so sorry
            eventCaller.minigames["advanced"].actions
                .Find(a => a.actionName == "play sfx").parameters[0].parameter = new EntityTypes.Dropdown(0, new string[] { "common" }.Concat(eventCaller.minigames.Keys.Skip(defaultGames.Count)).ToArray());
        }
    }
}