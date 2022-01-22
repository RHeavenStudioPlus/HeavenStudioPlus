using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Newtonsoft.Json;
using TMPro;
using Starpelly;

namespace RhythmHeavenMania.Editor
{
    public class Editor : MonoBehaviour
    {
        private Initializer Initializer;

        [SerializeField] private Canvas MainCanvas;
        [SerializeField] public Camera EditorCamera;

        [Header("Rect")]
        [SerializeField] private RenderTexture ScreenRenderTexture;
        [SerializeField] private RawImage Screen;
        [SerializeField] private RectTransform GridGameSelector;

        [Header("Components")]
        [SerializeField] private Timeline Timeline;
        [SerializeField] private TMP_Text GameEventSelectorTitle;

        [Header("Toolbar")]
        [SerializeField] private Button NewBTN;
        [SerializeField] private Button OpenBTN;
        [SerializeField] private Button SaveBTN;
        [SerializeField] private Button UndoBTN;
        [SerializeField] private Button RedoBTN;

        public static List<TimelineEventObj> EventObjs = new List<TimelineEventObj>();

        public static Editor instance { get; private set; }

        private void Start()
        {
            instance = this;
            Initializer = GetComponent<Initializer>();
        }

        public void Init()
        {
            GameManager.instance.GameCamera.targetTexture = ScreenRenderTexture;
            GameManager.instance.CursorCam.targetTexture = ScreenRenderTexture;
            Screen.texture = ScreenRenderTexture;

            GameManager.instance.Init();
            Timeline.Init();

            for (int i = 0; i < EventCaller.instance.minigames.Count; i++)
            {
                GameObject GameIcon_ = Instantiate(GridGameSelector.GetChild(0).gameObject, GridGameSelector);
                GameIcon_.GetComponent<Image>().sprite = GameIcon(EventCaller.instance.minigames[i].name);
                GameIcon_.gameObject.SetActive(true);
                GameIcon_.name = EventCaller.instance.minigames[i].displayName;
            }

            Tooltip.AddTooltip(NewBTN.gameObject, "New");
            Tooltip.AddTooltip(OpenBTN.gameObject, "Open");
            Tooltip.AddTooltip(SaveBTN.gameObject, "Save");
            Tooltip.AddTooltip(UndoBTN.gameObject, "Undo");
            Tooltip.AddTooltip(RedoBTN.gameObject, "Redo");
        }

        public void Update()
        {
            // This is buggy
            /*if (Conductor.instance.isPlaying || Conductor.instance.isPaused)
            {
                GetComponent<Selections>().enabled = false;
                GetComponent<Selector>().enabled = false;
                GetComponent<BoxSelection>().enabled = false;
            }
            else
            {
                GetComponent<Selections>().enabled = true;
                GetComponent<Selector>().enabled = true;
                GetComponent<BoxSelection>().enabled = true;
            }*/

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                List<TimelineEventObj> ev = new List<TimelineEventObj>();
                for (int i = 0; i < Selections.instance.eventsSelected.Count; i++) ev.Add(Selections.instance.eventsSelected[i]);
                CommandManager.instance.Execute(new Commands.Deletion(ev));
            }

            if (CommandManager.instance.canUndo())
                UndoBTN.transform.GetChild(0).GetComponent<Image>().color = "BE72FF".Hex2RGB();
            else
                UndoBTN.transform.GetChild(0).GetComponent<Image>().color = Color.gray;

            if (CommandManager.instance.canRedo())
                RedoBTN.transform.GetChild(0).GetComponent<Image>().color = "7299FF".Hex2RGB();
            else
                RedoBTN.transform.GetChild(0).GetComponent<Image>().color = Color.gray;

            if (Input.GetMouseButtonUp(0) && Timeline.instance.CheckIfMouseInTimeline())
            {
                List<TimelineEventObj> selectedEvents = Timeline.instance.eventObjs.FindAll(c => c.selected == true && c.eligibleToMove == true);

                if (selectedEvents.Count > 0)
                {
                    List<TimelineEventObj> result = new List<TimelineEventObj>();

                    for (int i = 0; i < selectedEvents.Count; i++)
                    {
                        result.Add(selectedEvents[i]);
                        selectedEvents[i].OnUp();
                    }
                    CommandManager.instance.Execute(new Commands.Move(result));
                }
            }
        }

        public static Sprite GameIcon(string name)
        {
            return Resources.Load<Sprite>($"Sprites/Editor/GameIcons/{name}");
        }

        public void CopyJson()
        {
            string json = string.Empty;
            json = JsonConvert.SerializeObject(GameManager.instance.Beatmap);

            Debug.Log(json);
        }

        public void DebugSave()
        {
            // temp
            string path = UnityEditor.AssetDatabase.GetAssetPath(GameManager.instance.txt);
            path = Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + path;
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(GameManager.instance.Beatmap));
            Debug.Log("Saved to " + path);
        }

        public void SetGameEventTitle(string txt)
        {
            GameEventSelectorTitle.text = txt;
        }
    }
}