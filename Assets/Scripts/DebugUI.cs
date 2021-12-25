using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;


// hardcoded because im lazy
namespace RhythmHeavenMania
{
    public class DebugUI : MonoBehaviour
    {
        public GameObject Template;

        private TMP_Text SongPosBeats;
        private TMP_Text BPM;
        private TMP_Text currEvent;
        private TMP_Text eventLength;
        private TMP_Text eventType;

        private void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject debug = Instantiate(Template, Template.transform.parent);
                debug.SetActive(true);
                debug.transform.localPosition = new Vector3(Template.transform.localPosition.x, Template.transform.localPosition.y - 44.9301f * i);

                switch (i)
                {
                    case 0:
                        SongPosBeats = debug.transform.GetChild(0).GetComponent<TMP_Text>();
                        break;
                    case 1:
                        BPM = debug.transform.GetChild(0).GetComponent<TMP_Text>();
                        break;
                    case 2:
                        currEvent = debug.transform.GetChild(0).GetComponent<TMP_Text>();
                        break;
                    case 3:
                        eventLength = debug.transform.GetChild(0).GetComponent<TMP_Text>();
                        break;
                    case 4:
                        eventType = debug.transform.GetChild(0).GetComponent<TMP_Text>();
                        break;
                }
            }
        }

        private void Update()
        {
            SongPosBeats.text = $"SongPosBeats: {Conductor.instance.songPositionInBeats}";
            BPM.text = $"BPM: {Conductor.instance.songBpm}";
            if (GameManager.instance.Beatmap.entities.Count > 0)
            if (GameManager.instance.currentEvent - 1 >= 0)
            {
                currEvent.text = $"CurrentEvent: {GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent - 1].datamodel}";
                eventLength.text = $"Event Length: {GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent - 1].length}";
                eventType.text = $"Event Type: {GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent - 1].type}";
                }
            else
            {
                currEvent.text = $"CurrentEvent: {GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].datamodel}";
                eventLength.text = $"Event Length: {GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].length}";
                eventType.text = $"Event Type: {GameManager.instance.Beatmap.entities[GameManager.instance.currentEvent].type}";
            }
        }
    }
}