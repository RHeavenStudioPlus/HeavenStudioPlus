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

        List<Beatmap.Entity> allPlayerActions;

        public void CheckNextFlick()
        {
            allPlayerActions = EventCaller.GetAllInGameManagerListExcept("forkLifter", new string[] { "gulp", "sigh", "prepare" });

            if (GameManager.instance.currentPlayerEvent < allPlayerActions.Count)
            {
                switch (allPlayerActions[GameManager.instance.currentPlayerEvent].datamodel.Split('/')[1])
                {
                    case "pea":
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[0];
                        fastSprite.sprite = fastSprites[0];
                        break;
                    case "topbun":
                        fastSprite.sprite = fastSprites[0];
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[1];
                        break;
                    case "burger":
                        fastSprite.sprite = fastSprites[1];
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[2];
                        break;
                    case "bottombun":
                        fastSprite.sprite = fastSprites[0];
                        ForkLifter.instance.peaPreview.sprite = ForkLifter.instance.peaSprites[3];
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
            GetComponent<Animator>().Play("Hand_Prepare");
        }
    }
}