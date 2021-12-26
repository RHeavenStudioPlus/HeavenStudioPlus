using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class ForkLifterHand : MonoBehaviour
    {
        public SpriteRenderer fastSprite;

        public Sprite[] fastSprites;

        public void CheckNextFlick()
        {
            // allPlayerActions = EventCaller.GetAllInGameManagerList("forkLifter", new string[] { "gulp", "sigh", "prepare" });
            var allPlayerActions = EventCaller.GetAllPlayerEntities("forkLifter");
            int currentPlayerEvent = GameManager.instance.currentPlayerEvent - EventCaller.GetAllPlayerEntitiesExceptBeforeBeat("forkLifter", Conductor.instance.songPositionInBeats).Count;

            if (currentPlayerEvent < allPlayerActions.Count)
            {
                switch (allPlayerActions[currentPlayerEvent].datamodel.Split('/')[1])
                {
                    case "pea":
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[0];
                        fastSprite.sprite = fastSprites[0];
                        break;
                    case "topbun":
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[1];
                        fastSprite.sprite = fastSprites[0];
                        break;
                    case "burger":
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[2];
                        fastSprite.sprite = fastSprites[1];
                        break;
                    case "bottombun":
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[3];
                        fastSprite.sprite = fastSprites[0];
                        break;
                }
            }
            else
            {
                ForkLifter.instance.peaPreview.sprite = null;
            }
        }

        public void Prepare()
        {
            Jukebox.PlayOneShotGame("forkLifter/flickPrepare");
            GetComponent<Animator>().Play("Hand_Prepare", 0, 0);
        }
    }
}