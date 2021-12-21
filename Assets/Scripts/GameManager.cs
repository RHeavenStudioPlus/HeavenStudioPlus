using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Starpelly;
using Newtonsoft.Json;

namespace RhythmHeavenMania
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public List<Event> Events = new List<Event>();

        public int currentEvent;

        public TextAsset txt;

        public float startOffset;

        [Serializable]
        public class Event : ICloneable
        {
            public float spawnTime;
            public string eventName;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            SortEventsList();

            string json = txt.text;
            Events = JsonConvert.DeserializeObject<List<Event>>(json);

            SortEventsList();

            StartCoroutine(Begin());

            GlobalGameManager.Init();
        }

        private IEnumerator Begin()
        {
            yield return new WaitForSeconds(startOffset);
            Conductor.instance.musicSource.Play();
        }

        private void Update()
        {
            if (Events.Count < 1)
                return;

            List<float> floats = Events.Select(c => c.spawnTime).ToList();

            if (currentEvent < Events.Count && currentEvent >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= floats[currentEvent])
                {
                    currentEvent++;
                }
            }
        }

        public void SortEventsList()
        {
            Events.Sort((x, y) => x.spawnTime.CompareTo(y.spawnTime));
        }

        public void SetCurrentEventToClosest()
        {
            if (Events.Count > 0)
            {
                List<float> floats = Events.Select(c => c.spawnTime).ToList();
                currentEvent = floats.IndexOf(Mathp.GetClosestInList(floats, Conductor.instance.songPositionInBeats));
            }
        }


        private void OnGUI()
        {
            // GUI.Box(new Rect(0, 0, 300, 50), $"SongPosInBeats: {Conductor.instance.songPositionInBeats}");
        }
    }
}