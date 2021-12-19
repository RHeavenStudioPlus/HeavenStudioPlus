using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Starpelly;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Event> Events = new List<Event>();
    public List<float> AutoPlay = new List<float>();
    public List<Event> allPlayerActions = new List<Event>();

    public int currentEvent, currentEventAutoplay, currentEventPlayer;

    public TextAsset txt;

    public bool autoplay = false;

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

        allPlayerActions = Events.FindAll(c => c.eventName != "gulp" && c.eventName != "sigh" && c.eventName != "prepare" && c.eventName != "end");
        AutoPlay = allPlayerActions.Select(c => c.spawnTime + 2).ToList();

        /*List<Event> temp = new List<Event>();
        for (int i = 0; i < allPlayerActions.Count; i++)
        {
            if (i - 1 > 0)
            {
                if (Mathp.IsWithin(allPlayerActions[i - 1].spawnTime, allPlayerActions[i].spawnTime - 1f, allPlayerActions[i].spawnTime))
                {
                    // do nothing lul
                    continue;
                }
            }
            Event e = (Event)allPlayerActions[i].Clone();
            e.spawnTime = allPlayerActions[i].spawnTime - 1;
            e.eventName = "prepare";
            
            temp.Add(e);
        }

        string s = JsonConvert.SerializeObject(temp);
        print(s);*/

        StartCoroutine(Begin());

        GlobalGameManager.Init();
    }

    private IEnumerator Begin()
    {
        yield return new WaitForSeconds(startOffset);
        Conductor.instance.musicSource.Play();
        GoForAPerfect.instance.Enable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ForkLifter.instance.Flick(Conductor.instance.songPositionInBeats, 0);
        if (Input.GetKeyDown(KeyCode.W))
            ForkLifter.instance.Flick(Conductor.instance.songPositionInBeats, 1);
        if (Input.GetKeyDown(KeyCode.E))
            ForkLifter.instance.Flick(Conductor.instance.songPositionInBeats, 2);
        if (Input.GetKeyDown(KeyCode.R))
            ForkLifter.instance.Flick(Conductor.instance.songPositionInBeats, 3);

        if (Events.Count < 1)
            return;

        List<float> floats = Events.Select(c => c.spawnTime).ToList();

        if (currentEvent < Events.Count && currentEvent >= 0)
        {
            if (Conductor.instance.songPositionInBeats >= floats[currentEvent])
            {

                switch (Events[currentEvent].eventName)
                {
                    case "pea":
                        currentEventPlayer++;
                        ForkLifter.instance.Flick(Events[currentEvent].spawnTime, 0);
                        break;
                    case "topbun":
                        currentEventPlayer++;
                        ForkLifter.instance.Flick(Events[currentEvent].spawnTime, 1);
                        break;
                    case "burger":
                        currentEventPlayer++;
                        ForkLifter.instance.Flick(Events[currentEvent].spawnTime, 2);
                        break;
                    case "bottombun":
                        currentEventPlayer++;
                        ForkLifter.instance.Flick(Events[currentEvent].spawnTime, 3);
                        break;
                    case "gulp":
                        ForkLifterPlayer.instance.Eat();
                        break;
                    case "sigh":
                        Jukebox.PlayOneShot("sigh");
                        break;
                    case "prepare":
                        ForkLifterHand.instance.Prepare();
                        break;
                    case "end":
                        GlobalGameManager.LoadScene(2, 0.45f);
                        break;

                }
                currentEvent++;
            }
        }

        if (autoplay)
        {
            if (currentEventAutoplay < AutoPlay.Count && currentEventAutoplay >= 0)
            {
                if (Conductor.instance.songPositionInBeats >= AutoPlay[currentEventAutoplay])
                {
                    ForkLifterPlayer.instance.Stab();
                    currentEventAutoplay++;
                }
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
        if (AutoPlay.Count > 0)
        {
            currentEvent = AutoPlay.IndexOf(Mathp.GetClosestInList(AutoPlay, Conductor.instance.songPositionInBeats));
            currentEventPlayer = AutoPlay.IndexOf(Mathp.GetClosestInList(AutoPlay, Conductor.instance.songPositionInBeats));
        }
    }


    private void OnGUI()
    {
        // GUI.Box(new Rect(0, 0, 300, 50), $"SongPosInBeats: {Conductor.instance.songPositionInBeats}");
    }
}
