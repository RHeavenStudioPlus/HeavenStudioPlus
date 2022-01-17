using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Newtonsoft.Json;
using TMPro;

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

            Tooltip.instance.AddTooltip(NewBTN.gameObject, "New");
            Tooltip.instance.AddTooltip(OpenBTN.gameObject, "Open");
            Tooltip.instance.AddTooltip(SaveBTN.gameObject, "Save");
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

        public void SetGameEventTitle(string txt)
        {
            GameEventSelectorTitle.text = txt;
        }
    }
}