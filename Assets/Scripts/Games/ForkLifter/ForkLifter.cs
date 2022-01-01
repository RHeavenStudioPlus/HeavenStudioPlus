using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

using DG.Tweening;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class ForkLifter : Minigame
    {
        public static ForkLifter instance;

        [Header("References")]
        public ForkLifterHand ForkLifterHand;
        private GameManager GameManager;


        [Header("Objects")]
        public Animator handAnim;
        public GameObject flickedObject;
        public SpriteRenderer peaPreview;

        public Sprite[] peaSprites;
        public Sprite[] peaHitSprites;


        private List<Beatmap.Entity> allPlayerActions = new List<Beatmap.Entity>();

        private void Awake()
        {
            instance = this;
        }

        public override void OnGameSwitch()
        {
            ForkLifterHand.CheckNextFlick();
        }

        private void Start()
        {
            GameManager = GameManager.instance;
            // allPlayerActions = GameManager.Events.FindAll(c => c.eventName != "gulp" && c.eventName != "sigh" && c.eventName != "prepare" && c.eventName != "end");

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
        }

        public void Flick(float beat, int type)
        {
            Jukebox.PlayOneShotGame("forkLifter/flick");
            handAnim.Play("Hand_Flick", 0, 0);
            GameObject fo = Instantiate(flickedObject);
            fo.transform.parent = flickedObject.transform.parent;
            fo.GetComponent<Pea>().startBeat = beat;
            fo.GetComponent<Pea>().type = type;
            fo.SetActive(true);
        }
    }

}